namespace JsonReader.PublicData.Game;

public sealed class Logging
{
    public Logging(string argument, DownloadFile file)
    {
        Argument = argument;
        File = file;
    }

    public string Argument { get; }
    public DownloadFile File { get; }
}