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
            var mainModel = new MainModel(installer, fileManager);
            DataContext = new MainViewModel(mainModel);
        }
    }
}