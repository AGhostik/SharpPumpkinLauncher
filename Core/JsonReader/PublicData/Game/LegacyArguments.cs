namespace JsonReader.PublicData.Game;

public sealed class LegacyArguments
{
    public LegacyArguments(string game, string jvm)
    {
        Game = game;
        Jvm = jvm;
    }

    public string Game { get; }
    public string Jvm { get; }
}