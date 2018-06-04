using System.Collections.Generic;
using MCLauncher.Model;
using MCLauncher.UI;
using NSubstitute;
using NUnit.Framework;

namespace Tests.UI
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
        public void OpenDirectory_OpenGameDirectory_ExecuteCommand()
        {
            var vm = new SettingsViewModel(_model, true);
            vm.OpenDirectory.Execute(null);

            _model.Received().OpenGameDirectory(string.Empty);
        }

        [Test]
        public void ProfileVisibility_Close_SelectedVisibilityChanged()
        {
            var vm = new SettingsViewModel(_model, true)
            {
                SelectedVisibility = UIResource.CloseLauncher
            };

            Assert.AreEqual(vm.CurrentProfile.LauncherVisibility, LauncherVisibility.Close);
        }

        [Test]
        public void ProfileVisibility_Hide_SelectedVisibilityChanged()
        {
            var vm = new SettingsViewModel(_model, true)
            {
                SelectedVisibility = UIResource.HideLauncher
            };

            Assert.AreEqual(vm.CurrentProfile.LauncherVisibility, LauncherVisibility.Hide);
        }

        [Test]
        public void ProfileVisibility_KeepOpen_SelectedVisibilityChanged()
        {
            var vm = new SettingsViewModel(_model, true)
            {
                SelectedVisibility = UIResource.KeepLauncherOpen
            };

            Assert.AreEqual(vm.CurrentProfile.LauncherVisibility, LauncherVisibility.KeepOpen);
        }

        [Test]
        public void Title_EqualsEditProfile_IsNewProfileFalse()
        {
            var vm = new SettingsViewModel(_model, false);

            Assert.AreEqual(vm.Title, UIResource.EditProfileTitle);
        }


        [Test]
        public void Title_EqualsNewProfile_IsNewProfileTrue()
        {
            var vm = new SettingsViewModel(_model, true);

            Assert.AreEqual(vm.Title, UIResource.NewProfileTitle);
        }

        [Test]
        public void Versions_HasAlpha_AlphaCheckboxTrue()
        {
            var vm = new SettingsViewModel(_model, true)
            {
                CurrentProfile = {ShowAlpha = true}
            };

            Assert.AreEqual(1, vm.Versions.Count);
            Assert.Contains("alpha1", vm.Versions);
        }

        [Test]
        public void Versions_HasBeta_BetaCheckboxTrue()
        {
            var vm = new SettingsViewModel(_model, true)
            {
                CurrentProfile = {ShowBeta = true}
            };

            Assert.AreEqual(1, vm.Versions.Count);
            Assert.Contains("beta1", vm.Versions);
        }

        [Test]
        public void Versions_HasCustom_CustomCheckboxTrue()
        {
            var vm = new SettingsViewModel(_model, true)
            {
                CurrentProfile = {ShowCustom = true}
            };

            Assert.AreEqual(1, vm.Versions.Count);
            Assert.Contains("custom1", vm.Versions);
        }

        [Test]
        public void Versions_HasRelease_ReleaseCheckboxTrue()
        {
            var vm = new SettingsViewModel(_model, true)
            {
                CurrentProfile = {ShowRelease = true}
            };

            Assert.AreEqual(1, vm.Versions.Count);
            Assert.Contains("release1", vm.Versions);
        }

        [Test]
        public void Versions_HasSnapshot_SnapshotCheckboxTrue()
        {
            var vm = new SettingsViewModel(_model, true)
            {
                CurrentProfile = {ShowSnapshot = true}
            };

            Assert.AreEqual(1, vm.Versions.Count);
            Assert.Contains("snapshot1", vm.Versions);
        }
    }
}