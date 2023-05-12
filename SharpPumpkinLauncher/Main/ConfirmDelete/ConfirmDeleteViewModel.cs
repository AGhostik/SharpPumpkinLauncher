using System;
using System.Reactive;
using System.Reactive.Subjects;
using ReactiveUI;
using SharpPumpkinLauncher.Main.Profile;

namespace SharpPumpkinLauncher.Main.ConfirmDelete;

public class ConfirmDeleteViewModel : ReactiveObject
{
    private readonly Action _close;
    private ProfileViewModel? _profileViewModel;
    private readonly Action<ProfileViewModel> _delete;

    public ConfirmDeleteViewModel(Action close, Action<ProfileViewModel> delete)
    {
        _close = close;
        _delete = delete;
        CloseCommand = ReactiveCommand.Create(Close);
        ConfirmCommand = ReactiveCommand.Create(Confirm, CanConfirm);
    }

    public void Setup(ProfileViewModel profileViewModel)
    {
        Profile = profileViewModel;
    }
    
    public ProfileViewModel? Profile
    {
        get => _profileViewModel;
        set
        {
            this.RaiseAndSetIfChanged(ref _profileViewModel, value);
            CanConfirm.OnNext(value != null);
        }
    }

    public ReactiveCommand<Unit, Unit> CloseCommand { get; }
    
    public ReactiveCommand<Unit, Unit> ConfirmCommand { get; }
    
    private Subject<bool> CanConfirm { get; } = new();
    
    private void Confirm()
    {
        if (Profile != null)
            _delete.Invoke(Profile);
    }

    private void Close()
    {
        _close.Invoke();
    }
}