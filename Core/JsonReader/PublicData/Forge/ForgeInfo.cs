using JsonReader.PublicData.Game;

namespace JsonReader.PublicData.Forge;

public sealed class ForgeInfo
{
    public ForgeInfo(string mainClass, string? legacyGameArguments, ForgeArguments? forgeArguments,
        IReadOnlyList<Library> libraries)
    {
        MainClass = mainClass;
        LegacyGameArguments = legacyGameArguments;
        ForgeArguments = forgeArguments;
        Libraries = libraries;
    }

    public string MainClass { get; }
    public string? LegacyGameArguments { get; }
    public ForgeArguments? ForgeArguments { get; }
    public IReadOnlyList<Library> Libraries { get; }
}