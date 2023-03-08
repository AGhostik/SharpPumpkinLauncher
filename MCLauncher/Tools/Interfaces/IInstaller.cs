using System.Threading.Tasks;

namespace MCLauncher.Tools.Interfaces;

public interface IInstaller
{
    string? LaunchArgs { get; }

    Task Install(Profile? profile);
}