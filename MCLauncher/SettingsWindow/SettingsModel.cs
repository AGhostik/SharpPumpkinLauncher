using System.Collections.Generic;
using Launcher.PublicData;
using Microsoft.Win32;
using Version = Launcher.PublicData.Version;

namespace MCLauncher.SettingsWindow;

public sealed class SettingsModel
{
    private readonly MinecraftData _minecraftData;
    private readonly Dictionary<string, Version> _versions = new();

    public SettingsModel(MinecraftData minecraftData)
    {
        _minecraftData = minecraftData;

        if (_minecraftData.Versions != null)
        {
            AddVersionsToDictionary(_minecraftData.Versions.Alpha);
            AddVersionsToDictionary(_minecraftData.Versions.Beta);
            AddVersionsToDictionary(_minecraftData.Versions.Snapshot);
            AddVersionsToDictionary(_minecraftData.Versions.Release);
        }

        void AddVersionsToDictionary(IReadOnlyList<Version> versions)
        {
            for (var i = 0; i < versions.Count; i++)
            {
                var version = versions[i];
                _versions.Add(version.Id, version);
            }
        }
    }

    public Versions? Versions => _minecraftData.Versions;

    public bool TryGetVersion(string? id, out Version? version)
    {
        if (string.IsNullOrEmpty(id))
        {
            version = null;
            return false;
        }
        
        return _versions.TryGetValue(id, out version);
    }
    
    public string? GetJavaPath()
    {
        const string regJavaPath = "SOFTWARE\\JavaSoft\\JDK";
        
        using var javaKey = Registry.LocalMachine.OpenSubKey(regJavaPath);
        var currentVersionKey = javaKey?.GetValue("CurrentVersion");
        
        if (currentVersionKey == null)
            return null;

        using var homeKey = Registry.LocalMachine.OpenSubKey($"{regJavaPath}\\{currentVersionKey}");
        var javaHome = homeKey?.GetValue("JavaHome");

        if (javaHome == null)
            return null;

        return $"{javaHome}\\bin\\javaw.exe";
    }
}