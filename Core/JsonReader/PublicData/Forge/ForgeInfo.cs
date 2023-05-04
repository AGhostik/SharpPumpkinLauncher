using JsonReader.PublicData.Game;

namespace JsonReader.PublicData.Forge;

public sealed class ForgeInfo
{
    public ForgeInfo(string mainClass, string arguments, IReadOnlyList<Library> libraries)
    {
        MainClass = mainClass;
        Arguments = arguments;
        Libraries = libraries;
    }

    public string MainClass { get; }
    public string Arguments { get; }
    public IReadOnlyList<Library> Libraries { get; }
}