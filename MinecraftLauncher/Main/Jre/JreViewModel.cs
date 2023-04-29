using System;
using System.Diagnostics;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using ReactiveUI;
using SimpleLogger;

namespace MinecraftLauncher.Main.Jre;

public class JreViewModel : ReactiveObject
{
    private readonly Action? _closeAction;
    
    private string? _status;
    private Task? _delay;
    private CancellationTokenSource? _delayCancellation;

    public JreViewModel(Action? closeAction)
    {
        _closeAction = closeAction;
        CopyToClipboardCommand = ReactiveCommand.Create<string>(CopyToClipboard);
        OpenLinkCommand = ReactiveCommand.Create<string>(OpenLink);
        CloseCommand = ReactiveCommand.Create(Close);
    }

    public string? Status
    {
        get => _status;
        set => this.RaiseAndSetIfChanged(ref _status, value);
    }

    public ReactiveCommand<string, Unit> CopyToClipboardCommand { get; }
    
    public ReactiveCommand<string, Unit> OpenLinkCommand { get; }
    
    public ReactiveCommand<Unit, Unit> CloseCommand { get; }
    
    private async void CopyToClipboard(string param)
    {
        if (_delay != null && _delayCancellation != null)
        {
            _delayCancellation.Cancel();
            _delayCancellation.Dispose();
            _delay.Dispose();
        }

        if (Application.Current?.Clipboard == null)
        {
            Logger.Log("Application.Current.Clipboard is null");
            return;
        }
        
        await Application.Current.Clipboard.SetTextAsync(param);

        Status = Properties.Localization.CopiedToClipboard;
        _delayCancellation = new CancellationTokenSource();
        _delay = Task.Delay(2000, _delayCancellation.Token);
        await _delay.ContinueWith(task =>
        {
            if (!task.IsCanceled)
                Status = null;
        });
    }
    
    private void OpenLink(string param)
    {
        try
        {
            using var process = new Process();
            process.StartInfo.UseShellExecute = true; 
            process.StartInfo.FileName = param;
            process.Start();
        }
        catch (Exception e)
        {
            Logger.Log(e);
        }
    }
    
    private void Close()
    {
        _closeAction?.Invoke();
    }
}