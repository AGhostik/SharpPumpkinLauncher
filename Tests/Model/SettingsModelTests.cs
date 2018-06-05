using MCLauncher.Model;
using MCLauncher.Model.Managers;
using NSubstitute;
using NUnit.Framework;

namespace Tests.Model
{
    [TestFixture]
    public class SettingsModelTests
    {
        [SetUp]
        public void SetUp()
        {
            _jsonManager = Substitute.For<IJsonManager>();
            _fileManager = Substitute.For<IFileManager>();
            _profileManager = Substitute.For<IProfileManager>();

            _lastProfile = new Profile()
            {
                LauncherVisibility = LauncherVisibility.Hide,
                JavaFile = "javafile",
                JvmArgs = "jvmargs"
            };

            _profileManager.GetLast().Returns(_lastProfile);

            _fileManager.DirectoryExist("").ReturnsForAnyArgs(true);
        }

        private Profile _lastProfile;
        private IJsonManager _jsonManager;
        private IFileManager _fileManager;
        private IProfileManager _profileManager;

        [Test]
        public void EditProfile_ReceivedEdit()
        {
            var model = new SettingsModel(_fileManager, _profileManager, _jsonManager);
            var profile = new Profile()
            {
                Name = "name2"
            };
            model.EditProfile("name1", profile);

            _profileManager.Received().Edit("name1", profile);
        }

        [Test]
        public void FindJava_ReceivedGetLast()
        {
            var model = new SettingsModel(_fileManager, _profileManager, _jsonManager);
            model.FindJava();

            _fileManager.Received().GetJavawPath();
        }

        [Test]
        public void LoadLastProfile_ReceivedGetLast()
        {
            var model = new SettingsModel(_fileManager, _profileManager, _jsonManager);
            model.LoadLastProfile();

            _profileManager.Received().GetLast();
        }

        [Test]
        public void OpenGameDirectory_ReceivedStartProcess()
        {
            var model = new SettingsModel(_fileManager, _profileManager, _jsonManager);
            model.OpenGameDirectory("path");

            //_fileManager.Received().DirectoryExist("path");
            _fileManager.Received().StartProcess("path");
        }

        [Test]
        public void SaveProfile_ReceivedSave()
        {
            var model = new SettingsModel(_fileManager, _profileManager, _jsonManager);
            var profile = new Profile()
            {
                Name = "name"
            };
            model.SaveProfile(profile);

            _profileManager.Received().Save(profile);
        }

        [Test]
        public void SaveProfile_ReceivedSaveLastProfileName()
        {
            var model = new SettingsModel(_fileManager, _profileManager, _jsonManager);
            var profile = new Profile()
            {
                Name = "name"
            };
            model.SaveProfile(profile);

            _profileManager.Received().SaveLastProfileName(profile.Name);
        }
    }
}