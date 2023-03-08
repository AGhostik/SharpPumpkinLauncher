using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.Messaging;
using MCLauncher.LauncherWindow;
using MCLauncher.Messages;
using MCLauncher.SettingsWindow;
using MCLauncher.Tools;
using MCLauncher.Tools.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace UnitTests.Model
{
    [TestFixture]
    public class LauncherModelTests
    {
        private List<string>? _profiles;
        private IInstaller? _installer;
        private IFileManager? _fileManager;
        private IProfileManager? _profileManager;
        
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
            _profileManager.GetProfiles().Returns(new List<Profile?>()
            {
                new()
                {
                    Name = _profiles.First()
                }
            });
        }

        [Test]
        public void DeleteProfile_ReceivedDeleteProfile()
        {
            if (_fileManager == null || _profileManager == null || _installer == null)
            {
                Assert.Fail("SetUp not executed");
                return;
            }
                
            var model = new LauncherModel(_fileManager, _profileManager, _installer);
            model.DeleteProfile("name");

            _profileManager.Received().Delete("name");
        }

        [Test]
        public void GetProfiles_ReturnNotEmptyList_RecievedGetProfiles()
        {
            if (_fileManager == null || _profileManager == null || _installer == null)
            {
                Assert.Fail("SetUp not executed");
                return;
            }
            
            var model = new LauncherModel(_fileManager, _profileManager, _installer);
            var profiles = model.GetProfiles();

            _profileManager.Received().GetProfiles();

            Assert.AreEqual(profiles, _profiles);
        }

        [Test]
        public void OpenEditProfileWindow_ShowSettingsMessage_IsNewProfileFalse()
        {
            if (_fileManager == null || _profileManager == null || _installer == null)
            {
                Assert.Fail("SetUp not executed");
                return;
            }
            
            var model = new LauncherModel(_fileManager, _profileManager, _installer);
            var pass = false;
            WeakReferenceMessenger.Default.Reset();
            WeakReferenceMessenger.Default.Register<ShowSettingsMessage>(this, (_, message) =>
            {
                pass = !message.IsNewProfile;
            });
            model.OpenEditProfileWindow();

            if (pass)
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public void OpenNewProfileWindow_ShowSettingsMessage_IsNewProfileTrue()
        {
            if (_fileManager == null || _profileManager == null || _installer == null)
            {
                Assert.Fail("SetUp not executed");
                return;
            }
            
            var model = new LauncherModel(_fileManager, _profileManager, _installer);
            var pass = false;
            WeakReferenceMessenger.Default.Reset();
            WeakReferenceMessenger.Default.Register<ShowSettingsMessage>(this, (_, message) =>
            {
                pass = message.IsNewProfile;
            });
            model.OpenNewProfileWindow();

            if (pass)
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public void SaveLastProfileName_ReceivedSaveLastProfileName()
        {
            if (_fileManager == null || _profileManager == null || _installer == null)
            {
                Assert.Fail("SetUp not executed");
                return;
            }
            
            var model = new LauncherModel(_fileManager, _profileManager, _installer);
            model.SaveLastProfileName("name");

            _profileManager.Received().SaveLastProfileName("name");
        }

        [Test]
        public void StartGame_ReceivedStartProcess()
        {
            if (_fileManager == null || _profileManager == null || _installer == null)
            {
                Assert.Fail("SetUp not executed");
                return;
            }
            
            var model = new LauncherModel(_fileManager, _profileManager, _installer);
            model.StartGame().Wait();

            _fileManager.ReceivedWithAnyArgs().StartProcess("", "", null);
        }
    }
}