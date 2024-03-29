using SimpleLogger;

namespace Launcher.Tools;

public static class DownloadManager
{
    private const int MaxAttemptCount = 3;

    private static readonly HttpClient _httpClient = new();
    
    public static async Task<string?> GetRequest(string url, IDictionary<string, string> parameters,
        CancellationToken cancellationToken = default)
    {
        var currentAttempt = 0;
        do
        {
            try
            {
                using var client = new HttpClient();

                var queryString = string.Join("&",
                    parameters.Select(x => $"{Uri.EscapeDataString(x.Key)}={Uri.EscapeDataString(x.Value)}"));
                var urlWithQuery = $"{url}?{queryString}";

                var response = 
                    await client.GetAsync(urlWithQuery, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                return await response.Content.ReadAsStringAsync(cancellationToken);
            }
            catch (Exception e)
            {
                Logger.Log(e);
                currentAttempt++;
                await Delay(1000 * currentAttempt, cancellationToken);
            }
        } while (currentAttempt < MaxAttemptCount);
        return null;
    }
    
    public static async Task<string?> DownloadJsonAsync(string url, CancellationToken cancellationToken)
    {
        var currentAttempt = 0;
        do
        {
            try
            {
                using var client = new HttpClient();
                var response = 
                    await client.GetAsync(new Uri(url), HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                return json;
            }
            catch (Exception e)
            {
                Logger.Log(e);
                currentAttempt++;
                await Delay(1000 * currentAttempt, cancellationToken);
            }
        } while (currentAttempt < MaxAttemptCount);

        return null;
    }

    public static async Task<bool> DownloadFilesParallel(IEnumerable<(Uri source, string filename)> download,
        Action<long>? bytesReceived = null, Action? fileDownloaded = null,
        CancellationToken cancellationToken = default)
    {
        var isSucced = true;
        var totalRead = 0L;

        var internalTokenSource = new CancellationTokenSource();
        var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, internalTokenSource.Token);
        
        await Parallel.ForEachAsync(
            download,
            new ParallelOptions { MaxDegreeOfParallelism = 10, CancellationToken = linkedTokenSource.Token },
            DownloadFileParallel);

        return isSucced;
        
        async ValueTask DownloadFileParallel((Uri source, string filename) data, CancellationToken token)
        {
            var currentAttempt = 0;
            
            var localRead = 0L;
            var isMoreToRead = true;
            
            Logger.Log($"Download file: {data.filename}");

            do
            {
                try
                {
                    using var response = _httpClient
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
                            fileDownloaded?.Invoke();
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
                    Logger.Log(e);

                    if (isMoreToRead)
                    {
                        currentAttempt++;
                        totalRead -= localRead;
                        localRead = 0;
                        bytesReceived?.Invoke(totalRead);

                        await Delay(1000 * currentAttempt, cancellationToken);
                    }
                    else
                    {
                        return;
                    }
                }
            } while (isMoreToRead && currentAttempt < MaxAttemptCount);

            if (currentAttempt >= MaxAttemptCount)
            {
                isSucced = false;
                internalTokenSource.Cancel();
            }
        }
    }

    private static async Task Delay(int millisecondDelay, CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(millisecondDelay, cancellationToken);
        }
        catch (Exception exception)
        {
            Logger.Log(exception);
        }
    }
}