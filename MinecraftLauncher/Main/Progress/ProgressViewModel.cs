using ReactiveUI;

namespace MinecraftLauncher.Main.Progress;

public sealed class ProgressViewModel : ReactiveObject
{
    private double _progressValue;
    private string? _text;

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
}