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
    private readonly ProfileModel _profileModel;
    private readonly VersionsLoader _versionsLoader;
    
    private string? _profileName;
    private string? _playerName;
    private Version? _selectedVersion;
    private Version? _selectedForgeVersion;
    private Versions? _versions;
    private bool _forge;
    private bool _custom;
    private bool _latest;
    private bool _release;
    private bool _snapshot;
    private bool _beta;
    private bool _alpha;

    private Action<ProfileViewModel>? _save;
    private Action? _cancel;

    private ProfileViewModel()
    {
        _profileModel = ServiceProvider.ProfileModel;
        _versionsLoader = ServiceProvider.VersionsLoader;
        _versionsLoader.VersionsLoaded += SetVersions;
        
        SaveProfileCommand = ReactiveCommand.Create(SaveProfile, CanSaveProfile);
        CloseProfileControlCommand = ReactiveCommand.Create(CloseProfileControl);
    }

    public void OnDelete()
    {
        _versionsLoader.VersionsLoaded -= SetVersions;
        _profileModel.DeleteProfile(this);
    }

    public static ProfileViewModel CreateDefault()
    {
        ProfileNameValidationAttribute.RestrictedName = null;
        
        var profileViewModel = new ProfileViewModel() 
        { 
            Latest = true,
            ProfileName = "Minecraft game",
            _save = null,
            _cancel = null
        };

        profileViewModel._versionsLoader.VersionsLoaded += OnVersionsLoaded;

        return profileViewModel;

        void OnVersionsLoaded(Versions versions)
        {
            profileViewModel.SelectedVersion = versions.Latest;
            profileViewModel.SaveToSettings();
            profileViewModel._versionsLoader.VersionsLoaded -= OnVersionsLoaded;
        }
    }
    
    public static ProfileViewModel CreateNew(string? playerName, IEnumerable<string?> restrictedNames, 
        Action<ProfileViewModel> save, Action cancel)
    {
        ProfileNameValidationAttribute.RestrictedName = new List<string?>(restrictedNames);
        
        return new ProfileViewModel() 
        { 
            Latest = true,
            PlayerName = playerName,
            _save = profile =>
            {
                profile.SaveToSettings();
                save.Invoke(profile);
            },
            _cancel = cancel,
        };
    }

    public static ProfileViewModel Edit(ProfileViewModel profileViewModel, IEnumerable<string?> restrictedNames,
        Action<ProfileViewModel> save, Action cancel)
    {
        var profileName = profileViewModel.ProfileName;
        profileViewModel._save = profile =>
        {
            if (!string.IsNullOrEmpty(profileName))
                profile.SaveEdited(profileName);
            
            save.Invoke(profile);
        };
        profileViewModel._cancel = cancel;
        profileViewModel.Latest = true;
        
        ProfileNameValidationAttribute.RestrictedName = new List<string?>(restrictedNames);
        
        return profileViewModel;
    }

    public static ProfileViewModel Load(ProfileData profileData)
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
        };
        
        profileViewModel._versionsLoader.VersionsLoaded += OnVersionsLoaded;
        void OnVersionsLoaded(Versions versions)
        {
            if (!string.IsNullOrEmpty(profileData.MinecraftVersion) &&
                versions.AllVersions.TryGetValue(profileData.MinecraftVersion, out var selectedVersion))
                profileViewModel.SelectedVersion = selectedVersion;
            
            profileViewModel._versionsLoader.VersionsLoaded -= OnVersionsLoaded;
        }

        return profileViewModel;
    }

    private void SaveToSettings()
    {
        _profileModel.SaveProfile(this);
    }

    private void SaveEdited(string originalProfileName)
    {
        _profileModel.ReplaceProfile(originalProfileName, this);
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
            OnSelectedVersionChanged();
            UpdateCanSaveProfile();
        }
    }

    public ObservableCollection<Version> Versions { get; } = new();
    
    public Version? SelectedForgeVersion
    {
        get => _selectedForgeVersion;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedForgeVersion, value);
            UpdateCanSaveProfile();
        }
    }

    public ObservableCollection<Version> ForgeVersions { get; } = new();

    public bool Forge
    {
        get => _forge;
        set => this.RaiseAndSetIfChanged(ref _forge, value);
    }

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

    private async void OnSelectedVersionChanged()
    {
        if (Forge && SelectedVersion != null)
        {
            var forgeVersions = await _profileModel.RequestForgeVersions(SelectedVersion.Id);
            
            ForgeVersions.Clear();
            for (var i = 0; i < forgeVersions.Forge.Count; i++)
                ForgeVersions.Add(forgeVersions.Forge[i]);
        }
    }
    
    private void SetVersions(Versions versions)
    {
        _versions = versions;

        if (SelectedVersion != null)
        {
            if (versions.AllVersions.TryGetValue(SelectedVersion.Id, out var version) && !SelectedVersion.Equals(version))
                SelectedVersion = version;
        }

        UpdateVisibleVersions();
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