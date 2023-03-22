using System.Windows;
using CommunityToolkit.Mvvm.Messaging;
using MCLauncher.Messages;

namespace MCLauncher.SettingsWindow;

public partial class SettingsView
{
    public SettingsView(SettingsViewModel viewModel)
    {
        DataContext = viewModel;
        Owner = Application.Current.MainWindow;
        InitializeComponent();

        WeakReferenceMessenger.Default.Register<ProfileSaved>(this, (_, _) => Close());
    }
}