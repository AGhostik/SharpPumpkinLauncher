using System.Windows;

namespace MCLauncher.UI
{
    public partial class MainView : Window
    {
        public MainView()
        {
            DataContext = new MainViewModel();
            InitializeComponent();
        }
    }
}