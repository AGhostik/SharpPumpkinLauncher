namespace JsonReader.PublicData.Game;

public sealed class Rule
{
    public Rule(string action)
    {
        Action = action;
    }

    public string Action { get; }
    public Dictionary<string, bool>? Features { get; set; }
    public Os? Os { get; set; }
}