using System.Collections.ObjectModel;
using System.Reactive;
using MinecraftLauncher.Main.Profile;
using ReactiveUI;

namespace MinecraftLauncher.Main;

public sealed class MainWindowViewModel : ReactiveObject
{
    //private readonly MainWindowModel _mainWindowModel;
    private object? _mainContent;
    private ProfileViewModel? _selectedProfile;

    public MainWindowViewModel()
    {
        //_mainWindowModel = mainWindowModel;
        
        StartGameCommand = ReactiveCommand.Create(StartGame);
        NewProfileCommand = ReactiveCommand.Create(NewProfile);
        EditProfileCommand = ReactiveCommand.Create(EditProfile);
        DeleteProfileCommand = ReactiveCommand.Create(DeleteProfile);
        OpenSettingsCommand = ReactiveCommand.Create(OpenSettings);
    }

    public ProfileViewModel? SelectedProfile
    {
        get => _selectedProfile;
        set => this.RaiseAndSetIfChanged(ref _selectedProfile, value);
    }

    public ObservableCollection<ProfileViewModel> Profiles { get; } = new();

    public object? MainContent
    {
        get => _mainContent;
        set => this.RaiseAndSetIfChanged(ref _mainContent, value);
    }

    public ReactiveCommand<Unit, Unit> NewProfileCommand { get; }

    private void NewProfile()
    {
        MainContent = new ProfileControl()
        {
            DataContext = ProfileViewModel.CreateNew(NewProfileSaved, CloseProfileContent)
        };
    }

    private void NewProfileSaved(ProfileViewModel newProfileViewModel)
    {
        Profiles.Add(newProfileViewModel);
        SelectedProfile ??= newProfileViewModel;

        MainContent = null;
    }

    public ReactiveCommand<Unit, Unit> EditProfileCommand { get; }

    private void EditProfile()
    {
        if (SelectedProfile == null)
            return;
        
        MainContent = new ProfileControl()
        {
            DataContext = ProfileViewModel.Edit(SelectedProfile, ProfileEdited, CloseProfileContent)
        };
    }

    private void ProfileEdited(ProfileViewModel editedProfileViewModel)
    {
        MainContent = null;
    }

    private void CloseProfileContent()
    {
        MainContent = null;
    }

    public ReactiveCommand<Unit, Unit> DeleteProfileCommand { get; }

    private void DeleteProfile()
    {
        if (SelectedProfile != null)
            Profiles.Remove(SelectedProfile);
    }

    public ReactiveCommand<Unit, Unit> OpenSettingsCommand { get; }

    private void OpenSettings()
    {
        //
    }

    public ReactiveCommand<Unit, Unit> StartGameCommand { get; }

    private void StartGame()
    {
        //
    }
}