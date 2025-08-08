using System.Diagnostics;
using Launcher;
using Launcher.PublicData;

namespace LauncherTests;

public class MinecraftLauncherTests
{
    private readonly LaunchFeaturesData _features = new(false, 0, 0);
    private string _temporaryGameFolder = string.Empty;
    private Versions? _availableVersions;

    private LaunchData GetLaunchData(string versionId)
    {
        return new LaunchData(
            "Steve", 
            versionId, 
            null, 
            _temporaryGameFolder, 
            _features,
            []);
    }

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
        var launchData = GetLaunchData(_availableVersions.Latest.Id);
        var errorCode = await launcher.LaunchMinecraft(launchData, startedAction: FindAndCloseMinecraft);
    
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
            var launchData = GetLaunchData(version.Id);
            var errorCode = await launcher.LaunchMinecraft(launchData, startedAction: FindAndCloseMinecraft);
            if (errorCode != ErrorCode.NoError)
                Assert.Fail($"Failed to run '{version.Id}', errorCode: '{errorCode}'");
        }
        
        Assert.Pass($"Success to run {_availableVersions.Release.Count} versions");
    }

    private static async void FindAndCloseMinecraft()
    {
        const int attemptCount = 15;

        var currentAttempt = 0;
        Process? minecraftProcess = null;

        do
        {
            await Task.Delay(1000);
            var processes = Process.GetProcessesByName("java");
            foreach (var process in processes)
            {
                if (process.MainWindowTitle.Contains("Minecraft"))
                    minecraftProcess = process;
            }

            currentAttempt++;
        } while (minecraftProcess == null && currentAttempt < attemptCount);

        if (minecraftProcess != null)
        {
            do
            {
                minecraftProcess.Refresh();
                if (minecraftProcess.Responding)
                    minecraftProcess.CloseMainWindow();
                else
                    await Task.Delay(1000);
            } while (!minecraftProcess.Responding);
        }
    }
}