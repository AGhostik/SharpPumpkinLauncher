using ReactiveUI;
using SimpleLogger;

namespace SharpPumpkinLauncher.Main.JavaArguments.ArgumentItems;

public class ArgumentWithValueViewModel : ReactiveObject, IArgument
{
    private bool _previousIsEnabled;
    private int _previousValue;
    
    private bool _isEnabled;
    private int _value;

    public ArgumentWithValueViewModel(string name, string description, string argument)
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
    
    public int Value
    {
        get => _value;
        set => this.RaiseAndSetIfChanged(ref _value, value);
    }

    public string Name { get; }
    public string Description { get; }
    public string Argument { get; }
    
    public string GetBuildedArgument => $"{Argument}={Value}";

    public void Setup(string buildedValue)
    {
        if (!buildedValue.Contains(Argument))
        {
            Logger.Log($"Error on parsing Argument ({Argument}) from value '{buildedValue}'");
            return;
        }

        _previousIsEnabled = true;
        IsEnabled = true;

        var value = buildedValue.Replace(Argument, string.Empty);
        if (int.TryParse(value, out var parsedValue))
        {
            Value = parsedValue;
            _previousValue = parsedValue;
        }
        else
        {
            Logger.Log($"Cant parse Value from '{value}'");
        }
    }
    
    public void Reset()
    {
        IsEnabled = _previousIsEnabled;
        Value = _previousValue;
    }
}