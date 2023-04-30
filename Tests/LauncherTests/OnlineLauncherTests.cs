using System.Diagnostics;
using Launcher;
using Launcher.PublicData;

namespace LauncherTests;

public class OnlineLauncherTests
{
    private string _temporaryGameFolder = string.Empty;
    private Versions? _availableVersions;

    [SetUp]
    public async Task Setup()
    {
        _temporaryGameFolder = Path.GetTempPath() + "minecraftTests\\";
        
        var launcher = new OnlineLauncher();
        _availableVersions = await launcher.GetAvailableVersions(_temporaryGameFolder, default);
    }

    [Test]
    public async Task GetAvailableVersions_True_IsNotEqualEmpty()
    {
        var launcher = new OnlineLauncher();
        var availableVersions = await launcher.GetAvailableVersions(_temporaryGameFolder, default);
        Assert.That(Versions.Empty, Is.Not.EqualTo(availableVersions));
    }

    [Test]
    public async Task LaunchMinecraft_True_LatestRelease()
    {
        if (_availableVersions?.Latest == null)
            return;

        var launcher = new OnlineLauncher();
        var launchData = new LaunchData("Steve", _temporaryGameFolder, _availableVersions.Latest);
        var errorCode = await launcher.LaunchMinecraft(launchData, default, startedAction: FindAndCloseMinecraft);

        Assert.That(errorCode, Is.EqualTo(ErrorCode.NoError));
    }
    
    [Test]
    public async Task LaunchMinecraft_True_AllRelease()
    {
        if (_availableVersions == null)
            return;

        var launcher = new OnlineLauncher();
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