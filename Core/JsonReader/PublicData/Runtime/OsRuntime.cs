namespace JsonReader.PublicData.Runtime;

public sealed class OsRuntime
{
    public OsRuntime(Runtime? javaRuntimeAlpha, Runtime? javaRuntimeBeta, Runtime? javaRuntimeGamma, 
        Runtime? javaRuntimeDelta, Runtime? jreLegacy, Runtime? minecraftJavaExe)
    {
        JavaRuntimeAlpha = javaRuntimeAlpha;
        JavaRuntimeBeta = javaRuntimeBeta;
        JavaRuntimeGamma = javaRuntimeGamma;
        JavaRuntimeDelta = javaRuntimeDelta;
        
        JreLegacy = jreLegacy;
        MinecraftJavaExe = minecraftJavaExe;
    }

    public Runtime? JavaRuntimeAlpha { get; }
    public Runtime? JavaRuntimeBeta { get; }
    public Runtime? JavaRuntimeGamma { get; }
    public Runtime? JavaRuntimeDelta { get; }
    public Runtime? JreLegacy { get; }
    public Runtime? MinecraftJavaExe { get; }
}