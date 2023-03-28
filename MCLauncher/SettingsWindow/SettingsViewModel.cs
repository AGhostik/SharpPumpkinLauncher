using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MCLauncher.LauncherWindow;
using MCLauncher.Messages;
using UserSettings;
using Version = Launcher.PublicData.Version;

namespace MCLauncher.SettingsWindow;

public class SettingsViewModel : ObservableObject
{
    private readonly SettingsModel _model;
    private string? _profileName;
    private string? _playerName;
    private string? _gameDirectory;
    private string? _javaFile;
    private string? _jvmArgs;
    private LauncherVisibility? _selectedLauncherVisibility;
    private Version? _selectedVersion;
    private bool _showCustom;
    private bool _showRelease;
    private bool _showSnapshot;
    private bool _showBeta;
    private bool _showAlpha;
    private bool _canSave;

    public SettingsViewModel(SettingsModel model, ProfileViewModel? profileViewModel)
    {
        _model = model;

        _canSave = true;
        _showRelease = true;
        _javaFile = model.GetJavaPath();
        _selectedLauncherVisibility = LauncherVisibility.KeepOpen;
        
        OpenDirectoryCommand = new RelayCommand(OpenDirectory);
        SaveCommand = new RelayCommand(Save);
        
        SetUpProperties(profileViewModel);
        UpdateVersions();
    }

    public string? ProfileName
    {
        get => _profileName;
        set => SetProperty(ref _profileName, value);
    }

    public string? PlayerName
    {
        get => _playerName;
        set => SetProperty(ref _playerName, value);
    }

    public string? GameDirectory
    {
        get => _gameDirectory;
        set => SetProperty(ref _gameDirectory, value);
    }

    public string? JavaFile
    {
        get => _javaFile;
        set => SetProperty(ref _javaFile, value);
    }

    public string? JvmArgs
    {
        get => _jvmArgs;
        set => SetProperty(ref _jvmArgs, value);
    }

    public LauncherVisibility? SelectedLauncherVisibility
    {
        get => _selectedLauncherVisibility;
        set => SetProperty(ref _selectedLauncherVisibility, value);
    }

    public ICollection<Version> Versions { get; } = new ObservableCollection<Version>();

    public Version? SelectedVersion
    {
        get => _selectedVersion;
        set => SetProperty(ref _selectedVersion, value);
    }

    public bool ShowCustom
    {
        get => _showCustom;
        set
        {
            SetProperty(ref _showCustom, value);
            UpdateVersions();
        }
    }

    public bool ShowRelease
    {
        get => _showRelease;
        set
        {
            SetProperty(ref _showRelease, value);
            UpdateVersions();
        }
    }

    public bool ShowSnapshot
    {
        get => _showSnapshot;
        set
        {
            SetProperty(ref _showSnapshot, value);
            UpdateVersions();
        }
    }

    public bool ShowBeta
    {
        get => _showBeta;
        set
        {
            SetProperty(ref _showBeta, value);
            UpdateVersions();
        }
    }

    public bool ShowAlpha
    {
        get => _showAlpha;
        set
        {
            SetProperty(ref _showAlpha, value);
            UpdateVersions();
        }
    }

    public ICommand OpenDirectoryCommand { get; }

    private void OpenDirectory()
    {
        //
    }

    public bool CanSave
    {
        get => _canSave;
        set => SetProperty(ref _canSave, value);
    }

    public ICommand SaveCommand { get; }

    private void Save()
    {
        var profileData = new ProfileData()
        {
            Name = _profileName,
            GameDirectory = _gameDirectory,
            JvmArgs = _jvmArgs,
            MinecraftVersion = _selectedVersion?.Id,
            PlayerNickname = _playerName
        };

        WeakReferenceMessenger.Default.Send(new ProfileSaved(profileData));
    }

    private void SetUpProperties(ProfileViewModel? profileViewModel)
    {
        if (profileViewModel == null)
            return;
        
        _profileName = profileViewModel.Name;
        _playerName = profileViewModel.PlayerNickname;
        _gameDirectory = profileViewModel.GameDirectory;
        _jvmArgs = profileViewModel.JvmArgs;
            
        if (_model.Versions == null)
            return;

        if (_model.TryGetVersion(profileViewModel.MinecraftVersion, out var version))
            _selectedVersion = version;
    }

    private void UpdateVersions()
    {
        Versions.Clear();
        
        if (_model.Versions == null)
            return;

        if (_showAlpha)
        {
            for (var i = 0; i < _model.Versions.Alpha.Count; i++)
                Versions.Add(_model.Versions.Alpha[i]);
        }
        
        if (_showBeta)
        {
            for (var i = 0; i < _model.Versions.Beta.Count; i++)
                Versions.Add(_model.Versions.Beta[i]);
        }
        
        if (_showSnapshot)
        {
            for (var i = 0; i < _model.Versions.Snapshot.Count; i++)
                Versions.Add(_model.Versions.Snapshot[i]);
        }
        
        if (_showRelease)
        {
            for (var i = 0; i < _model.Versions.Release.Count; i++)
                Versions.Add(_model.Versions.Release[i]);
        }

        //todo:
        // if (_showCustom)
        // {
        //     for (var i = 0; i < _model.Versions.Release.Count; i++)
        //         Versions.Add(_model.Versions.Release[i]);
        // }
        
        if (_selectedVersion != null && !Versions.Contains(_selectedVersion))
            Versions.Add(_selectedVersion);
    }
}