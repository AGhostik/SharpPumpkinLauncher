using JsonReader.PublicData.Forge;
using JsonReader.PublicData.Game;
using Launcher.Tools;

namespace Launcher.Forge;

internal static class ForgeArgumentsBuilder
{
    public static string GetForgeLaunchArguments(ForgeInfo forgeInfo, MinecraftData minecraftVersionData,
        LaunchArgumentsData launchArgumentsData)
    {
        if (forgeInfo.ForgeArguments != null)
        {
            return GetForgeArguments(forgeInfo.MainClass, forgeInfo.ForgeArguments, minecraftVersionData,
                launchArgumentsData);
        }

        return GetLegacyForgeArguments(forgeInfo, minecraftVersionData, launchArgumentsData);
    }
    
    private static string GetForgeArguments(string mainClass, ForgeArguments forgeArguments, 
        MinecraftData minecraftVersionData, LaunchArgumentsData launchArgumentsData)
    {
        var jvmArguments = ArgumentBuilder.BuildArguments(minecraftVersionData.Arguments.Jvm);
        var gameArguments = 
            ArgumentBuilder.BuildArguments(minecraftVersionData.Arguments.Game, launchArgumentsData.Features);
            
        var forgeJvmArguments = ArgumentBuilder.BuildArguments(forgeArguments.Jvm, jvmArguments);
        var forgeGameArguments = ArgumentBuilder.BuildArguments(forgeArguments.Game, gameArguments);
            
        var jvmFilledArguments = ArgumentBuilder.FillJmvParameters(forgeJvmArguments, launchArgumentsData,
            minecraftVersionData.MinimumLauncherVersion);

        var gameFilledArguments = ArgumentBuilder.FillGameArguments(forgeGameArguments, launchArgumentsData);
        
        var additionalArguments = ArgumentBuilder.BuildArguments(launchArgumentsData.AdditionalArguments);
            
        return $"{additionalArguments}{jvmFilledArguments} {mainClass} {gameFilledArguments}";
    }

    private static string GetLegacyForgeArguments(ForgeInfo forgeInfo, MinecraftData minecraftVersionData,
        LaunchArgumentsData launchArgumentsData)
    {
        string? jvmFilledArguments;
        if (minecraftVersionData.Arguments.LegacyArguments == null)
        {
            var jvmArguments = ArgumentBuilder.BuildArguments(minecraftVersionData.Arguments.Jvm);
            jvmFilledArguments = ArgumentBuilder.FillJmvParameters(jvmArguments, launchArgumentsData,
                minecraftVersionData.MinimumLauncherVersion);
        }
        else
        {
            jvmFilledArguments = ArgumentBuilder.FillJmvParameters(minecraftVersionData.Arguments.LegacyArguments.Jvm,
                launchArgumentsData, minecraftVersionData.MinimumLauncherVersion);
        }

        var gameFilledArguments = ArgumentBuilder.FillGameArguments(forgeInfo.LegacyGameArguments, launchArgumentsData);
        
        return $"{jvmFilledArguments} {forgeInfo.MainClass} {gameFilledArguments}";
    }
}