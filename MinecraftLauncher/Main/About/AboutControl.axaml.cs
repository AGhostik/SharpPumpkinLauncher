using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MinecraftLauncher.Main.About;

public partial class AboutControl : UserControl
{
    public AboutControl()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}