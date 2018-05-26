using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCLauncher.Model
{
    public class Profile
    {
        public string Name { get; set; }
        public string Nickname { get; set; }
        public string JavaDirectory { get; set; }
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
