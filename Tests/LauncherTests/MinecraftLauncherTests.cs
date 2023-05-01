using System.Diagnostics;
using Launcher;
using Launcher.PublicData;

namespace LauncherTests;

public class MinecraftLauncherTests
{
    private string _temporaryGameFolder = string.Empty;
    private Versions? _availableVersions;

    [SetUp]
    public async Task Setup()
    {
     _temporaryGameFolder = Path.GetTempPath() + "minecraftTests\\";
     
     var launcher = new MinecraftLauncher();
     _availableVersions = await launcher.GetAvailableVersions(_temporaryGameFolder);
    }
    
    [Test]
    public async Task GetAvailableVersions_IsNotEmpty()
    {
        var launcher = new MinecraftLauncher();
        var availableVersions = await launcher.GetAvailableVersions(_temporaryGameFolder);
        Assert.That(Versions.Empty, Is.Not.EqualTo(availableVersions));
    }
    
    [Test]
    public async Task LaunchLatestReleaseMinecraft_Success()
    {
        if (_availableVersions?.Latest == null)
            return;
    
        var launcher = new MinecraftLauncher();
        var launchData = new LaunchData("Steve", _temporaryGameFolder, _availableVersions.Latest);
        var errorCode = await launcher.LaunchMinecraft(launchData, default, startedAction: FindAndCloseMinecraft);
    
        Assert.That(errorCode, Is.EqualTo(ErrorCode.NoError));
    }
    
    [Test]
    public async Task LaunchAllReleaseMinecraft_Success()
    {
        if (_availableVersions == null)
            return;
    
        var launcher = new MinecraftLauncher();
        foreach (var version in _availableVersions.Release)
        {
            var launchData = new LaunchData("Steve", _temporaryGameFolder, version);
            var errorCode = await launcher.LaunchMinecraft(launchData, default, startedAction: FindAndCloseMinecraft);
            if (errorCode != ErrorCode.NoError)
                Assert.Fail($"Failed to run '{version.Id}', errorCode: '{errorCode}'");
        }
        
        Assert.Pass($"Success to run {_availableVersions.Release.Count} versions");
    }

    private static async void FindAndCloseMinecraft()
    {
        const int attemptCount = 15;

        var currentAttempt = 0;
        Process? minecraftProces = null;

        do
        {
            await Task.Delay(1000);
            var processes = Process.GetProcessesByName("java");
            foreach (var process in processes)
            {
                if (process.MainWindowTitle.Contains("Minecraft"))
                    minecraftProces = process;
            }

            currentAttempt++;
        } while (minecraftProces == null && currentAttempt < attemptCount);

        if (minecraftProces != null)
        {
            do
            {
                minecraftProces.Refresh();
                if (minecraftProces.Responding)
                    minecraftProces.CloseMainWindow();
                else
                    await Task.Delay(1000);
            } while (!minecraftProces.Responding);
        }
    }
}