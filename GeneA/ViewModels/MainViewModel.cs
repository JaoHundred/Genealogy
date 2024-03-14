using CommunityToolkit.Mvvm.ComponentModel;
using GeneA._Services;
using ModelA.Core;
using Model.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using MvvmHelpers;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Notification;
using System.Runtime;

namespace GeneA.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public MainViewModel(NavigationService navigationService, INotificationMessageManager notificationManager
        , IRepository<Settings> settingsRepo, ThemeService themeService)
    {
        _navigationService = navigationService;
        NotificationManager = notificationManager;

        var settings = settingsRepo.FindById(1);

        if (settings == null)
            settingsRepo.Upsert(new Settings { ColorTheme = 0 });
        else
            themeService.ChangeTheme(settings.ColorTheme);

        HomeCommand.Execute(null);
    }

    private readonly NavigationService _navigationService;

    [ObservableProperty]
    private string? _title;

    [ObservableProperty]
    private INotificationMessageManager _notificationManager;

    [ObservableProperty]
    private bool _canGoback;

    [RelayCommand]
    private async Task GoBack()
    {
        if (_navigationService.GetLastViewModel()?.GetType() == typeof(PersonListingViewModel)) // make PersonListing to dont clean the filters
            await _navigationService.GoBackAsync(needToReload: false);
        else
            await _navigationService.GoBackAsync(param: _navigationService.GetLastViewModel()?.Param);
    }

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
