using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Launcher.PublicData;
using ReactiveUI;
using SharpPumpkinLauncher.Main.Validation;
using UserSettings;
using Version = Launcher.PublicData.Version;

namespace SharpPumpkinLauncher.Main.Profile;

public sealed class ProfileViewModel : ReactiveObject
{
    private readonly VersionsLoader _versionsLoader;
    
    private string? _profileName;
    private string? _playerName;
    private VersionViewModel? _selectedVersion;
    private VersionViewModel? _selectedForgeVersion;
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
        _versionsLoader = ServiceProvider.VersionsLoader;
        _versionsLoader.VersionsLoaded += SetVersions;
        
        SaveProfileCommand = ReactiveCommand.Create(SaveProfile, CanSaveProfile);
        CloseProfileControlCommand = ReactiveCommand.Create(CloseProfileControl);
    }
    
    public void OnDelete()
    {
        _versionsLoader.VersionsLoaded -= SetVersions;
        SettingsManager.DeleteProfile(this);
    }

    public static ProfileViewModel Create(Version version, string? playerName)
    {
        ProfileNameValidationAttribute.RestrictedName = null;
        
        var profileViewModel = new ProfileViewModel() 
        {
            ProfileName = "Minecraft game",
            PlayerName = playerName,
            SelectedVersion = new VersionViewModel(version),
            Latest = true,
            _save = null,
            _cancel = null
        };

        return profileViewModel;
    }
    
    public static ProfileViewModel CreateNew(string? playerName, IEnumerable<string?> restrictedNames, 
        Action<ProfileViewModel> save, Action cancel)
    {
        ProfileNameValidationAttribute.RestrictedName = new List<string?>(restrictedNames);
        
        var profileViewModel = new ProfileViewModel() 
        {
            PlayerName = playerName,
            Latest = true,
        };
        profileViewModel._save = profile =>
        {
            SettingsManager.SaveProfile(profileViewModel);
            save.Invoke(profile);
        };
        profileViewModel._cancel = cancel;
        
        return profileViewModel;
    }

    public static ProfileViewModel Edit(ProfileViewModel profileViewModel, IEnumerable<string?> restrictedNames,
        Action<ProfileViewModel> save, Action cancel)
    {
        var profileName = profileViewModel.ProfileName;
        profileViewModel._save = profile =>
        {
            if (!string.IsNullOrEmpty(profileName))
            {
                SettingsManager.ReplaceProfile(profileName, profileViewModel);
            }
            
            save.Invoke(profile);
        };
        profileViewModel._cancel = cancel;
        
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
            Forge = profileData.Forge
        };

        if (!string.IsNullOrEmpty(profileData.MinecraftVersion))
        {
            profileViewModel.SelectedVersion =
                new VersionViewModel(profileData.MinecraftVersion, profileData.MinecraftVersionTags);
        }

        if (!string.IsNullOrEmpty(profileData.ForgeVersion))
        {
            profileViewModel.SelectedForgeVersion =
                new VersionViewModel(profileData.ForgeVersion, profileData.ForgeVersionTags);
        }

        return profileViewModel;
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

    public VersionViewModel? SelectedVersion
    {
        get => _selectedVersion;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedVersion, value);
            OnSelectedVersionChanged();
            UpdateCanSaveProfile();
        }
    }

    public ObservableCollection<VersionViewModel> Versions { get; } = new();
    
    public VersionViewModel? SelectedForgeVersion
    {
        get => _selectedForgeVersion;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedForgeVersion, value);
            UpdateCanSaveProfile();
        }
    }

    public ObservableCollection<VersionViewModel> ForgeVersions { get; } = new();

    public bool Forge
    {
        get => _forge;
        set
        {
            this.RaiseAndSetIfChanged(ref _forge, value);
            OnForgeCheckboxChanged();
        }
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
        await UpdateVisibleForgeVersions();
    }
    
    private async void OnForgeCheckboxChanged()
    {
        await UpdateVisibleForgeVersions();
    }
    
    private void SetVersions(Versions versions)
    {
        _versions = versions;
        UpdateVisibleVersions();
    }
    
    private void UpdateCanSaveProfile()
    {
        var value = ProfileNameValidation.IsProfileNameValid(ProfileName, ProfileNameValidationAttribute.RestrictedName) &&
                    PlayerNameValidation.IsPlayerNameValid(PlayerName) &&
                    SelectedVersion != null && !string.IsNullOrEmpty(SelectedVersion.Id) &&
                    (!Forge || Forge && SelectedForgeVersion != null);
        
        CanSaveProfile.OnNext(value);
    }
    
    private void UpdateVisibleVersions()
    {
        Versions.Clear();
        
        if (_versions != null)
        {
            if (Alpha)
                AddVersions(_versions.Alpha);

            if (Beta)
                AddVersions(_versions.Beta);

            if (Snapshot)
                AddVersions(_versions.Snapshot);

            if (Release)
                AddVersions(_versions.Release);

            if (Latest)
            {
                if (_versions.Latest != null)
                    Versions.Add(new VersionViewModel(_versions.Latest));

                if (_versions.LatestSnapshot != null)
                    Versions.Add(new VersionViewModel(_versions.LatestSnapshot));
            }
        }
        
        if (SelectedVersion != null && !Versions.Contains(SelectedVersion))
            Versions.Add(SelectedVersion);

        void AddVersions(IReadOnlyList<Version> versions)
        {
            for (var i = 0; i < versions.Count; i++)
                Versions.Add(new VersionViewModel(versions[i]));
        }
    }

    private async Task UpdateVisibleForgeVersions()
    {
        ForgeVersions.Clear();
        SelectedForgeVersion = null;
        
        if (Forge && SelectedVersion != null)
        {
            var forgeVersions = await _versionsLoader.RequestForgeVersions(SelectedVersion.Id);

            for (var i = 0; i < forgeVersions.Versions.Count; i++)
            {
                var forgeVersion = forgeVersions.Versions[i];
                ForgeVersions.Add(new VersionViewModel(forgeVersion));
            }
            
            if (SelectedForgeVersion != null && !ForgeVersions.Contains(SelectedForgeVersion))
                ForgeVersions.Add(SelectedForgeVersion);
        }
    }
}