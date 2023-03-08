using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MCLauncher.Messages;
using MCLauncher.Properties;
using MCLauncher.Tools;

namespace MCLauncher.SettingsWindow;

public class SettingsViewModel : ObservableObject
{
    private readonly ISettingsModel _settingsModel;
    private readonly bool _isNewProfile;
    private string? _oldProfileName;
    private string? _selectedVisibility;

    private AllVersions? _versionsCache;
    private Profile? _currentProfile;
    private string? _title;
    private RelayCommand? _save;

    public SettingsViewModel(ISettingsModel model, bool isNewProfile)
    {
        _settingsModel = model;
        _isNewProfile = isNewProfile;
    }

    public Profile? CurrentProfile
    {
        get => _currentProfile;
        set => SetProperty(ref _currentProfile, value);
    }

    public string? SelectedVisibility
    {
        get => _selectedVisibility;
        set
        {
            SetProperty(ref _selectedVisibility, value);

            if (CurrentProfile != null)
            {
                if (_selectedVisibility == UIResource.KeepLauncherOpen)
                    CurrentProfile.LauncherVisibility = LauncherVisibility.KeepOpen;
                else if (_selectedVisibility == UIResource.HideLauncher)
                    CurrentProfile.LauncherVisibility = LauncherVisibility.Hide;
                else if (_selectedVisibility == UIResource.CloseLauncher)
                    CurrentProfile.LauncherVisibility = LauncherVisibility.Close;
            }
        }
    }

    public List<string> Visibilitys { get; } = new()
    {
        UIResource.KeepLauncherOpen,
        UIResource.HideLauncher,
        UIResource.CloseLauncher
    };

    public ObservableCollection<string> Versions { get; } = new();

    public RelayCommand? Save
    {
        get => _save;
        set => SetProperty(ref _save, value);
    }

    public RelayCommand? OpenDirectory { get; set; }

    public string? Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public async Task Init()
    {
        var isNewProfile = _isNewProfile;
            
        Title = isNewProfile ? UIResource.NewProfileTitle : UIResource.EditProfileTitle;
            
        if (isNewProfile)
            CreateProfile();
        else
            LoadProfile();

        SelectedVisibility = CurrentProfile?.LauncherVisibility switch
        {
            LauncherVisibility.KeepOpen => UIResource.KeepLauncherOpen,
            LauncherVisibility.Close => UIResource.HideLauncher,
            LauncherVisibility.Hide => UIResource.CloseLauncher,
            _ => throw new ArgumentOutOfRangeException()
        };

        await SaveVersionsToCache();
        ShowSelectedVersions();

        CurrentProfile.CurrentVersion = _versionsCache?.Latest ?? string.Empty;

        CurrentProfile.SelectedVersionsChanged += ShowSelectedVersions;

        Save = new RelayCommand(() =>
        {
            if (isNewProfile)
                SaveNewProfile();
            else
                SaveEditedProfile();

            WeakReferenceMessenger.Default.Send(new ProfilesChangedMessage());
        });
        
        OpenDirectory = new RelayCommand(() =>
        {
            _settingsModel.OpenGameDirectory(CurrentProfile.GameDirectory);
        });
    }

    private void ShowSelectedVersions()
    {
        if (CurrentProfile == null || _versionsCache == null)
            return;
        
        Versions.Clear();
        if (CurrentProfile.ShowAlpha)
        {
            foreach (var version in _versionsCache.Alpha)
                Versions.Add(version);
        }

        if (CurrentProfile.ShowBeta)
        {
            foreach (var version in _versionsCache.Beta)
                Versions.Add(version);
        }

        if (CurrentProfile.ShowRelease)
        {
            foreach (var version in _versionsCache.Release)
                Versions.Add(version);
        }

        if (CurrentProfile.ShowSnapshot)
        {
            foreach (var version in _versionsCache.Snapshot)
                Versions.Add(version);
        }

        if (CurrentProfile.ShowCustom)
        {
            foreach (var version in _versionsCache.Custom)
                Versions.Add(version);
        }
    }

    private void SaveNewProfile()
    {
        _settingsModel.SaveProfile(CurrentProfile);
        WeakReferenceMessenger.Default.Send(new StatusMessage(UIResource.NewProfileStatus));
    }

    private void SaveEditedProfile()
    {
        _settingsModel.EditProfile(_oldProfileName, CurrentProfile);
        WeakReferenceMessenger.Default.Send(new StatusMessage(UIResource.ProfileEditedStatus));
    }

    private async Task SaveVersionsToCache()
    {
        _versionsCache = await _settingsModel.DownloadAllVersions();
    }

    private void CreateProfile()
    {
        CurrentProfile = new Profile
        {
            JavaFile = _settingsModel.FindJava()
        };
    }

    private void LoadProfile()
    {
        CurrentProfile = _settingsModel.LoadLastProfile();
        
        if (CurrentProfile == null)
            return;

        _oldProfileName = CurrentProfile.Name;

        switch (CurrentProfile.LauncherVisibility)
        {
            case LauncherVisibility.KeepOpen:
                _selectedVisibility = UIResource.KeepLauncherOpen;
                break;
            case LauncherVisibility.Hide:
                _selectedVisibility = UIResource.HideLauncher;
                break;
            case LauncherVisibility.Close:
                _selectedVisibility = UIResource.CloseLauncher;
                break;
        }
    }
}