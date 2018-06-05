using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using MCLauncher.Model;
using MCLauncher.Model.Managers;
using MCLauncher.UI.Messages;
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
            var model = new LauncherModel(_fileManager, _profileManager, _installer);
            model.DeleteProfile("name");

            _profileManager.Received().Delete("name");
        }

        [Test]
        public void GetProfiles_ReturnNotEmptyList_RecievedGetProfiles()
        {
            var model = new LauncherModel(_fileManager, _profileManager, _installer);
            var profiles = model.GetProfiles();

            _profileManager.Received().GetProfiles();

            Assert.AreEqual(profiles, _profiles);
        }

        [Test]
        public void OpenEditProfileWindow_ShowSettingsMessage_IsNewProfileFalse()
        {
            var model = new LauncherModel(_fileManager, _profileManager, _installer);
            var pass = false;
            Messenger.Default.Register(this, (ShowSettingsMessage message) => { pass = !message.IsNewProfile; });
            model.OpenEditProfileWindow();

            if (pass)
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public void OpenNewProfileWindow_ShowSettingsMessage_IsNewProfileTrue()
        {
            var model = new LauncherModel(_fileManager, _profileManager, _installer);
            var pass = false;
            Messenger.Default.Register(this, (ShowSettingsMessage message) => { pass = message.IsNewProfile; });
            model.OpenNewProfileWindow();

            if (pass)
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public void SaveLastProfileName_ReceivedSaveLastProfileName()
        {
            var model = new LauncherModel(_fileManager, _profileManager, _installer);
            model.SaveLastProfileName("name");

            _profileManager.Received().SaveLastProfileName("name");
        }

        [Test]
        public void StartGame_ReceivedStartProcess()
        {
            var model = new LauncherModel(_fileManager, _profileManager, _installer);
            model.StartGame().Wait();

            _fileManager.ReceivedWithAnyArgs().StartProcess("", "", null);
        }
    }
}