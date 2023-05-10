using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace SharpPumpkinLauncher.Main.Profile;

public partial class ProfileControl : UserControl
{
    public ProfileControl()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}