using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.Messaging;
using MCLauncher.Model.Managers;
using MCLauncher.UI;
using MCLauncher.UI.Messages;

namespace MCLauncher.Model;

public class LauncherModel : ILauncherModel
{
    private readonly IFileManager _fileManager;
    private readonly IInstaller _installer;
    private readonly IProfileManager _profileManager;

    public LauncherModel(IFileManager fileManager, IProfileManager profileManager, IInstaller installer)
    {
        _installer = installer;
        _profileManager = profileManager;
        _fileManager = fileManager;
    }

    public async Task StartGame()
    {
        var currentProfile = _profileManager.GetLast();
        await _installer.Install(currentProfile);
        LaunchMinecraft(currentProfile);
    }

    public void SaveLastProfileName(string? name)
    {
        _profileManager.SaveLastProfileName(name);
    }

    public void OpenNewProfileWindow()
    {
        WeakReferenceMessenger.Default.Send(new ShowSettingsMessage(true));
    }

    public void OpenEditProfileWindow()
    {
        WeakReferenceMessenger.Default.Send(new ShowSettingsMessage(false));
    }

    public void DeleteProfile(string? name)
    {
        _profileManager.Delete(name);
        WeakReferenceMessenger.Default.Send(new ProfilesChangedMessage());
    }

    public List<string?> GetProfiles()
    {
        var names = new List<string?>();
        var profiles = _profileManager.GetProfiles();

        foreach (var profile in profiles)
            names.Add(profile.Name);

        return names;
    }

    public string? GetLastProfile()
    {
        var lastProfile = _profileManager.GetLastProfileName();
        return lastProfile;
    }

    private void LaunchMinecraft(Profile? profile)
    {
        if (profile == null)
            return;
        
        var exitedAction = MinecraftProcessExited;

        WeakReferenceMessenger.Default.Send(new StatusMessage(UIResource.LaunchGameStatus));

        switch (profile.LauncherVisibility)
        {
            case LauncherVisibility.Close:
                Application.Current?.MainWindow?.Close();
                break;
            case LauncherVisibility.Hide:
                Application.Current?.MainWindow?.Hide();
                exitedAction = () =>
                {
                    ShowMainWindow();
                    MinecraftProcessExited();
                };
                break;
            case LauncherVisibility.KeepOpen:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        _fileManager.StartProcess(profile.JavaFile, _installer.LaunchArgs, exitedAction);
    }

    private static void MinecraftProcessExited()
    {
        WeakReferenceMessenger.Default.Send(new StatusMessage(UIResource.GameExitedStatus));
    }

    private static void ShowMainWindow()
    {
        WeakReferenceMessenger.Default.Send(new MinecraftExitedMessage());
    }
}