namespace Launcher.Data;

internal interface IMinecraftFile
{
    string Url { get; }
    string FileName { get; }
    string Sha1 { get; }
    int Size { get; }
}