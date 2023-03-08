using System;
using CommunityToolkit.Mvvm.ComponentModel;
using MCLauncher.SettingsWindow;

namespace MCLauncher.Tools;

[Serializable]
public class Profile : ObservableObject
{
    private string _currentVersion;
    private string _gameDirectory;
    private string _javaFile;
    private string _jvmArgs;
    private LauncherVisibility _launcherVisibility;
    private string? _name;
    private string _nickname;
    private bool _showAlpha;
    private bool _showBeta;
    private bool _showCustom;
    private bool _showRelease;
    private bool _showSnapshot;
    
    public event Action? SelectedVersionsChanged;

    public Profile()
    {
        _name = string.Empty;
        _nickname = string.Empty;
        _javaFile = string.Empty;
        _gameDirectory = AppDomain.CurrentDomain.BaseDirectory + "Minecraft";
        _jvmArgs = string.Empty;
        _launcherVisibility = LauncherVisibility.KeepOpen;
        _currentVersion = string.Empty;
        _showCustom = false;
        _showRelease = true;
        _showSnapshot = false;
        _showBeta = false;
        _showAlpha = false;
    }

    public string? Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public string Nickname
    {
        get => _nickname;
        set => SetProperty(ref _nickname, value);
    }

    public string JavaFile
    {
        get => _javaFile;
        set => SetProperty(ref _javaFile, value);
    }

    public string GameDirectory
    {
        get => _gameDirectory;
        set => SetProperty(ref _gameDirectory, value);
    }

    public string JvmArgs
    {
        get => _jvmArgs;
        set => SetProperty(ref _jvmArgs, value);
    }

    public string CurrentVersion
    {
        get => _currentVersion;
        set => SetProperty(ref _currentVersion, value);
    }

    public LauncherVisibility LauncherVisibility
    {
        get => _launcherVisibility;
        set => SetProperty(ref _launcherVisibility, value);
    }

    public bool ShowCustom
    {
        get => _showCustom;
        set
        {
            SetProperty(ref _showCustom, value);
            SelectedVersionsChanged?.Invoke();
        }
    }

    public bool ShowRelease
    {
        get => _showRelease;
        set
        {
            SetProperty(ref _showRelease, value);
            SelectedVersionsChanged?.Invoke();
        }
    }

    public bool ShowSnapshot
    {
        get => _showSnapshot;
        set
        {
            SetProperty(ref _showSnapshot, value);
            SelectedVersionsChanged?.Invoke();
        }
    }

    public bool ShowBeta
    {
        get => _showBeta;
        set
        {
            SetProperty(ref _showBeta, value);
            SelectedVersionsChanged?.Invoke();
        }
    }

    public bool ShowAlpha
    {
        get => _showAlpha;
        set
        {
            SetProperty(ref _showAlpha, value);
            SelectedVersionsChanged?.Invoke();
        }
    }
}