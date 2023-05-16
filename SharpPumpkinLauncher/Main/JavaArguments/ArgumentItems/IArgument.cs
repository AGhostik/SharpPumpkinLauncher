namespace SharpPumpkinLauncher.Main.JavaArguments.ArgumentItems;

public interface IArgument
{
    string Argument { get; }
    string GetBuildedArgument { get; }
    bool IsEnabled { get; }
    void Setup(string value);
    void Reset();
}