using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Subjects;
using Avalonia.Data;
using Launcher.PublicData;
using ReactiveUI;
using UserSettings;
using Version = Launcher.PublicData.Version;

namespace MinecraftLauncher.Main.Profile;

public sealed class ProfileViewModel : ReactiveObject
{
    private string? _profileName;
    private string? _playerName;
    private string? _directory;
    private Version? _selectedVersion;
    private bool _custom;
    private bool _latest;
    private bool _release;
    private bool _snapshot;
    private bool _beta;
    private bool _alpha;
    private static Action<ProfileViewModel>? _save;
    private static Action? _cancel;
    private static Versions? _versions;
    private static List<string?>? _restrictedNames;

    private ProfileViewModel()
    {
        SaveProfileCommand = ReactiveCommand.Create(SaveProfile, CanSaveProfile);
        CloseProfileControlCommand = ReactiveCommand.Create(CloseProfileControl);
    }

    public static ProfileViewModel CreateNew(Versions versions, IEnumerable<string?> restrictedNames,
        Action<ProfileViewModel> save, Action cancel)
    {
        _restrictedNames = new List<string?>(restrictedNames);
        _versions = versions;
        _save = save;
        _cancel = cancel;
        return new ProfileViewModel() { Latest = true };
    }

    public static ProfileViewModel Edit(ProfileViewModel profileViewModel, Versions versions,
        IEnumerable<string?> restrictedNames, Action<ProfileViewModel> save, Action cancel)
    {
        _restrictedNames = new List<string?>(restrictedNames);
        _versions = versions;
        _save = save;
        _cancel = cancel;
        return profileViewModel;
    }

    public static ProfileViewModel Load(ProfileData profileData, Versions versions)
    {
        Version? selectedVersion = null;
        if (!string.IsNullOrEmpty(profileData.MinecraftVersion))
            versions.AllVersions.TryGetValue(profileData.MinecraftVersion, out selectedVersion);
        
        return new ProfileViewModel()
        {
            ProfileName = profileData.Name,
            PlayerName = profileData.PlayerNickname,
            Directory = profileData.GameDirectory,
            SelectedVersion = selectedVersion,
        };
    }
    
    public string? ProfileName
    {
        get => _profileName;
        set
        {
            if (!IsProfileNameValid(value, out var errorKey))
                throw new DataValidationException(errorKey);

            this.RaiseAndSetIfChanged(ref _profileName, value);
            UpdateCanSaveProfile();
        }
    }

    public string? PlayerName
    {
        get => _playerName;
        set
        {
            if (!IsPlayerNameValid(value, out var errorKey))
                throw new DataValidationException(errorKey);
            
            this.RaiseAndSetIfChanged(ref _playerName, value);
            UpdateCanSaveProfile();
        }
    }

    public string? Directory
    {
        get => _directory;
        set
        {
            if (!IsDirectoryValid(value, out var errorKey))
                throw new DataValidationException(errorKey);
            
            this.RaiseAndSetIfChanged(ref _directory, value);
            UpdateCanSaveProfile();
        }
    }

