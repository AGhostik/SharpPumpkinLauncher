namespace Launcher.PublicData;

public enum ErrorCode
{
    NoError,
    VersionId,
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
    ExtractArchive,
    LaunchArgument,
    StartProcess,
    GameAborted,
    Connection,
}