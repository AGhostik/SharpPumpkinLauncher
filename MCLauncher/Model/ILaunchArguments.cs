using MCLauncher.Model.MinecraftVersionJson;

namespace MCLauncher.Model
{
    public interface ILaunchArguments
    {
        void AddLibrary(string fileName);
        void Create(Profile profile, MinecraftVersion minecraftVersion);
        string Get();
    }
}