using System;
using System.Reactive;
using System.Reflection;
using ReactiveUI;

namespace MinecraftLauncher.Main.About;

public class AboutViewModel
{
    private readonly Action _close;

    public AboutViewModel(Action close)
    {
        _close = close;
        CloseCommand = ReactiveCommand.Create(Close);

        var version = Assembly.GetExecutingAssembly().GetName().Version;
        if (version != null)
            Version = $"v{version.Major}.{version.Minor}.{version.Build}";
    }
    
    public string? Version { get; }

    public ReactiveCommand<Unit, Unit> CloseCommand { get; }
    
    private void Close()
    {
        _close.Invoke();
    }
}