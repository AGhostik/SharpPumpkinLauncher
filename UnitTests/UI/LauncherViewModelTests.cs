using System.Collections.Generic;
using System.Windows;
using CommunityToolkit.Mvvm.Messaging;
using MCLauncher.LauncherWindow;
using MCLauncher.Messages;
using NSubstitute;
using NUnit.Framework;

namespace UnitTests.UI
{
    [TestFixture]
    public class LauncherViewModelTests
    {
        private List<string?>? _profileList;
        private ILauncherModel? _launcherModel;
        
        [SetUp]
        public void SetUp()
        {
            _profileList = new List<string?>()
            {
                "lastProfile",
                "profile1"
            };

            _launcherModel = Substitute.For<ILauncherModel>();
            _launcherModel.GetLastProfile().Returns("lastProfile");
            _launcherModel.GetProfiles().Returns(_profileList);
        }

        [Test]
        public void EditProfile_OpenProfileEditingWindow()
        {
            if (_launcherModel == null)
            {
                Assert.Fail("SetUp not executed");
                return;
            }
            
            var vm = new LauncherViewModel(_launcherModel);
            vm.EditProfile?.Execute(null);

            _launcherModel.Received().OpenEditProfileWindow();
        }

        [Test]
        public void NewProfile_OpenProfileCreatingWindow()
        {
            if (_launcherModel == null)
            {
                Assert.Fail("SetUp not executed");
                return;
            }
            
            var vm = new LauncherViewModel(_launcherModel);
            vm.NewProfile?.Execute(null);

            _launcherModel.Received().OpenNewProfileWindow();
        }

        [Test]
        public void Profiles_Loaded_Init()
        {
            if (_launcherModel == null)
            {
                Assert.Fail("SetUp not executed");
                return;
            }
            
            var vm = new LauncherViewModel(_launcherModel);
            Assert.AreEqual(vm.Profiles, _profileList);
        }

        [Test]
        public void Progress_EqualsMessageProgress_RecievedInstallProgressMessage()
        {
            if (_launcherModel == null)
            {
                Assert.Fail("SetUp not executed");
                return;
            }
            
            var vm = new LauncherViewModel(_launcherModel);
            WeakReferenceMessenger.Default.Send(new InstallProgressMessage(50));

            Assert.AreEqual(vm.Progress, 50);
        }

        [Test]
        public void ProgressBarVisibility_Collapsed_ProgressEquals100()
        {
            if (_launcherModel == null)
            {
                Assert.Fail("SetUp not executed");
                return;
            }
            
            var vm = new LauncherViewModel(_launcherModel);
            WeakReferenceMessenger.Default.Send(new InstallProgressMessage(100));

            Assert.AreEqual(vm.ProgresBarVisibility, Visibility.Collapsed);
        }

        [Test]
        public void ProgressBarVisibility_Visible_ProgressLess100()
        {
            if (_launcherModel == null)
            {
                Assert.Fail("SetUp not executed");
                return;
            }
            
            var vm = new LauncherViewModel(_launcherModel);
            WeakReferenceMessenger.Default.Send(new InstallProgressMessage(50));

            Assert.AreEqual(vm.ProgresBarVisibility, Visibility.Visible);
        }

        [Test]
        public void RefreshProfiles_UpdateProfiles_RecievedProfilesChangedMessage()
        {
            if (_launcherModel == null)
            {
                Assert.Fail("SetUp not executed");
                return;
            }
            
            var vm = new LauncherViewModel(_launcherModel);
            vm.Profiles.Clear();
            WeakReferenceMessenger.Default.Send(new ProfilesChangedMessage());

            Assert.IsNotEmpty(vm.Profiles);
        }

        [Test]
        public void Start_ExecuteStartGame()
        {
            if (_launcherModel == null)
            {
                Assert.Fail("SetUp not executed");
                return;
            }
            
            var vm = new LauncherViewModel(_launcherModel);
            vm.Start?.Execute(null);

            _launcherModel.Received().StartGame();
        }

        [Test]
        public void Status_UpdateStatus_RecievedStatusMessage()
        {
            if (_launcherModel == null)
            {
                Assert.Fail("SetUp not executed");
                return;
            }
            
            var vm = new LauncherViewModel(_launcherModel);
            WeakReferenceMessenger.Default.Send(new StatusMessage("testStatus"));

            Assert.AreEqual(vm.Status, "testStatus");
        }
    }
}