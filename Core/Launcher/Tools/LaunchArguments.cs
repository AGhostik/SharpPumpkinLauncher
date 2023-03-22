using System.Security.Cryptography;
using System.Text;

namespace Launcher.Tools;

public sealed class LaunchArguments
{
    private readonly StringBuilder _afterLibraries;
    private readonly StringBuilder _beforeLibraries;
    private readonly StringBuilder _libraries;

    public LaunchArguments()
    {
        _beforeLibraries = new StringBuilder();
        _libraries = new StringBuilder();
        _afterLibraries = new StringBuilder();
    }

    public string Get()
    {
        var output = _beforeLibraries.Append(_libraries).Append(_afterLibraries).ToString();
        _beforeLibraries.Clear();
        _libraries.Clear();
        _afterLibraries.Clear();
        return output;
    }

    public void AddLibrary(string fileName)
    {
        _libraries.Append($"{fileName};");
    }

    public void Create()
    {
        // _beforeLibraries.Append($"{profile.JvmArgs} ");
        // _beforeLibraries.Append($"-Djava.library.path=\"{profile.GameDirectory}\\versions\\{minecraftVersion.Id}\\natives\" -cp \"");
        //
        // _afterLibraries.Append($"{profile.GameDirectory}\\versions\\{minecraftVersion.Id}\\{minecraftVersion.Id}.jar\" ");
        // _afterLibraries.Append($"{minecraftVersion.MainClass} ");
        // _afterLibraries.Append(GetMinecraftArguments(profile, minecraftVersion));
    }

    // private static string GetMinecraftArguments(Profile profile, MinecraftVersionData? minecraftVersion)
    // {
    //     if (minecraftVersion?.MinecraftArguments == null)
    //     {
    //         return
    //             $"--username {profile.Nickname} " +
    //             $"--version {minecraftVersion.Id} " +
    //             $"--gameDir {profile.GameDirectory} " +
    //             $"--assetsDir {profile.GameDirectory + "assets"} " +
    //             $"--assetIndex {minecraftVersion.Assets} " +
    //             $"--uuid {GetUuid(profile.Nickname)} " +
    //             $"--accessToken null " +
    //             //$"--clientId null " +
    //             // $"--xuid {auth_xuid} " +
    //             $"--userType mojang "; //+
    //         //$"--versionType {version_type}";
    //     }
    //     
    //     var args = minecraftVersion.MinecraftArguments;
    //     args = args.Replace("${auth_player_name}", profile.Nickname);
    //     args = args.Replace("${version_name}", minecraftVersion.Id);
    //     args = args.Replace("${game_directory}", profile.GameDirectory);
    //     args = args.Replace("${assets_root}", profile.GameDirectory + "assets");
    //     args = args.Replace("${assets_index_name}", minecraftVersion.Assets);
    //     args = args.Replace("${auth_uuid}", GetUuid(profile.Nickname));
    //     args = args.Replace("${auth_access_token}", "null");
    //     args = args.Replace("${user_properties}", "{}");
    //     args = args.Replace("${user_type}", "mojang");
    //     args = args.Replace("${auth_session}", "null");
    //     args = args.Replace("${game_assets}", profile.GameDirectory + "\\assets\\virtual\\legacy");
    //
    //     return args;
    // }

    private static string GetUuid(string input)
    {
        var data = MD5.HashData(Encoding.Default.GetBytes(input));
        var guid = new Guid(data);
        return guid.ToString();
    }
}