using CommunityToolkit.Mvvm.ComponentModel;
using GeneA._Services;
using Model.Core;
using Model.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using MvvmHelpers;

namespace GeneA.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public MainViewModel(NavigationService navigationService)
    {
        _navigationService = navigationService;

        HomeCommand().SafeFireAndForget();
    }

    private readonly NavigationService _navigationService;

    public async Task HomeCommand()
    {
        await _navigationService.GoToAsync<HomeViewModel>();
    }

    public async Task PeopleCommand()
    {
        await _navigationService.GoToAsync<PeopleViewModel>();
    }

    public async Task SettingsCommand()
    {
        await _navigationService.GoToAsync<SettingsViewModel>();
    }
}
