namespace Launcher.Data;

internal interface IMinecraftFile
{
    string Url { get; }
    string FileName { get; }
    int Size { get; }
}