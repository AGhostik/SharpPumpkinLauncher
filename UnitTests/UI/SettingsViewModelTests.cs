using System.Threading.Tasks;
using MCLauncher.Properties;
using MCLauncher.SettingsWindow;
using MCLauncher.Tools;
using NSubstitute;
using NUnit.Framework;

namespace UnitTests.UI
{
    [TestFixture]
    public class SettingsViewModelTests
    {
        private ISettingsModel? _model;
        
        [SetUp]
        public void SetUp()
        {
            var versions = new Versions();
            versions.AddMinecraftVersion("alpha1", "empty", "empty", WellKnownValues.Alpha);
            versions.AddMinecraftVersion("beta1", "empty", "empty", WellKnownValues.Beta);
            versions.AddMinecraftVersion("snapshot1", "empty", "empty", WellKnownValues.Snapshot);
            versions.AddMinecraftVersion("release1", "empty", "empty", WellKnownValues.Release);
            versions.AddCustomMinecraftVersion("custom1", "empty", "empty");
            
            _model = Substitute.For<ISettingsModel>();
            _model.LoadLastProfile().Returns(new Profile());
            _model.DownloadAllVersions().Returns(versions);
        }

        [Test]
        public async Task OpenDirectory_OpenGameDirectory_ExecuteCommand()
        {
            if (_model == null)
            {
                Assert.Fail("SetUp not executed");
                return;
            }
            
            var vm = new SettingsViewModel(_model, true);
            await vm.Init();
            vm.OpenDirectory?.Execute(null);
  
            _model.ReceivedWithAnyArgs().OpenGameDirectory(string.Empty);
        }

        [Test]
        public async Task ProfileVisibility_Close_SelectedVisibilityChanged()
        {
            if (_model == null)
            {
                Assert.Fail("SetUp not executed");
                return;
            }
            
            var vm = new SettingsViewModel(_model, true);
            await vm.Init();
            vm.SelectedLauncherVisibility = UIResource.CloseLauncher;

            Assert.AreEqual(vm.CurrentProfile?.LauncherVisibility, LauncherVisibility.Close);
        }

        [Test]
        public async Task ProfileVisibility_Hide_SelectedVisibilityChanged()
        {
            if (_model == null)
            {
                Assert.Fail("SetUp not executed");
                return;
            }
            
            var vm = new SettingsViewModel(_model, true);
            await vm.Init();
            vm.SelectedLauncherVisibility = UIResource.HideLauncher;

            Assert.AreEqual(vm.CurrentProfile?.LauncherVisibility, LauncherVisibility.Hide);
        }

        [Test]
        public async Task ProfileVisibility_KeepOpen_SelectedVisibilityChanged()
        {
            if (_model == null)
            {
                Assert.Fail("SetUp not executed");
                return;
            }
            
            var vm = new SettingsViewModel(_model, true);
            await vm.Init();
            vm.SelectedLauncherVisibility = UIResource.KeepLauncherOpen;

            Assert.AreEqual(vm.CurrentProfile?.LauncherVisibility, LauncherVisibility.KeepOpen);
        }

        [Test]
        public async Task Title_EqualsEditProfile_IsNewProfileFalse()
        {
            if (_model == null)
            {
                Assert.Fail("SetUp not executed");
                return;
            }
            
            var vm = new SettingsViewModel(_model, false);
            await vm.Init();

            Assert.AreEqual(vm.Title, UIResource.EditProfileTitle);
        }


        [Test]
        public async Task Title_EqualsNewProfile_IsNewProfileTrue()
        {
            if (_model == null)
            {
                Assert.Fail("SetUp not executed");
                return;
            }
            
            var vm = new SettingsViewModel(_model, true);
            await vm.Init();

            Assert.AreEqual(vm.Title, UIResource.NewProfileTitle);
        }

        [Test]
        public async Task Versions_HasAlpha_AlphaCheckboxTrue()
        {
            if (_model == null)
            {
                Assert.Fail("SetUp not executed");
                return;
            }
            
            var vm = new SettingsViewModel(_model, true);
            await vm.Init();

            if (vm.CurrentProfile != null)
            {
                vm.CurrentProfile.ShowAlpha = true;
                vm.CurrentProfile.ShowBeta = false;
                vm.CurrentProfile.ShowCustom = false;
                vm.CurrentProfile.ShowRelease = false;
                vm.CurrentProfile.ShowSnapshot = false;
            }

            Assert.AreEqual(1, vm.Versions.Count);
            Assert.Contains("alpha1", vm.Versions);
        }

        [Test]
        public async Task Versions_HasBeta_BetaCheckboxTrue()
        {
            if (_model == null)
            {
                Assert.Fail("SetUp not executed");
                return;
            }
            
            var vm = new SettingsViewModel(_model, true);
            await vm.Init();

            if (vm.CurrentProfile != null)
            {
                vm.CurrentProfile.ShowAlpha = false;
                vm.CurrentProfile.ShowBeta = true;
                vm.CurrentProfile.ShowCustom = false;
                vm.CurrentProfile.ShowRelease = false;
                vm.CurrentProfile.ShowSnapshot = false;
            }

            Assert.AreEqual(1, vm.Versions.Count);
            Assert.Contains("beta1", vm.Versions);
        }

        [Test]
        public async Task Versions_HasCustom_CustomCheckboxTrue()
        {
            if (_model == null)
            {
                Assert.Fail("SetUp not executed");
                return;
            }
            
            var vm = new SettingsViewModel(_model, true);
            await vm.Init();

            if (vm.CurrentProfile != null)
            {
                vm.CurrentProfile.ShowAlpha = false;
                vm.CurrentProfile.ShowBeta = false;
                vm.CurrentProfile.ShowCustom = true;
                vm.CurrentProfile.ShowRelease = false;
                vm.CurrentProfile.ShowSnapshot = false;
            }

            Assert.AreEqual(1, vm.Versions.Count);
            Assert.Contains("custom1", vm.Versions);
        }

        [Test]
        public async Task Versions_HasRelease_ReleaseCheckboxTrue()
        {
            if (_model == null)
            {
                Assert.Fail("SetUp not executed");
                return;
            }
            
            var vm = new SettingsViewModel(_model, true);
            await vm.Init();

            if (vm.CurrentProfile != null)
            {
                vm.CurrentProfile.ShowAlpha = false;
                vm.CurrentProfile.ShowBeta = false;
                vm.CurrentProfile.ShowCustom = false;
                vm.CurrentProfile.ShowRelease = true;
                vm.CurrentProfile.ShowSnapshot = false;
            }

            Assert.AreEqual(1, vm.Versions.Count);
            Assert.Contains("release1", vm.Versions);
        }

        [Test]
        public async Task Versions_HasSnapshot_SnapshotCheckboxTrue()
        {
            if (_model == null)
            {
                Assert.Fail("SetUp not executed");
                return;
            }
            
            var vm = new SettingsViewModel(_model, true);
            await vm.Init();

            if (vm.CurrentProfile != null)
            {
                vm.CurrentProfile.ShowAlpha = false;
                vm.CurrentProfile.ShowBeta = false;
                vm.CurrentProfile.ShowCustom = false;
                vm.CurrentProfile.ShowRelease = false;
                vm.CurrentProfile.ShowSnapshot = true;
            }

            Assert.AreEqual(1, vm.Versions.Count);
            Assert.Contains("snapshot1", vm.Versions);
        }
    }
}