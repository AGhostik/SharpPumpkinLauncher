using System.Windows;
using CommunityToolkit.Mvvm.Messaging;
using MCLauncher.UI.Messages;

namespace MCLauncher.UI;

public partial class SettingsView
{
    private readonly SettingsViewModel _viewModel;

    public SettingsView(SettingsViewModel viewModel)
    {
        _viewModel = viewModel;
        DataContext = viewModel;
        Owner = Application.Current.MainWindow;
        InitializeComponent();

        WeakReferenceMessenger.Default.Register<ProfilesChangedMessage>(this, (_, _) => { Close(); });
    }

    private async void SettingsView_OnLoaded(object sender, RoutedEventArgs e)
    {
        await _viewModel.Init();
    }
}