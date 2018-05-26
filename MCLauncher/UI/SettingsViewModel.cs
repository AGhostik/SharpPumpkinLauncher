using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using MCLauncher.Model;

namespace MCLauncher.UI
{
    public class SettingsViewModel : ViewModelBase
    {
        public SettingsViewModel()
        {
            _init();
        }

        public string Name { get; set; }
        public string Nickname { get; set; }
        public string JavaFile { get; set; }
        public string GameDirectory { get; set; }
        public string JvmArgs { get; set; }
        public LauncherVisibility LauncherVisibility { get; set; }
        public string MinecraftVersion { get; set; }
        public bool ShowCustom { get; set; }
        public bool ShowRelease { get; set; }
        public bool ShowSnapshot { get; set; }
        public bool ShowBeta { get; set; }
        public bool ShowAlpha { get; set; }

        public string SelectedVisibility { get; set; }

        public List<string> Visibilitys { get; private set; }
        public ObservableCollection<string> Versions { get; set; } = new ObservableCollection<string>();

        private void _init()
        {
            Visibilitys = new List<string>
            {
                "KeepOpen",
                "Hide",
                "Close"
            };
        }

        public Profile GetProfile()
        {
            return new Profile
            {
                Name = Name,
                Nickname = Nickname,
                JavaFile = JavaFile,
                GameDirectory = GameDirectory,
                JvmArgs = JvmArgs,
                LauncherVisibility = LauncherVisibility,
                MinecraftVersion = MinecraftVersion,
                ShowCustom = ShowCustom,
                ShowRelease = ShowRelease,
                ShowSnapshot = ShowSnapshot,
                ShowBeta = ShowBeta,
                ShowAlpha = ShowAlpha
            };
        }
    }
}