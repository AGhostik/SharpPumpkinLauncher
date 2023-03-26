using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Subjects;
using ReactiveUI;

namespace MinecraftLauncher.Main.Profile;

public sealed class ProfileViewModel : ReactiveObject
{
    private string? _profileName;
    private string? _playerName;
    private string? _directory;
    private VersionViewModel? _selectedVersion;
    private static Action<ProfileViewModel>? _save;
    private static Action? _cancel;

    private ProfileViewModel()
    {
        SaveProfileCommand = ReactiveCommand.Create(SaveProfile, CanSaveProfile);
        CloseProfileControlCommand = ReactiveCommand.Create(CloseProfileControl);
    }

    public static ProfileViewModel CreateNew(Action<ProfileViewModel> save, Action cancel)
    {
        _save = save;
        _cancel = cancel;
        return new ProfileViewModel();
    }

    public static ProfileViewModel Edit(ProfileViewModel profileViewModel, Action<ProfileViewModel> save, Action cancel)
    {
        _save = save;
        _cancel = cancel;
        return profileViewModel;
    }
    
    public string? ProfileName
    {
        get => _profileName;
        set
        {
            this.RaiseAndSetIfChanged(ref _profileName, value);
            UpdateCanSaveProfile();
        }
    }

    public string? PlayerName
    {
        get => _playerName;
        set
        {
            this.RaiseAndSetIfChanged(ref _playerName, value);
            UpdateCanSaveProfile();
        }
    }

    public string? Directory
    {
        get => _directory;
        set
        {
            this.RaiseAndSetIfChanged(ref _directory, value);
            UpdateCanSaveProfile();
        }
    }

    public VersionViewModel? SelectedVersion
    {
        get => _selectedVersion;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedVersion, value);
            UpdateCanSaveProfile();
        }
    }

    public ObservableCollection<VersionViewModel> Versions { get; } = new();
    
    public bool Custom { get; set; }
    public bool Release { get; set; }
    public bool Snapshot { get; set; }
    public bool Beta { get; set; }
    public bool Alpha { get; set; }
    
    public ReactiveCommand<Unit, Unit> SaveProfileCommand { get; }
    private Subject<bool> CanSaveProfile { get; } = new();

    private void UpdateCanSaveProfile()
    {
        var value = !string.IsNullOrEmpty(ProfileName) &&
                    !string.IsNullOrEmpty(PlayerName) &&
                    !string.IsNullOrEmpty(Directory);
        
        CanSaveProfile.OnNext(value);
    }

    private void SaveProfile()
    {
        _save?.Invoke(this);
    }
    
    public ReactiveCommand<Unit, Unit> CloseProfileControlCommand { get; }

    private void CloseProfileControl()
    {
        _cancel?.Invoke();
    }
}