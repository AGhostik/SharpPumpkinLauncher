using System;
using MinecraftLauncher.Properties;
using ReactiveUI;

namespace MinecraftLauncher.Main.Progress;

public sealed class ProgressViewModel : ReactiveObject
{
    private double _progressValue;
    private string? _text;
    private bool _isProgressVisible;

    public ProgressViewModel(MainWindowModel mainWindowModel)
    {
        mainWindowModel.UpdateProgressValues += OnUpdateProgressValues;
        Text = Localization.ProgressLoading;
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

    public bool IsProgressVisible
    {
        get => _isProgressVisible;
        set => this.RaiseAndSetIfChanged(ref _isProgressVisible, value);
    }

    private void OnUpdateProgressValues(ProgressLocalizationKeys localizationKey, float progress01)
    {
        Text = localizationKey switch
        {
            ProgressLocalizationKeys.Prepare => Localization.ProgressPrepare,
            ProgressLocalizationKeys.DownloadFiles => Localization.ProgressDownloadFiles,
            ProgressLocalizationKeys.StartGame => Localization.ProgressStartGame,
            ProgressLocalizationKeys.End => Localization.ProgressEnd,
            ProgressLocalizationKeys.InvalidProfile => Localization.ProgressInvalidProfile,
            ProgressLocalizationKeys.FailToStartGame => Localization.ProgressFailToStartGame,
            ProgressLocalizationKeys.Loading => Localization.ProgressLoading,
            ProgressLocalizationKeys.Ready => Localization.ProgressReady,
            ProgressLocalizationKeys.JavaNotFound => Localization.ProgressJavaNotFound,
            ProgressLocalizationKeys.FailToStartGameWithoutJava => Localization.ProgressFailToStartGameWithoutJava,
            _ => throw new ArgumentOutOfRangeException(nameof(localizationKey), localizationKey, null)
        };

        IsProgressVisible = progress01 > 0;
        ProgressValue = 100f * progress01;
    }
}