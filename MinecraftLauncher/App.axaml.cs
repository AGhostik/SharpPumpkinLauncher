using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using MinecraftLauncher.Main;

namespace MinecraftLauncher
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var model = new MainWindowModel();
                var mainWindow = new MainWindow()
                {
                    DataContext = new MainWindowViewModel(model)
                };
                
                mainWindow.Opened += async (_, _) => await model.InitAsync();

                desktop.MainWindow = mainWindow;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}