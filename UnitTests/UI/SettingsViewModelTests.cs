using System.Collections.Generic;
using System.Threading.Tasks;
using MCLauncher.Model;
using MCLauncher.UI;
using NSubstitute;
using NUnit.Framework;

namespace UnitTests.UI
{
    [TestFixture]
    public class SettingsViewModelTests
    {
        [SetUp]
        public void SetUp()
        {
            _model = Substitute.For<ISettingsModel>();
            _model.LoadLastProfile().Returns(new Profile());
            _model.DownloadAllVersions().Returns(new AllVersions()
            {
                Alpha = new List<string>() {"alpha1"},
                Beta = new List<string>() {"beta1"},
                Release = new List<string>() {"release1"},
                Custom = new List<string>() {"custom1"},
                Snapshot = new List<string>() {"snapshot1"}
            });
        }

        private ISettingsModel _model;

        [Test]
        public async Task OpenDirectory_OpenGameDirectory_ExecuteCommand()
        {
            var vm = new SettingsViewModel(_model, true);
            await vm.Init();
            vm.OpenDirectory.Execute(null);
  
            _model.ReceivedWithAnyArgs().OpenGameDirectory(string.Empty);
        }

        [Test]
        public async Task ProfileVisibility_Close_SelectedVisibilityChanged()
        {
            var vm = new SettingsViewModel(_model, true);
            await vm.Init();
            vm.SelectedVisibility = UIResource.CloseLauncher;

            Assert.AreEqual(vm.CurrentProfile.LauncherVisibility, LauncherVisibility.Close);
        }

        [Test]
        public async Task ProfileVisibility_Hide_SelectedVisibilityChanged()
        {
            var vm = new SettingsViewModel(_model, true);
            await vm.Init();
            vm.SelectedVisibility = UIResource.HideLauncher;

            Assert.AreEqual(vm.CurrentProfile.LauncherVisibility, LauncherVisibility.Hide);
        }

        [Test]
        public async Task ProfileVisibility_KeepOpen_SelectedVisibilityChanged()
        {
            var vm = new SettingsViewModel(_model, true);
            await vm.Init();
            vm.SelectedVisibility = UIResource.KeepLauncherOpen;

            Assert.AreEqual(vm.CurrentProfile.LauncherVisibility, LauncherVisibility.KeepOpen);
        }

        [Test]
        public async Task Title_EqualsEditProfile_IsNewProfileFalse()
        {
            var vm = new SettingsViewModel(_model, false);
            await vm.Init();

            Assert.AreEqual(vm.Title, UIResource.EditProfileTitle);
        }


        [Test]
        public async Task Title_EqualsNewProfile_IsNewProfileTrue()
        {
            var vm = new SettingsViewModel(_model, true);
            await vm.Init();

            Assert.AreEqual(vm.Title, UIResource.NewProfileTitle);
        }

        [Test]
        public async Task Versions_HasAlpha_AlphaCheckboxTrue()
        {
            var vm = new SettingsViewModel(_model, true);
            await vm.Init();
            vm.CurrentProfile.ShowAlpha = true;
            vm.CurrentProfile.ShowBeta = false;
            vm.CurrentProfile.ShowCustom = false;
            vm.CurrentProfile.ShowRelease = false;
            vm.CurrentProfile.ShowSnapshot = false;

            Assert.AreEqual(1, vm.Versions.Count);
            Assert.Contains("alpha1", vm.Versions);
        }

        [Test]
        public async Task Versions_HasBeta_BetaCheckboxTrue()
        {
            var vm = new SettingsViewModel(_model, true);
            await vm.Init();
            vm.CurrentProfile.ShowAlpha = false;
            vm.CurrentProfile.ShowBeta = true;
            vm.CurrentProfile.ShowCustom = false;
            vm.CurrentProfile.ShowRelease = false;
            vm.CurrentProfile.ShowSnapshot = false;

            Assert.AreEqual(1, vm.Versions.Count);
            Assert.Contains("beta1", vm.Versions);
        }

        [Test]
        public async Task Versions_HasCustom_CustomCheckboxTrue()
        {
            var vm = new SettingsViewModel(_model, true);
            await vm.Init();
            vm.CurrentProfile.ShowAlpha = false;
            vm.CurrentProfile.ShowBeta = false;
            vm.CurrentProfile.ShowCustom = true;
            vm.CurrentProfile.ShowRelease = false;
            vm.CurrentProfile.ShowSnapshot = false;

            Assert.AreEqual(1, vm.Versions.Count);
            Assert.Contains("custom1", vm.Versions);
        }

        [Test]
        public async Task Versions_HasRelease_ReleaseCheckboxTrue()
        {
            var vm = new SettingsViewModel(_model, true);
            await vm.Init();
            vm.CurrentProfile.ShowAlpha = false;
            vm.CurrentProfile.ShowBeta = false;
            vm.CurrentProfile.ShowCustom = false;
            vm.CurrentProfile.ShowRelease = true;
            vm.CurrentProfile.ShowSnapshot = false;

            Assert.AreEqual(1, vm.Versions.Count);
            Assert.Contains("release1", vm.Versions);
        }

        [Test]
        public async Task Versions_HasSnapshot_SnapshotCheckboxTrue()
        {
            var vm = new SettingsViewModel(_model, true);
            await vm.Init();
            vm.CurrentProfile.ShowAlpha = false;
            vm.CurrentProfile.ShowBeta = false;
            vm.CurrentProfile.ShowCustom = false;
            vm.CurrentProfile.ShowRelease = false;
            vm.CurrentProfile.ShowSnapshot = true;

            Assert.AreEqual(1, vm.Versions.Count);
            Assert.Contains("snapshot1", vm.Versions);
        }
    }
}