using MCLauncher.Json;
using MCLauncher.SettingsWindow;
using MCLauncher.Tools;
using MCLauncher.Tools.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace UnitTests.Model
{
    [TestFixture]
    public class SettingsModelTests
    {
        private Profile? _lastProfile;
        private IJsonManager? _jsonManager;
        private IFileManager? _fileManager;
        private IProfileManager? _profileManager;
        
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

        [Test]
        public void EditProfile_ReceivedEdit()
        {
            if (_fileManager == null || _profileManager == null || _jsonManager == null)
            {
                Assert.Fail("SetUp not executed");
                return;
            }
            
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
            if (_fileManager == null || _profileManager == null || _jsonManager == null)
            {
                Assert.Fail("SetUp not executed");
                return;
            }
            
            var model = new SettingsModel(_fileManager, _profileManager, _jsonManager);
            model.FindJava();

            _fileManager.Received().GetJavawPath();
        }

        [Test]
        public void LoadLastProfile_ReceivedGetLast()
        {
            if (_fileManager == null || _profileManager == null || _jsonManager == null)
            {
                Assert.Fail("SetUp not executed");
                return;
            }
            
            var model = new SettingsModel(_fileManager, _profileManager, _jsonManager);
            model.LoadLastProfile();

            _profileManager.Received().GetLast();
        }

        [Test]
        public void OpenGameDirectory_ReceivedStartProcess()
        {
            if (_fileManager == null || _profileManager == null || _jsonManager == null)
            {
                Assert.Fail("SetUp not executed");
                return;
            }
            
            var model = new SettingsModel(_fileManager, _profileManager, _jsonManager);
            model.OpenGameDirectory("path");

            //_fileManager.Received().DirectoryExist("path");
            _fileManager.Received().StartProcess("path");
        }

        [Test]
        public void SaveProfile_ReceivedSave()
        {
            if (_fileManager == null || _profileManager == null || _jsonManager == null)
            {
                Assert.Fail("SetUp not executed");
                return;
            }
            
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
            if (_fileManager == null || _profileManager == null || _jsonManager == null)
            {
                Assert.Fail("SetUp not executed");
                return;
            }
            
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