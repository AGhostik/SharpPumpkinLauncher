namespace JsonReader.PublicData.Runtime;

public sealed class RuntimeFiles
{
    public RuntimeFiles(IReadOnlyList<RuntimeFile> files)
    {
        Files = files;
    }

    public IReadOnlyList<RuntimeFile> Files { get; }
}