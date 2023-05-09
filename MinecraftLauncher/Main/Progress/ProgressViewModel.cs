using System;
using Launcher.PublicData;
using MinecraftLauncher.Properties;
using ReactiveUI;

namespace MinecraftLauncher.Main.Progress;

public sealed class ProgressViewModel : ReactiveObject
{
    private readonly VersionsLoader _versionsLoader;
    
    private double _progressValue;
    private string? _text;
    private string? _additionalText;
    private bool _isProgressVisible;

    public ProgressViewModel(MainWindowModel mainWindowModel)
    {
        _versionsLoader = ServiceProvider.VersionsLoader;
        
        Text = Localization.ProgressLoading;
        _versionsLoader.VersionsLoaded += OnVersionsLoaded;
        mainWindowModel.Progress += OnUpdateProgressValues;
    }

    public double ProgressValue
    {
        get => _progressValue;
        set => this.RaiseAndSetIfChanged(ref _progressValue, value);
    }

    public string? Text
    {
        get => _text;
        set => this.RaiseAndSetIfChanged(ref _text, value);
    }
    
    public string? AdditionalText
    {
        get => _additionalText;
        set => this.RaiseAndSetIfChanged(ref _additionalText, value);
    }

    public bool IsProgressVisible
    {
        get => _isProgressVisible;
        set => this.RaiseAndSetIfChanged(ref _isProgressVisible, value);
    }
    
    private void OnVersionsLoaded(Versions versions)
    {
        if (versions.IsEmpty)
            return;
        
        _versionsLoader.VersionsLoaded -= OnVersionsLoaded;
        OnUpdateProgressValues(ProgressLocalizationKeys.Ready, -1, null);
    }

    private void OnUpdateProgressValues(ProgressLocalizationKeys localizationKey, float progress01,
        string? additionalInfo)
    {
        Text = localizationKey switch
        {
            ProgressLocalizationKeys.Prepare => Localization.ProgressPrepare,
            ProgressLocalizationKeys.DownloadFiles => Localization.ProgressDownloadFiles,
            ProgressLocalizationKeys.InstallForge => Localization.ProgressInstallForge,
            ProgressLocalizationKeys.StartGame => Localization.ProgressStartGame,
            ProgressLocalizationKeys.End => Localization.ProgressEnd,
            ProgressLocalizationKeys.InvalidProfile => Localization.ProgressInvalidProfile,
            ProgressLocalizationKeys.FailToStartGame => Localization.ProgressFailToStartGame,
            ProgressLocalizationKeys.Loading => Localization.ProgressLoading,
            ProgressLocalizationKeys.Ready => Localization.ProgressReady,
            ProgressLocalizationKeys.Aborted => Localization.ProgressAbort,
            _ => throw new ArgumentOutOfRangeException(nameof(localizationKey), localizationKey, null)
        };

        AdditionalText = additionalInfo;

        IsProgressVisible = progress01 >= 0;
        ProgressValue = 100f * progress01;
    }
}