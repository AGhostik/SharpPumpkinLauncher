using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MCLauncher.Model;
using MCLauncher.Model.Managers;
using NSubstitute;
using NUnit.Framework;

namespace Tests.Model
{
    [TestFixture]
    public class LauncherModelTests
    {
        [SetUp]
        public void SetUp()
        {
            _profiles = new List<string>()
            {
                "profile1"
            };

            _installer = Substitute.For<IInstaller>();
            _fileManager = Substitute.For<IFileManager>();
            _profileManager = Substitute.For<IProfileManager>();

            _profileManager.GetLast().Returns(new Profile()
            {
                LauncherVisibility = LauncherVisibility.Hide,
                JavaFile = "javafile",
                JvmArgs = "jvmargs"
            });
            _profileManager.GetProfiles().Returns(new List<Profile>()
            {
                new Profile()
                {
                    Name = _profiles.First()
                }
            });
        }

        private List<string> _profiles;
        private IInstaller _installer;
        private IFileManager _fileManager;
        private IProfileManager _profileManager;

        [Test]
        public void DeleteProfile_ReceivedDeleteProfile()
        {
            var model = new LauncherModel(_installer, _profileManager, _fileManager);
            model.DeleteProfile("name");

            _profileManager.Received().Delete("name");
        }

        [Test]
        public void GetProfiles_ReturnNotEmptyList_RecievedGetProfiles()
        {
            var model = new LauncherModel(_installer, _profileManager, _fileManager);
            var profiles = model.GetProfiles();

            _profileManager.Received().GetProfiles();

            Assert.AreEqual(profiles, _profiles);
        }

        [Test]
        public void SaveLastProfileName_ReceivedSaveLastProfileName()
        {
            var model = new LauncherModel(_installer, _profileManager, _fileManager);
            model.SaveLastProfileName("name");

            _profileManager.Received().SaveLastProfileName("name");
        }

        [Test]
        public async Task StartGame_ReceivedStartProcess()
        {
            var model = new LauncherModel(_installer, _profileManager, _fileManager);
            await model.StartGame();

            _fileManager.ReceivedWithAnyArgs().StartProcess("", "", null);
        }
    }
}