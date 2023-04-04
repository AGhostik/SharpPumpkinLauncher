using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Subjects;
using Launcher.PublicData;
using MinecraftLauncher.Main.Validation;
using ReactiveUI;
using UserSettings;
using Version = Launcher.PublicData.Version;

namespace MinecraftLauncher.Main.Profile;

public sealed class ProfileViewModel : ReactiveObject
{
    private const string SetLastVersion = "SetLastVersion";
    
    private string? _profileName;
    private string? _playerName;
    private Version? _selectedVersion;
    private Versions? _versions;
    private bool _custom;
    private bool _latest;
    private bool _release;
    private bool _snapshot;
    private bool _beta;
    private bool _alpha;

    private string? _loadedSelectedVersion;
    private Action<ProfileViewModel>? _versionLoaded;
    private Action<ProfileViewModel>? _save;
    private Action? _cancel;

    private ProfileViewModel()
    {
        SaveProfileCommand = ReactiveCommand.Create(SaveProfile, CanSaveProfile);
        CloseProfileControlCommand = ReactiveCommand.Create(CloseProfileControl);
    }

    public static ProfileViewModel CreateDefault(Action<ProfileViewModel> versionLoaded)
    {
        ProfileNameValidationAttribute.RestrictedName = null;
        
        return new ProfileViewModel() 
        { 
            Latest = true,
            ProfileName = "Minecraft game",
            _save = null,
            _cancel = null,
            _loadedSelectedVersion = SetLastVersion,
            _versionLoaded = versionLoaded,
        };
    }
    
    public static ProfileViewModel CreateNew(IEnumerable<string?> restrictedNames, Action<ProfileViewModel> save,
        Action cancel)
    {
        ProfileNameValidationAttribute.RestrictedName = new List<string?>(restrictedNames);
        
        return new ProfileViewModel() 
        { 
            Latest = true,
            _save = save,
            _cancel = cancel
        };
    }

    public static ProfileViewModel Edit(ProfileViewModel profileViewModel, IEnumerable<string?> restrictedNames,
        Action<string?, ProfileViewModel> save, Action cancel)
    {
        var profileName = profileViewModel.ProfileName;
        profileViewModel._save = profile => save.Invoke(profileName, profile);
        profileViewModel._cancel = cancel;
        
        ProfileNameValidationAttribute.RestrictedName = new List<string?>(restrictedNames);
        
        return profileViewModel;
    }

    public static ProfileViewModel Load(ProfileData profileData, Action<ProfileViewModel> versionLoaded)
    {
        var profileViewModel = new ProfileViewModel()
        {
            _profileName = profileData.Name,
            _playerName = profileData.PlayerNickname,
            Alpha = profileData.Alpha,
            Beta = profileData.Beta,
            Custom = profileData.Custom,
            Snapshot = profileData.Snapshot,
            Release = profileData.Release,
            SelectedVersion = null,
            _loadedSelectedVersion = profileData.MinecraftVersion,
            _versionLoaded = versionLoaded,
        };

        return profileViewModel;
    }

    public void SetVersions(Versions versions)
    {
        _versions = versions;

        if (!string.IsNullOrEmpty(_loadedSelectedVersion))
        {
            if (_loadedSelectedVersion == SetLastVersion)
                SelectedVersion = versions.Latest;
            else if (versions.AllVersions.TryGetValue(_loadedSelectedVersion, out var selectedVersion))
                SelectedVersion = selectedVersion;

            _versionLoaded?.Invoke(this);
        }

        UpdateVisibleVersions();
    }
    
    [ProfileNameValidation]
    public string? ProfileName
    {
        get => _profileName;
        set
        {
            this.RaiseAndSetIfChanged(ref _profileName, value);
            UpdateCanSaveProfile();
        }
    }

    [PlayerNameValidation]
    public string? PlayerName
    {
        get => _playerName;
        set
        {
            this.RaiseAndSetIfChanged(ref _playerName, value);
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
        var value = ProfileNameValidation.IsProfileNameValid(ProfileName, ProfileNameValidationAttribute.RestrictedName) &&
                    PlayerNameValidation.IsPlayerNameValid(PlayerName) &&
                    SelectedVersion != null && !string.IsNullOrEmpty(SelectedVersion.Id);
        
        CanSaveProfile.OnNext(value);
    }
    
    private void UpdateVisibleVersions()
    {
        Versions.Clear();
        
        if (_versions != null)
        {
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
        
        if (SelectedVersion != null && !Versions.Contains(SelectedVersion))
            Versions.Add(SelectedVersion);
    }
}