using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using MCLauncher.Model;
using MCLauncher.UI.Messages;

namespace MCLauncher.UI
{
    public partial class LauncherView : Window
    {
        public LauncherView()
        {
            InitializeComponent();
            _init();
        }

        private void _init()
        {
            var fileManager = new FileManager();
            var installer = new Installer(fileManager);
            var mainModel = new LauncherModel(installer, fileManager);
            DataContext = new LauncherViewModel(mainModel);

            Messenger.Default.Register(this, (MinecraftExitedMessage message) => { Dispatcher.Invoke(Show); });
        }
    }
}