    public Version? SelectedVersion
    {
        get => _selectedVersion;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedVersion, value);
            UpdateCanSaveProfile();
        }
    }

    public ObservableCollection<Version> Versions { get; } = new();

    public bool Custom
    {
        get => _custom;
        set
        {
            this.RaiseAndSetIfChanged(ref _custom, value);
            UpdateVisibleVersions();
        }
    }

    public bool Latest
    {
        get => _latest;
        set
        {
            this.RaiseAndSetIfChanged(ref _latest, value);
            UpdateVisibleVersions();
        }
    }

    public bool Release
    {
        get => _release;
        set
        {
            this.RaiseAndSetIfChanged(ref _release, value);
            UpdateVisibleVersions();
        }
    }

    public bool Snapshot
    {
        get => _snapshot;
        set
        {
            this.RaiseAndSetIfChanged(ref _snapshot, value);
            UpdateVisibleVersions();
        }
    }

    public bool Beta
    {
        get => _beta;
        set
        {
            this.RaiseAndSetIfChanged(ref _beta, value);
            UpdateVisibleVersions();
        }
    }

    public bool Alpha
    {
        get => _alpha;
        set
        {
            this.RaiseAndSetIfChanged(ref _alpha, value);
            UpdateVisibleVersions();
        }
    }

    public ReactiveCommand<Unit, Unit> SaveProfileCommand { get; }
    private Subject<bool> CanSaveProfile { get; } = new();
    public ReactiveCommand<Unit, Unit> CloseProfileControlCommand { get; }

    private void SaveProfile()
    {
        _save?.Invoke(this);
    }
    
    private void CloseProfileControl()
    {
        _cancel?.Invoke();
    }
    
    private void UpdateCanSaveProfile()
    {
        var value = IsProfileNameValid(ProfileName) &&
                    IsPlayerNameValid(PlayerName) &&
                    IsDirectoryValid(Directory);
        
        CanSaveProfile.OnNext(value);
    }
    
    private void UpdateVisibleVersions()
    {
        if (_versions == null)
            return;
        
        if (Alpha)
        {
            for (var i = 0; i < _versions.Alpha.Count; i++)
                Versions.Add(_versions.Alpha[i]);
        }
        if (Beta)
        {
            for (var i = 0; i < _versions.Beta.Count; i++)
                Versions.Add(_versions.Beta[i]);
        }
        if (Snapshot)
        {
            for (var i = 0; i < _versions.Snapshot.Count; i++)
                Versions.Add(_versions.Snapshot[i]);
        }
        if (Release)
        {
            for (var i = 0; i < _versions.Release.Count; i++)
                Versions.Add(_versions.Release[i]);
        }
        if (Latest)
        {
            if (_versions.Latest != null)
                Versions.Add(_versions.Latest);
            
            if (_versions.LatestSnapshot != null)
                Versions.Add(_versions.LatestSnapshot);
        }
        if (Custom)
        {
            //todo:
            //for (var i = 0; i < _versions.Release.Count; i++)
            //    Versions.Add(_versions.Release[i]);
        }
    }

    private static bool IsProfileNameValid(string? profileName)
    {
        return IsProfileNameValid(profileName, out _);
    }
    
    private static bool IsProfileNameValid(string? profileName, out string errorKey)
    {
        if (string.IsNullOrEmpty(profileName))
        {
            errorKey = "Empty";
            return false;
        }

        if (_restrictedNames != null && _restrictedNames.Contains(profileName))
        {
            errorKey = "Restricted";
            return false;
        }

        errorKey = string.Empty;
        return true;
    }

    private static bool IsPlayerNameValid(string? playerName)
    {
        return IsPlayerNameValid(playerName, out _);
    }

    private static bool IsPlayerNameValid(string? playerName, out string errorKey)
    {
        if (string.IsNullOrEmpty(playerName))
        {
            errorKey = "Empty";
            return false;
        }

        if (playerName.Length < 3)
        {
            errorKey = "Short";
            return false;
        }
        
        if (playerName.Length > 16)
        {
            errorKey = "Long";
            return false;
        }
        
        for (var i = 0; i < playerName.Length; i++)
        {
            var c = playerName[i];
            
            if (char.IsLetterOrDigit(c) || c == '_')
                continue;

            errorKey = "RestrictedChar";
            return false;
        }

        errorKey = string.Empty;
        return true;
    }

    private static bool IsDirectoryValid(string? path)
    {
        return IsDirectoryValid(path, out _);
    }
    
    private static bool IsDirectoryValid(string? path, out string errorKey)
    {
        if (string.IsNullOrEmpty(path))
        {
            errorKey = "Empty";
            return false;
        }
        
        errorKey = string.Empty;
        return true;
    }
}