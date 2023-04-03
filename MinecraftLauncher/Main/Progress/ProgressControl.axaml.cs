using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MinecraftLauncher.Main.Progress;

public partial class ProgressControl : UserControl
{
    public ProgressControl()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}