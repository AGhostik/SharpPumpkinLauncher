using JsonReader.PublicData.Forge;

namespace Launcher.Data;

public sealed class ForgeProfileInstallInfo
{
    public ForgeProfileInstallInfo(string javaFile, string minecraftJar, string librariesPath,
        ForgeInstall forgeInstall)
    {
        JavaFile = javaFile;
        MinecraftJar = minecraftJar;
        LibrariesPath = librariesPath;
        ForgeInstall = forgeInstall;
    }

    public string JavaFile { get; }
    public string MinecraftJar { get; }
    public string LibrariesPath { get; }
    public ForgeInstall ForgeInstall { get; }
}