namespace Launcher.PublicData;

public enum ErrorCode
{
    NoError,
    VersionId,
    NeedVersionUrl,
    Url,
    MinecraftData,
    AssetsData,
    RuntimeData,
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
    Connection,
}