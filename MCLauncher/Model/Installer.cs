using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MCLauncher.Model.VersionsJson;

namespace MCLauncher.Model
{
    public class Installer
    {
        private readonly FileManager _fileManager;

        public Installer(FileManager fileManager)
        {
            _fileManager = fileManager;
        }

        public List<string> GetVersions(bool custom, bool release, bool snapshot, bool beta, bool alpha)
        {
            var versions = new List<string>();

            var json = _fileManager.DownloadJson(ModelResource.Versions);
            var jVersions = json["versions"].ToObject<Version[]>();

            if (release)
            {
                versions.AddRange(jVersions.Where(_ => _.Type == ModelResource.release).Select(_ => _.Id));
            }
            if (snapshot)
            {
                versions.AddRange(jVersions.Where(_ => _.Type == ModelResource.snapshot).Select(_ => _.Id));
            }
            if (beta)
            {
                versions.AddRange(jVersions.Where(_ => _.Type == ModelResource.beta).Select(_ => _.Id));
            }
            if (alpha)
            {
                versions.AddRange(jVersions.Where(_ => _.Type == ModelResource.alpha).Select(_ => _.Id));
            }

            return versions;
        }

        public void Install()
        {
        }
    }
}
