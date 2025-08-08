using System;
using System.IO;
using System.Reactive;
using System.Reactive.Subjects;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using ReactiveUI;
using SharpPumpkinLauncher.Main.JavaArguments;
using SharpPumpkinLauncher.Main.Validation;
using SimpleLogger;

namespace SharpPumpkinLauncher.Main.Settings;

public sealed class SettingsViewModel : ReactiveObject
{
    private readonly JavaArgumentsViewModel _javaArgumentsViewModel;
    private readonly SettingsManager _settingsManager;
    private readonly Action _saveAction;
    private readonly Action _closeAction;
    private readonly Action _showJavaEdit;

    private string? _directory;
    private string? _defaultPlayerName;
    private LauncherVisibility _launcherVisibility;
    private bool _useCustomResolution;
    private int _screenHeight;
    private int _screenWidth;
    
    private string? _previousDirectoryValue;
    private string? _previousDefaultPlayerNameValue;
    private LauncherVisibility _previousLauncherVisibilityValue;
    private bool _previousUseCustomResolutionValue;
    private int _previousScreenWidthValue;
    private int _previousScreenHeightValue;
    private string? _javaArguments;
    private Arguments _arguments;
    private bool _useJavaArguments;

    public SettingsViewModel(Action saveAction, Action closeAction, Action showJavaEdit,
        JavaArgumentsViewModel javaArgumentsViewModel)
    {
        _arguments = new Arguments();
        _settingsManager = ServiceProvider.SettingsManager;
        _javaArgumentsViewModel = javaArgumentsViewModel;
        _javaArgumentsViewModel.Setup(JavaArgumentsSaved);
        _javaArgumentsViewModel.Setup(_arguments);
        
        _saveAction = saveAction;
        _closeAction = closeAction;
        _showJavaEdit = showJavaEdit;
        SaveCommand = ReactiveCommand.Create(Save, CanSave);
        CloseCommand = ReactiveCommand.Create(Close);
        PickFolderCommand = ReactiveCommand.Create(PickFolder);
        EditJavaArgsCommand = ReactiveCommand.Create(EditJavaArgs);
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

    public bool UseJavaArguments
    {
        get => _useJavaArguments;
        set => this.RaiseAndSetIfChanged(ref _useJavaArguments, value);
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

    public string? JavaArguments
    {
        get => _javaArguments;
        set => this.RaiseAndSetIfChanged(ref _javaArguments, value);
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
    public ReactiveCommand<Unit, Unit> EditJavaArgsCommand { get; }

    public void SetUp(SettingsData settingsData)
    {
        Directory = settingsData.Directory;
        DefaultPlayerName = settingsData.DefaultPlayerName;
        LauncherVisibility = settingsData.LauncherVisibility;
        UseCustomResolution = settingsData.UseCustomResolution;
        ScreenWidth = settingsData.ScreenWidth;
        ScreenHeight = settingsData.ScreenHeight;
        UseJavaArguments = settingsData.UseJavaArguments;
        
        _arguments = new Arguments(settingsData.Arguments);
        JavaArguments = _arguments.ToString();
        _javaArgumentsViewModel.Setup(_arguments);
        
        SavePreviousValues();
    }

    private void Save()
    {
        if (string.IsNullOrEmpty(Directory))
            return;
        
        SavePreviousValues();

        var settings = new SettingsData(DefaultPlayerName, Directory, LauncherVisibility, UseCustomResolution,
            ScreenHeight, ScreenWidth, UseJavaArguments, _arguments.EnabledArguments);
        
        _settingsManager.SaveSettingsData(settings);
        
        _saveAction.Invoke();
    }

    private void Close()
    {
        _closeAction.Invoke();
        RestorePreviousValues();
    }

    private void SavePreviousValues()
    {
        _previousDirectoryValue = Directory;
        _previousDefaultPlayerNameValue = DefaultPlayerName;
        _previousLauncherVisibilityValue = LauncherVisibility;
        _previousUseCustomResolutionValue = UseCustomResolution;
        _previousScreenWidthValue = ScreenWidth;
        _previousScreenHeightValue = ScreenHeight;
    }

    private void RestorePreviousValues()
    {
        Directory = _previousDirectoryValue;
        DefaultPlayerName = _previousDefaultPlayerNameValue;
        LauncherVisibility = _previousLauncherVisibilityValue;
        UseCustomResolution = _previousUseCustomResolutionValue;
        ScreenWidth = _previousScreenWidthValue;
        ScreenHeight = _previousScreenHeightValue;
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
        try
        {
            if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
            {
                Logger.Log("Current application is not IClassicDesktopStyleApplicationLifetime");
                return;
            }

            if (desktop.MainWindow is null)
            {
                Logger.Log("MainWindow is null");
                return;
            }

            var storageProvider = desktop.MainWindow.StorageProvider;
            var startLocation = await storageProvider.TryGetFolderFromPathAsync(new Uri(AppContext.BaseDirectory));

            var options = new FolderPickerOpenOptions()
            {
                AllowMultiple = false,
                SuggestedStartLocation = startLocation,
            };

            var result = await storageProvider.OpenFolderPickerAsync(options);
            
            if (result.Count == 0)
                return;
        
            Directory = Path.GetRelativePath(AppContext.BaseDirectory, result[0].Path.AbsolutePath) + '\\';
        }
        catch (Exception e)
        {
            Logger.Log(e);
        }
    }

    private void EditJavaArgs()
    {
        _showJavaEdit.Invoke();
    }
    
    private void JavaArgumentsSaved(Arguments arguments)
    {
        if (!UseJavaArguments)
            return;
        
        _arguments = arguments;
        JavaArguments = arguments.ToString();
    }
}