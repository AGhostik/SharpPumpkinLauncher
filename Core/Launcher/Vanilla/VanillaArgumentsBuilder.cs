using JsonReader.PublicData.Game;
using Launcher.Tools;

namespace Launcher.Vanilla;

internal static class VanillaArgumentsBuilder
{
    public static string GetLaunchArguments(MinecraftData minecraftVersionData, LaunchArgumentsData launchArgumentsData)
    {
        if (minecraftVersionData.Arguments.LegacyArguments == null)
        {
            return GetArguments(launchArgumentsData, minecraftVersionData.Arguments, minecraftVersionData.MainClass,
                minecraftVersionData.MinimumLauncherVersion);
        }

        return GetLegacyArguments(launchArgumentsData, minecraftVersionData.Arguments.LegacyArguments,
            minecraftVersionData.MainClass, minecraftVersionData.MinimumLauncherVersion);
    }

    private static string GetArguments(LaunchArgumentsData launchArgumentsData, Arguments arguments,
        string mainClass, int launcherVersion)
    {
        var jvmArguments = ArgumentBuilder.BuildArguments(arguments.Jvm);
        var gameArguments = ArgumentBuilder.BuildArguments(arguments.Game, launchArgumentsData.Features);

        var jvmFilledArguments = ArgumentBuilder.FillJmvParameters(jvmArguments, launchArgumentsData, launcherVersion);
        var gameFilledArguments = ArgumentBuilder.FillGameArguments(gameArguments, launchArgumentsData);

        return $"{jvmFilledArguments} {mainClass} {gameFilledArguments}";
    }
    
    private static string GetLegacyArguments(LaunchArgumentsData launchArgumentsData, LegacyArguments legacyArguments,
        string mainClass, int launcherVersion)
    {
        var jvmFilledArguments = ArgumentBuilder.FillJmvParameters(legacyArguments.Jvm, launchArgumentsData, launcherVersion);
        var gameFilledArguments = ArgumentBuilder.FillGameArguments(legacyArguments.Game, launchArgumentsData);
        
        return $"{jvmFilledArguments} {mainClass} {gameFilledArguments}";
    }
}