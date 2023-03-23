namespace JsonReader.PublicData.Game;

public sealed class ArgumentItem
{
    public ArgumentItem(string[] values, Rule[]? rules)
    {
        Values = values;
        Rules = rules;
    }

    public string[] Values { get; }
    public Rule[]? Rules { get; }
}