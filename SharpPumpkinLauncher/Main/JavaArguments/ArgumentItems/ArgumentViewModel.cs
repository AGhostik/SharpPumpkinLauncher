using ReactiveUI;

namespace SharpPumpkinLauncher.Main.JavaArguments.ArgumentItems;

public class ArgumentViewModel : ReactiveObject, IArgument
{
    private bool _previousIsEnabled;
    private bool _isEnabled;

    public ArgumentViewModel(string name, string description, string argument)
    {
        Name = name;
        Description = description;
        Argument = argument;

        _previousIsEnabled = false;
    }

    public bool IsEnabled
    {
        get => _isEnabled;
        set => this.RaiseAndSetIfChanged(ref _isEnabled, value);
    }
    
    public string Name { get; }
    public string Description { get; }
    public string Argument { get; }
    
    public string GetBuildedArgument => Argument;

    public void Setup(string _)
    {
        _previousIsEnabled = true;
        IsEnabled = true;
    }
    
    public void Reset()
    {
        IsEnabled = _previousIsEnabled;
    }
}