using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using MCLauncher.Model;
using MCLauncher.Model.Managers;
using MCLauncher.UI;
using MCLauncher.UI.Messages;
using Unity;
using Unity.Resolution;

namespace MCLauncher
{
    public partial class App : Application
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

            Messenger.Default.Register(this, (ShowSettingsMessage message) =>
            {
                var viewModel =
                    container.Resolve<SettingsViewModel>(new DependencyOverride(typeof(bool), message.IsNewProfile));
                var settingsWindow =
                    container.Resolve<SettingsView>(new DependencyOverride(typeof(SettingsViewModel), viewModel));
                settingsWindow.Show();
            });
        }
    }
}