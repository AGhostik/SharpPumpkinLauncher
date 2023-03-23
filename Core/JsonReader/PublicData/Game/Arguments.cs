namespace JsonReader.PublicData.Game;

public sealed class Arguments
{
    public Arguments(ArgumentItem[] game, ArgumentItem[] jvm)
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

    public ArgumentItem[] Game { get; }
    public ArgumentItem[] Jvm { get; }
    public LegacyArguments? LegacyArguments { get; }
}