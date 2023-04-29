using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MinecraftLauncher.Main.Jre;

public partial class JreControl : UserControl
{
    public JreControl()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}