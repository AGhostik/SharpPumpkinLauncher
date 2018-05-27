using System;

namespace MCLauncher.Model
{
    [Serializable]
    public class Profile
    {
        public Profile()
        {
            Name = string.Empty;
            Nickname = string.Empty;
            JavaFile = string.Empty;
            GameDirectory = string.Empty;
            JvmArgs = string.Empty;
            LauncherVisibility = LauncherVisibility.KeepOpen;
            MinecraftVersion = string.Empty;
            ShowCustom = false;
            ShowRelease = false;
            ShowSnapshot = false;
            ShowBeta = false;
            ShowAlpha = false;
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
    }
}