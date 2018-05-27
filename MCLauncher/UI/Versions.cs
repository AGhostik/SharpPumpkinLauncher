using System.Collections.Generic;

namespace MCLauncher.UI
{
    public class Versions
    {
        public List<string> Custom { get; set; } = new List<string>();
        public List<string> Release { get; set; } = new List<string>();
        public List<string> Snapshot { get; set; } = new List<string>();
        public List<string> Beta { get; set; } = new List<string>();
        public List<string> Alpha { get; set; } = new List<string>();
    }
}
