using System;
using System.Threading.Tasks;
using Launcher.PublicData;

namespace SharpPumpkinLauncher;

public sealed class VersionsLoader
{
    private readonly Launcher.MinecraftLauncher _minecraftLauncher;

    private Action<Versions>? _versionsLoaded;
    private Versions? _offlineVersions;
    private Versions? _onlineVersions;
    private Versions? _allVersions;
    
    public VersionsLoader(Launcher.MinecraftLauncher minecraftLauncher, SettingsManager settingsManager)
    {
        _minecraftLauncher = minecraftLauncher;
        
        settingsManager.DirectoryChanged += LoadOfflineVersions;
        
        LoadOfflineVersions(settingsManager.CurrentSettings.Directory);
        LoadOnlineVersions();
    }
    
    public event Action<Versions>? VersionsLoaded
    {
        add
        {
            _versionsLoaded += value;
            InvokeVersionsLoaded();
        }
        remove => _versionsLoaded -= value;
    }
    
    public async Task<ForgeVersions> RequestForgeVersions(string versionId)
    {
        return await _minecraftLauncher.GetOnlineForgeVersions(versionId);
    }

    private async void LoadOfflineVersions(string directory)
    {
        _offlineVersions = await _minecraftLauncher.GetAvailableVersions(directory);
        if (_offlineVersions.IsEmpty)
            return;
        
        TryMergeOfflineAndOnlineVersions();
        InvokeVersionsLoaded();
    }
    
    private async void LoadOnlineVersions()
    {
        _onlineVersions = await _minecraftLauncher.GetOnlineAvailableVersions();
        if (_onlineVersions.IsEmpty)
            return;
        
        TryMergeOfflineAndOnlineVersions();
        InvokeVersionsLoaded();
    }
    
    private void TryMergeOfflineAndOnlineVersions()
    {
        if (_offlineVersions == null || _onlineVersions == null)
            return;
        
        _allVersions = Versions.Merge(_onlineVersions, _offlineVersions);
    }

    private void InvokeVersionsLoaded()
    {
        if (_allVersions != null)
            _versionsLoaded?.Invoke(_allVersions);
        else if (_onlineVersions != null)
            _versionsLoaded?.Invoke(_onlineVersions);
        else if (_offlineVersions != null)
            _versionsLoaded?.Invoke(_offlineVersions);
    }
}