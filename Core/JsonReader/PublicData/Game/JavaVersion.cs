namespace JsonReader.PublicData.Game;

public sealed class JavaVersion
{
    public JavaVersion(string component, int majorVersion)
    {
        Component = component;
        MajorVersion = majorVersion;
    }

    public string Component { get; }
    public int MajorVersion { get; }
}