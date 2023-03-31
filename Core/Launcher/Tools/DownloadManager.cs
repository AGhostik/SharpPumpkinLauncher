using System.Diagnostics;
using System.Net;
using System.Text;

namespace Launcher.Tools;

public static class DownloadManager
{
    public static async Task<bool> CheckConnection()
    {
        using var client = new HttpClient();
        var response = await client.GetAsync("https://google.com");
        return response.StatusCode == HttpStatusCode.OK;
    }
    
    public static async Task<string> DownloadJsonAsync(string url, CancellationToken cancellationToken)
    {
        using var client = new HttpClient();
        var response = await client.GetAsync(new Uri(url), cancellationToken);
        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        return json;
    }
    
    public static async Task<string> DownloadFile(string url, CancellationToken cancellationToken, Action<long>? bytesReceived = null)
    {
        using var client = new HttpClient();
        using var response = client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken).Result;
        response.EnsureSuccessStatusCode();

        await using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
            
        var totalRead = 0L;
        var buffer = new byte[8192];
        var isMoreToRead = true;
        var sb = new StringBuilder();

        do
        {
            var read = await contentStream.ReadAsync(buffer, cancellationToken);
            if (read == 0)
            {
                isMoreToRead = false;
            }
            else
            {
                sb.Append(Encoding.Default.GetString(buffer, 0, read));
                totalRead += read;
                bytesReceived?.Invoke(totalRead);
            }
        } while (isMoreToRead);

        return sb.ToString();
    }

    public static async Task DownloadFilesParallel(IEnumerable<(Uri source, string filename)> download,
        CancellationToken cancellationToken, Action<long>? bytesReceived = null, Action? failed = null)
    {
        var totalRead = 0L;

        var internalTokenSource = new CancellationTokenSource();
        var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, internalTokenSource.Token);
        
        await Parallel.ForEachAsync(
            download,
            new ParallelOptions { MaxDegreeOfParallelism = 10, CancellationToken = linkedTokenSource.Token },
            DownloadFileParallel);
        
        async ValueTask DownloadFileParallel((Uri source, string filename) data, CancellationToken token)
        {
            const int attemptCount = 3;
            var currentAttempt = 0;
            
            var localRead = 0L;
            var isMoreToRead = true;

            do
            {
                try
                {
                    using var client = new HttpClient();
                    using var response = client
                        .GetAsync(data.source, HttpCompletionOption.ResponseHeadersRead, token).Result;
                    response.EnsureSuccessStatusCode();

                    await using var contentStream = await response.Content.ReadAsStreamAsync(token);
                    await using var fileStream = new FileStream(data.filename, FileMode.Create, FileAccess.Write,
                        FileShare.None, 8192, true);

                    var buffer = new byte[8192];

                    do
                    {
                        var read = await contentStream.ReadAsync(buffer, token);
                        if (read == 0)
                        {
                            isMoreToRead = false;
                        }
                        else
                        {
                            await fileStream.WriteAsync(buffer.AsMemory(0, read), token);

                            totalRead += read;
                            localRead += read;
                            bytesReceived?.Invoke(totalRead);
                        }
                    } while (isMoreToRead);
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"failed download file {data.filename}\n{e}");

                    if (isMoreToRead)
                    {
                        currentAttempt++;
                        totalRead -= localRead;
                        localRead = 0;
                        bytesReceived?.Invoke(totalRead);
                        
                        await Task.Delay(1000 * currentAttempt, cancellationToken);
                    }
                    else
                    {
                        return;
                    }
                }
            } while (isMoreToRead && currentAttempt < attemptCount);

            if (currentAttempt >= attemptCount)
            {
                failed?.Invoke();
                internalTokenSource.Cancel();
            }
        }
    }
}