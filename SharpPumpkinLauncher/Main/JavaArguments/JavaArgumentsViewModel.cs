using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using Launcher.PublicData;
using ReactiveUI;
using SharpPumpkinLauncher.Main.JavaArguments.ArgumentItems;
using SharpPumpkinLauncher.Properties;

namespace SharpPumpkinLauncher.Main.JavaArguments;

public class JavaArgumentsViewModel : ReactiveObject
{
    private readonly Action _close;
    private Action<Arguments>? _save;
    
    public JavaArgumentsViewModel(Action close)
    {
        _close = close;
        SaveCommand = ReactiveCommand.Create(Save);
        CloseCommand = ReactiveCommand.Create(Close);
    }

    public ArgumentViewModel[] Arguments { get; } = 
    {
        new(JavaArgumentLocalization.NameUseG1Gc, 
            JavaArgumentLocalization.DescriptionUseG1Gc, 
            WellKnownAdditionalArguments.UseG1Gc),
        
        new(JavaArgumentLocalization.NameAggressiveHeap, 
            JavaArgumentLocalization.DescriptionAggressiveHeap, 
            WellKnownAdditionalArguments.AggressiveHeap),
        
        new(JavaArgumentLocalization.NameAlwaysPreTouch, 
            JavaArgumentLocalization.DescriptionAlwaysPreTouch, 
            WellKnownAdditionalArguments.AlwaysPreTouch),
        
        new(JavaArgumentLocalization.NameUseCompressedOops, 
            JavaArgumentLocalization.DescriptionUseCompressedOops, 
            WellKnownAdditionalArguments.UseCompressedOops),
        
        new(JavaArgumentLocalization.NameDisableExplicitGc, 
            JavaArgumentLocalization.DescriptionDisableExplicitGc, 
            WellKnownAdditionalArguments.DisableExplicitGc),
        
        new(JavaArgumentLocalization.NameG1UseAdaptiveIhop, 
            JavaArgumentLocalization.DescriptionG1UseAdaptiveIhop, 
            WellKnownAdditionalArguments.G1UseAdaptiveIhop),
        
        new(JavaArgumentLocalization.NamePerfDisableSharedMem, 
            JavaArgumentLocalization.DescriptionPerfDisableSharedMem, 
            WellKnownAdditionalArguments.PerfDisableSharedMem),
        
        new(JavaArgumentLocalization.NameParallelRefProcEnabled, 
            JavaArgumentLocalization.DescriptionParallelRefProcEnabled, 
            WellKnownAdditionalArguments.ParallelRefProcEnabled),
        
        new(JavaArgumentLocalization.NameUseStringDeduplication, 
            JavaArgumentLocalization.DescriptionUseStringDeduplication, 
            WellKnownAdditionalArguments.UseStringDeduplication),
        
        new(JavaArgumentLocalization.NameUnlockExperimentalVmOptions, 
            JavaArgumentLocalization.DescriptionUnlockExperimentalVmOptions, 
            WellKnownAdditionalArguments.UnlockExperimentalVmOptions),
    };

    public ArgumentWithMemoryValueViewModel[] ArgumentsWithMemoryValue { get; } = 
    {
        new(JavaArgumentLocalization.NameXmn, 
            JavaArgumentLocalization.DescriptionXmn, 
            WellKnownAdditionalArguments.Xmn,
            new []{ 'M', 'G' }),
        
        new(JavaArgumentLocalization.NameXmx, 
            JavaArgumentLocalization.DescriptionXmx, 
            WellKnownAdditionalArguments.Xmx,
            new []{ 'M', 'G' }),
        
        new(JavaArgumentLocalization.NameXms, 
            JavaArgumentLocalization.DescriptionXms, 
            WellKnownAdditionalArguments.Xms,
            new []{ 'M', 'G' }),
        
        new(JavaArgumentLocalization.NameG1HeapRegionSize, 
            JavaArgumentLocalization.DescriptionG1HeapRegionSize,
            WellKnownAdditionalArguments.G1HeapRegionSize,
            new []{ 'M' },
            useEqualsCharacter: true),
    };

    public ArgumentWithValueViewModel[] ArgumentsWithValue { get; } =
    {
        new(JavaArgumentLocalization.NameConcGcThreads, 
            JavaArgumentLocalization.DescriptionConcGcThreads, 
            WellKnownAdditionalArguments.ConcGcThreads),
        
        new(JavaArgumentLocalization.NameMaxGcPauseMillis, 
            JavaArgumentLocalization.DescriptionMaxGcPauseMillis, 
            WellKnownAdditionalArguments.MaxGcPauseMillis),
        
        new(JavaArgumentLocalization.NameG1NewSizePercent, 
            JavaArgumentLocalization.DescriptionG1NewSizePercent, 
            WellKnownAdditionalArguments.G1NewSizePercent),
        
        new(JavaArgumentLocalization.NameG1ReservePercent, 
            JavaArgumentLocalization.DescriptionG1ReservePercent, 
            WellKnownAdditionalArguments.G1ReservePercent),
        
        new(JavaArgumentLocalization.NameParallelGcThreads, 
            JavaArgumentLocalization.DescriptionParallelGcThreads, 
            WellKnownAdditionalArguments.ParallelGcThreads),
        
        new(JavaArgumentLocalization.NameG1MaxNewSizePercent, 
            JavaArgumentLocalization.DescriptionG1MaxNewSizePercent, 
            WellKnownAdditionalArguments.G1MaxNewSizePercent),
        
        new(JavaArgumentLocalization.NameG1MixedGcCountTarget, 
            JavaArgumentLocalization.DescriptionG1MixedGcCountTarget, 
            WellKnownAdditionalArguments.G1MixedGcCountTarget),
        
        new(JavaArgumentLocalization.NameG1MixedGcLiveThresholdPercent, 
            JavaArgumentLocalization.DescriptionG1MixedGcLiveThresholdPercent, 
            WellKnownAdditionalArguments.G1MixedGcLiveThresholdPercent),
        
        new(JavaArgumentLocalization.NameInitiatingHeapOccupancyPercent, 
            JavaArgumentLocalization.DescriptionInitiatingHeapOccupancyPercent, 
            WellKnownAdditionalArguments.InitiatingHeapOccupancyPercent),
    };

    public ReactiveCommand<Unit, Unit> SaveCommand { get; }
    public ReactiveCommand<Unit, Unit> CloseCommand { get; }

    public void Setup(Action<Arguments> save)
    {
        _save = save;
    }
    
    public void Setup(Arguments arguments)
    {
        for (var i = 0; i < arguments.EnabledArguments.Count; i++)
        {
            var value = arguments.EnabledArguments[i];

            foreach (var argument in EnumAllArguments())
            {
                if (value.Contains(argument.Argument))
                    argument.Setup(value);
            }
        }
    }

    private void Close()
    {
        foreach (var argument in EnumAllArguments())
            argument.Reset();
        
        _close.Invoke();
    }

    private void Save()
    {
        var enabledArguments = 
            EnumAllArguments().Where(arg => arg.IsEnabled).Select(arg => arg.GetBuildedArgument).ToList();
        
        _save?.Invoke(new Arguments(enabledArguments));
        _close.Invoke();
    }

    private IEnumerable<IArgument> EnumAllArguments()
    {
        for (var i = 0; i < Arguments.Length; i++)
            yield return Arguments[i];

        for (var i = 0; i < ArgumentsWithMemoryValue.Length; i++)
            yield return ArgumentsWithMemoryValue[i];
        
        for (var i = 0; i < ArgumentsWithValue.Length; i++)
            yield return ArgumentsWithValue[i];
    }
}