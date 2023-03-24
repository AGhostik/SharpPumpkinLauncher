namespace JsonReader.PublicData.Game;

public sealed class ArgumentItem
{
    public ArgumentItem(IReadOnlyList<string> values, IReadOnlyList<Rule>? rules)
    {
        Values = values;
        Rules = rules;
    }

    public IReadOnlyList<string> Values { get; }
    public IReadOnlyList<Rule>? Rules { get; }
}