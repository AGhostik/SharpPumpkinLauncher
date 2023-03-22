using System;
using System.Windows;
using CommunityToolkit.Mvvm.Messaging;
using Launcher;
using MCLauncher.LauncherWindow;
using MCLauncher.Messages;
using MCLauncher.SettingsWindow;
using Unity;
using Unity.Resolution;

namespace MCLauncher;

public partial class App
{
    private readonly UnityContainer _container;

    public App()
    {
        var container = new UnityContainer();
        _container = container;

        // container.RegisterType<IFileManager, FileManager>();
        // container.RegisterType<IProfileManager, ProfileManager>();
        // container.RegisterType<IJsonManager, JsonManager>();
        // container.RegisterType<ISettingsModel, SettingsModel>();

        container.RegisterSingleton<MinecraftLauncher>();
        container.RegisterSingleton<MinecraftData>();

        var launcherView = container.Resolve<LauncherView>();

        MainWindow = launcherView;
        ShutdownMode = ShutdownMode.OnMainWindowClose;

        launcherView.Show();

        WeakReferenceMessenger.Default.Register<ShowSettingsMessage>(this, Handler);
        
        Startup += OnStartup;
    }

    private async void OnStartup(object sender, StartupEventArgs e)
    {
        var minecraftData = _container.Resolve<MinecraftData>();
        await minecraftData.LoadAvailableVersions();
    }

    private void Handler(object _, ShowSettingsMessage message)
    {
        _container.RegisterInstance(message.Profile);
        var settingsWindow = _container.Resolve<SettingsView>();
        settingsWindow.Show();
    }
}