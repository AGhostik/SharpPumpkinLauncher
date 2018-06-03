using System.Collections.Generic;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using MCLauncher.Model;
using MCLauncher.UI;
using MCLauncher.UI.Messages;
using NSubstitute;
using NUnit.Framework;

namespace Tests.UI
{
    [TestFixture]
    public class MainViewModelTests
    {
        [SetUp]
        public void SetUp()
        {
            _profileList = new List<string>()
            {
                "lastProfile",
                "profile1"
            };

            _mainModel = Substitute.For<IMainModel>();
            _mainModel.GetLastProfile().Returns("lastProfile");
            _mainModel.GetProfiles().Returns(_profileList);
        }

        private List<string> _profileList;
        private IMainModel _mainModel;

        [Test]
        public void EditProfile_OpenProfileEditingWindow()
        {
            var vm = new MainViewModel(_mainModel);
            vm.EditProfile.Execute(null);

            _mainModel.Received().OpenProfileEditingWindow();
        }

        [Test]
        public void NewProfile_OpenProfileCreatingWindow()
        {
            var vm = new MainViewModel(_mainModel);
            vm.NewProfile.Execute(null);

            _mainModel.Received().OpenProfileCreatingWindow();
        }

        [Test]
        public void Profiles_Loaded_Init()
        {
            var vm = new MainViewModel(_mainModel);
            Assert.AreEqual(vm.Profiles, _profileList);
        }

        [Test]
        public void Progress_EqualsMessageProgress_RecievedInstallProgressMessage()
        {
            var vm = new MainViewModel(_mainModel);
            Messenger.Default.Send(new InstallProgressMessage(50));

            Assert.AreEqual(vm.Progress, 50);
        }

        [Test]
        public void ProgressBarVisibility_Collapsed_ProgressEquals100()
        {
            var vm = new MainViewModel(_mainModel);
            Messenger.Default.Send(new InstallProgressMessage(100));

            Assert.AreEqual(vm.ProgresBarVisibility, Visibility.Collapsed);
        }

        [Test]
        public void ProgressBarVisibility_Visible_ProgressLess100()
        {
            var vm = new MainViewModel(_mainModel);
            Messenger.Default.Send(new InstallProgressMessage(50));

            Assert.AreEqual(vm.ProgresBarVisibility, Visibility.Visible);
        }

        [Test]
        public void RefreshProfiles_UpdateProfiles_RecievedProfilesChangedMessage()
        {
            var vm = new MainViewModel(_mainModel);
            vm.Profiles.Clear();
            Messenger.Default.Send(new ProfilesChangedMessage());

            Assert.IsNotEmpty(vm.Profiles);
        }

        [Test]
        public void Start_ExecuteStartGame()
        {
            var vm = new MainViewModel(_mainModel);
            vm.Start.Execute(null);

            _mainModel.Received().StartGame();
        }

        [Test]
        public void Status_UpdateStatus_RecievedStatusMessage()
        {
            var vm = new MainViewModel(_mainModel);
            Messenger.Default.Send(new StatusMessage("testStatus"));

            Assert.AreEqual(vm.Status, "testStatus");
        }
    }
}