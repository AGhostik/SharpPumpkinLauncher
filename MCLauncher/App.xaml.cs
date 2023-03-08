using System.Windows;
using CommunityToolkit.Mvvm.Messaging;
using MCLauncher.Json;
using MCLauncher.LauncherWindow;
using MCLauncher.Messages;
using MCLauncher.SettingsWindow;
using MCLauncher.Tools;
using MCLauncher.Tools.Interfaces;
using Unity;
using Unity.Resolution;

namespace MCLauncher;

public partial class App
{
    public App()
    {
        var container = new UnityContainer();

        container.RegisterType<ILauncherModel, LauncherModel>();

        container.RegisterType<IInstaller, Installer>();
        container.RegisterType<IFileManager, FileManager>();
        container.RegisterType<IProfileManager, ProfileManager>();
        container.RegisterType<IJsonManager, JsonManager>();
        container.RegisterType<ISettingsModel, SettingsModel>();

        var launcherView = container.Resolve<LauncherView>();

        MainWindow = launcherView;
        ShutdownMode = ShutdownMode.OnMainWindowClose;

        launcherView.Show();

        WeakReferenceMessenger.Default.Register<ShowSettingsMessage>(this, (_, message) =>
        {
            var viewModel =
                container.Resolve<SettingsViewModel>(new DependencyOverride(typeof(bool), message.IsNewProfile));
            var settingsWindow =
                container.Resolve<SettingsView>(new DependencyOverride(typeof(SettingsViewModel), viewModel));
            settingsWindow.Show();
        });
    }
}