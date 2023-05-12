using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace SharpPumpkinLauncher.Main.ConfirmDelete;

public partial class ConfirmDeleteControl : UserControl
{
    public ConfirmDeleteControl()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}