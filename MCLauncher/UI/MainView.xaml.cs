using System.Windows;
using MCLauncher.Model;

namespace MCLauncher.UI
{
    public partial class MainView : Window
    {
        public MainView()
        {
            var fileManager = new FileManager();
            var installer = new Installer(fileManager);
            DataContext = new MainViewModel(installer);
            InitializeComponent();
        }
    }
}