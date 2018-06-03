using System.Threading.Tasks;

namespace MCLauncher.Model
{
    public interface IInstaller
    {
        string LaunchArgs { get; }

        Task Install(Profile profile);
    }
}