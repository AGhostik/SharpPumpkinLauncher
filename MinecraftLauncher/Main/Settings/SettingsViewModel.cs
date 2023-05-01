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
    private bool _useCustomResolution;
    private int _screenHeight;
    private int _screenWidth;

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

    public bool UseCustomResolution
    {
        get => _useCustomResolution;
        set
        {
            this.RaiseAndSetIfChanged(ref _useCustomResolution, value);
            UpdateCanSave();
        }
    }

    public int ScreenHeight
    {
        get => _screenHeight;
        set
        {
            this.RaiseAndSetIfChanged(ref _screenHeight, value);
            UpdateCanSave();
        }
    }

    public int ScreenWidth
    {
        get => _screenWidth;
        set
        {
            this.RaiseAndSetIfChanged(ref _screenWidth, value);
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

    public void SetUp(SettingsData settingsData)
    {
        Directory = settingsData.Directory;
        DefaultPlayerName = settingsData.DefaultPlayerName;
        LauncherVisibility = settingsData.LauncherVisibility;
        UseCustomResolution = settingsData.UseCustomResolution;
        ScreenWidth = settingsData.ScreenWidth;
        ScreenHeight = settingsData.ScreenHeight;
    }

    private void Save()
    {
        if (string.IsNullOrEmpty(Directory))
            return;
        
        var settings = new SettingsData(DefaultPlayerName, Directory, LauncherVisibility, UseCustomResolution,
            ScreenHeight, ScreenWidth);
        _saveAction.Invoke(settings);
    }

    private void Close()
    {
        _closeAction.Invoke();
    }

    private void UpdateCanSave()
    {
        var isValid = DirectoryValidation.IsDirectoryValid(Directory) &&
                      PlayerNameValidation.IsPlayerNameValid(DefaultPlayerName) &&
                      (!UseCustomResolution || ScreenWidth > 0 && ScreenHeight > 0);
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