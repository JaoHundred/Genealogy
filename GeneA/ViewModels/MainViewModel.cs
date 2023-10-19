using CommunityToolkit.Mvvm.ComponentModel;
using GeneA._Services;
using Model.Core;
using Model.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using MvvmHelpers;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Notification;

namespace GeneA.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public MainViewModel(NavigationService navigationService, INotificationMessageManager notificationManager)
    {
        _navigationService = navigationService;
        NotificationManager = notificationManager;

        HomeCommand.Execute(null);
    }

    private readonly NavigationService _navigationService;


    [ObservableProperty]
    private INotificationMessageManager _notificationManager;

    [RelayCommand]
    private async Task Home()
    {
        await _navigationService.GoToAsync<HomeViewModel>();
    }

    [RelayCommand]
    private async Task People()
    {
        await _navigationService.GoToAsync<PersonListingViewModel>();
    }

    [RelayCommand]
    private async Task Settings()
    {
        await _navigationService.GoToAsync<SettingsViewModel>();
    }
}
