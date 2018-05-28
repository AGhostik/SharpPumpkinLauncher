using System;
using System.Windows;
using MCLauncher.Model;

namespace MCLauncher.UI
{
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
            _init();
        }

        private void _init()
        {
            var fileManager = new FileManager();
            var installer = new Installer(fileManager);
            var settingsModel = new SettingsModel(fileManager);
            var settingsViewModel = new SettingsViewModel(settingsModel);
            var mainModel = new MainModel(installer, settingsViewModel);
            DataContext = new MainViewModel(mainModel);
        }
    }
}