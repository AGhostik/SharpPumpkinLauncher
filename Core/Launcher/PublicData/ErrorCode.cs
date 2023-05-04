namespace Launcher.PublicData;

public enum ErrorCode
{
    NoError,
    VersionId,
    GetVersionData,
    GetForgeVersionData,
    Url,
    MinecraftData,
    AssetsData,
    RuntimeData,
    ForgeData,
    UnknownRuntimeVersion,
    RuntimeDataNotFound,
    CreateDirectory,
    CreateFile,
    ReadFile,
    DeleteFileOrDirectory,
    Check,
    Download,
    Install,
    JavaNotInstalled,
    ExtractArchive,
    LaunchArgument,
    StartProcess,
    GameAborted,
}