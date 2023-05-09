namespace JsonReader.PublicData.Forge;

public sealed class ForgeInstallProcessor
{
    public ForgeInstallProcessor(string jar, IReadOnlyList<string> classpath, IReadOnlyList<string> args,
        IReadOnlyList<string>? sides, IReadOnlyList<(string, string)>? outputs)
    {
        Jar = jar;
        Classpath = classpath;
        Args = args;
        Sides = sides;
        Outputs = outputs;
    }

    public string Jar { get; }
    
    public IReadOnlyList<string> Classpath { get; }
    
    public IReadOnlyList<string> Args { get; }
    
    public IReadOnlyList<string>? Sides { get; }
    
    public IReadOnlyList<(string, string)>? Outputs { get; }
}