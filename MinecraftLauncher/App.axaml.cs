using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using MinecraftLauncher.Main;

namespace MinecraftLauncher
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var mainWindow = new MainWindow()
                {
                    DataContext = new MainWindowViewModel(new MainWindowModel())
                };

                desktop.MainWindow = mainWindow;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}