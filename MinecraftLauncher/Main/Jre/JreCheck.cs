using System;
using System.Diagnostics;
using SimpleLogger;

namespace MinecraftLauncher.Main.Jre;

public static class JreCheck
{
    public static bool IsJavaInstalled()
    {
        try
        {
            using var process = new Process();
            process.StartInfo.UseShellExecute = true; 
            process.StartInfo.FileName = "java";
            process.StartInfo.Arguments = "-version";
            return process.Start();
        }
        catch (Exception e)
        {
            Logger.Log(e);
            return false;
        }
    }
}