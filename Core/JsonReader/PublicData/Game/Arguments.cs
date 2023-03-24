namespace JsonReader.PublicData.Game;

public sealed class Arguments
{
    public Arguments(IReadOnlyList<ArgumentItem> game, IReadOnlyList<ArgumentItem> jvm)
    {
        Game = game;
        Jvm = jvm;
        LegacyArguments = null;
    }

    public Arguments(LegacyArguments legacyArguments)
    {
        Game = Array.Empty<ArgumentItem>();
        Jvm = Array.Empty<ArgumentItem>();
        LegacyArguments = legacyArguments;
    }

    public IReadOnlyList<ArgumentItem> Game { get; }
    public IReadOnlyList<ArgumentItem> Jvm { get; }
    public LegacyArguments? LegacyArguments { get; }
}