using System;
using System.IO;
using System.Reactive;
using System.Reactive.Subjects;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using MinecraftLauncher.Main.Validation;
using ReactiveUI;
using SimpleLogger;

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
        PickFolderCommand = ReactiveCommand.Create(PickFolder);
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
    public ReactiveCommand<Unit, Unit> PickFolderCommand { get; }

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
    
    private async void PickFolder()
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
        {
            Logger.Log("Current application is not IClassicDesktopStyleApplicationLifetime");
            return;
        }
        
        var openFolder = new OpenFolderDialog
        {
            Directory = AppContext.BaseDirectory
        };
        
        var path = await openFolder.ShowAsync(desktop.MainWindow);
        if (string.IsNullOrEmpty(path))
            return;
        
        Directory = Path.GetRelativePath(AppContext.BaseDirectory, path) + '\\';
    }
}