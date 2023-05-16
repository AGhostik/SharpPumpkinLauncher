using System.Linq;
using System.Reactive;
using ReactiveUI;
using SimpleLogger;

namespace SharpPumpkinLauncher.Main.JavaArguments.ArgumentItems;

public class ArgumentWithMemoryValueViewModel : ReactiveObject, IArgument
{
    private readonly char[] _memoryTypes;
    private readonly bool _useEqualsCharacter;
    
    private bool _previousIsEnabled;
    private int _previousValue;
    private char _previousMemoryType;
    
    private bool _isEnabled;
    private int _value;
    private char _memoryType;

    public ArgumentWithMemoryValueViewModel(string name, string description, string argument, char[] memoryTypes,
        bool useEqualsCharacter = false)
    {
        _memoryTypes = memoryTypes;
        _useEqualsCharacter = useEqualsCharacter;
        if (memoryTypes.Length > 0)
            _memoryType = memoryTypes[0];

        _previousValue = 0;
        _previousIsEnabled = false;
        _previousMemoryType = _memoryType;
        
        Name = name;
        Description = description;
        Argument = argument;
        
        ChangeMemoryTypeCommand = ReactiveCommand.Create(ChangeMemoryType);
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
    
    public char MemoryType
    {
        get => _memoryType;
        set => this.RaiseAndSetIfChanged(ref _memoryType, value);
    }
    
    public ReactiveCommand<Unit, Unit> ChangeMemoryTypeCommand { get; }

    public string Name { get; }
    public string Description { get; }
    public string Argument { get; }
    public string GetBuildedArgument
    {
        get
        {
            if (_useEqualsCharacter)
                return $"{Argument}={Value}{MemoryType}";
            
            return $"{Argument}{Value}{MemoryType}";
        }
    }

    public void Reset()
    {
        Value = _previousValue;
        IsEnabled = _previousIsEnabled;
        MemoryType = _previousMemoryType;
    }
    
    public void Setup(string buildedValue)
    {
        if (!buildedValue.Contains(Argument))
        {
            Logger.Log($"Error on parsing Argument ({Argument}) from value '{buildedValue}'");
            return;
        }

        _previousIsEnabled = true;
        IsEnabled = true;

        var value = buildedValue.Replace(Argument, string.Empty).TrimEnd(_memoryTypes);
        if (int.TryParse(value, out var parsedValue))
        {
            Value = parsedValue;
            _previousValue = parsedValue;
        }
        else
        {
            Logger.Log($"Cant parse Value from '{value}'");
            return;
        }

        var lastChar = buildedValue[^1];
        if (_memoryTypes.Contains(lastChar))
        {
            MemoryType = lastChar;
            _previousMemoryType = lastChar;
        }
        else
        {
            Logger.Log($"Cant parse MemoryType from '{lastChar}'");
        }
    }
    
    private void ChangeMemoryType()
    {
        if (_memoryTypes.Length < 2)
            return;
            
        MemoryType = _memoryType == _memoryTypes[0] ? _memoryTypes[1] : _memoryTypes[0];
    }
}