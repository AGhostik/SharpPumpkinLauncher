using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MinecraftLauncher.Main.Profile;

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