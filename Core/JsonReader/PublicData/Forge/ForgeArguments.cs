namespace JsonReader.PublicData.Forge;

public sealed class ForgeArguments
{
    public ForgeArguments(IReadOnlyList<string> jvm, IReadOnlyList<string> game)
    {
        Jvm = jvm;
        Game = game;
    }

    public IReadOnlyList<string> Jvm { get; }
    public IReadOnlyList<string> Game { get; }
}