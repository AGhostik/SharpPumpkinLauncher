using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MCLauncher.Model;
using NSubstitute;
using NUnit.Framework;

namespace Tests.Model
{
    [TestFixture]
    public class MainModelTests
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
            _fileManager.GetLastProfile().Returns(new Profile()
            {
                LauncherVisibility = LauncherVisibility.Hide,
                JavaFile = "javafile",
                JvmArgs = "jvmargs"
            });
            _fileManager.GetProfiles().Returns(new List<Profile>()
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

        [Test]
        public void DeleteProfile_ReceivedDeleteProfile()
        {
            var model = new MainModel(_installer, _fileManager);
            model.DeleteProfile("name");

            _fileManager.Received().DeleteProfile("name");
        }

        [Test]
        public void GetProfiles_ReturnNotEmptyList_RecievedGetProfiles()
        {
            var model = new MainModel(_installer, _fileManager);
            var profiles = model.GetProfiles();

            _fileManager.Received().GetProfiles();

            Assert.AreEqual(profiles, _profiles);
        }

        [Test]
        public void SaveLastProfileName_ReceivedSaveLastProfileName()
        {
            var model = new MainModel(_installer, _fileManager);
            model.SaveLastProfileName("name");

            _fileManager.Received().SaveLastProfileName("name");
        }

        [Test]
        public async Task StartGame_ReceivedStartProcess()
        {
            var model = new MainModel(_installer, _fileManager);
            await model.StartGame();

            _fileManager.ReceivedWithAnyArgs().StartProcess("", "", null);
        }
    }
}