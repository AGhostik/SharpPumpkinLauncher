namespace JsonReader.PublicData.Game;

public sealed class Rule
{
    public Rule(string action)
    {
        Action = action;
    }

    public string Action { get; }
    public IReadOnlyDictionary<string, bool>? Features { get; init; }
    public Os? Os { get; init; }
}