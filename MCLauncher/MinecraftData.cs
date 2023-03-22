using System;
using System.Threading.Tasks;
using Launcher;
using Launcher.PublicData;

namespace MCLauncher;

public class MinecraftData
{
    private readonly MinecraftLauncher _minecraftLauncher;

    public MinecraftData(MinecraftLauncher minecraftLauncher)
    {
        _minecraftLauncher = minecraftLauncher;
    }

    public event Action? VersionsLoaded;
    
    public bool IsVersionsLoaded { get; private set; }
    
    public Versions? Versions { get; private set; }

    public async Task LoadAvailableVersions()
    {
        IsVersionsLoaded = false;
        Versions = await _minecraftLauncher.GetAvailableVersions();
        IsVersionsLoaded = true;
        VersionsLoaded?.Invoke();
    }
}