namespace MinecraftLauncher.Main;

public sealed class VersionViewModel
{
    public VersionViewModel(string id)
    {
        Id = id;
    }

    public string Id { get; }
}