using System;
using System.Reactive;
using System.Reactive.Subjects;
using MinecraftLauncher.Main.Validation;
using ReactiveUI;

namespace MinecraftLauncher.Main.Settings;

public sealed class SettingsViewModel : ReactiveObject
{
    private readonly Action<SettingsData> _saveAction;
    private readonly Action _closeAction;
    
    private string? _directory;
    private string? _defaultPlayerName;
    private LauncherVisibility _launcherVisibility;

    public SettingsViewModel(Action<SettingsData> saveAction, Action closeAction)
    {
        _saveAction = saveAction;
        _closeAction = closeAction;
        SaveCommand = ReactiveCommand.Create(Save, CanSave);
        CloseCommand = ReactiveCommand.Create(Close);
    }

    [PlayerNameValidation]
    public string? DefaultPlayerName
    {
        get => _defaultPlayerName;
        set
        {
            this.RaiseAndSetIfChanged(ref _defaultPlayerName, value);
            UpdateCanSave();
        }
    }

    [DirectoryValidation]
    public string? Directory
    {
        get => _directory;
        set
        {
            this.RaiseAndSetIfChanged(ref _directory, value);
            UpdateCanSave();
        }
    }

    public LauncherVisibility LauncherVisibility
    {
        get => _launcherVisibility;
        set => this.RaiseAndSetIfChanged(ref _launcherVisibility, value);
    }

    public LauncherVisibility[] Visibilities { get; } = {
        LauncherVisibility.KeepOpen,
        LauncherVisibility.Hide,
        LauncherVisibility.Close
    };

    public ReactiveCommand<Unit, Unit> SaveCommand { get; }
    private Subject<bool> CanSave { get; } = new();
    public ReactiveCommand<Unit, Unit> CloseCommand { get; }

    private void Save()
    {
        if (string.IsNullOrEmpty(Directory))
            return;
        
        var settings = new SettingsData(DefaultPlayerName, Directory, LauncherVisibility);
        _saveAction.Invoke(settings);
    }

    private void Close()
    {
        _closeAction.Invoke();
    }

    private void UpdateCanSave()
    {
        var isValid = DirectoryValidation.IsDirectoryValid(Directory) &&
                      PlayerNameValidation.IsPlayerNameValid(DefaultPlayerName);
        CanSave.OnNext(isValid);
    }
}