using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace SharpPumpkinLauncher.Main.Settings;

public partial class SettingsControl : UserControl
{
    public SettingsControl()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}