using System;
using System.Collections.Generic;
using System.Text;

namespace SharpPumpkinLauncher.Main.JavaArguments;

public sealed class Arguments
{
    public Arguments()
    {
        EnabledArguments = Array.Empty<string>();
    }
    
    public Arguments(IReadOnlyList<string> enabledArguments)
    {
        EnabledArguments = enabledArguments;
    }

    /// <summary>
    /// Key - Value
    /// </summary>
    public IReadOnlyList<string> EnabledArguments { get; }

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (var i = 0; i < EnabledArguments.Count; i++)
        {
            sb.Append(EnabledArguments[i]);
            sb.Append(' ');
        }

        return sb.ToString();
    }
}