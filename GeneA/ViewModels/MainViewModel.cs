using CommunityToolkit.Mvvm.ComponentModel;
using GeneA._Services;
using Model.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeneA.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public MainViewModel(NavigationService navigationService)
    {
        RecentlyAdded = new List<Person>();
        _navigationService = navigationService;
    }

    private readonly NavigationService _navigationService;

    [ObservableProperty]
    private List<Person> _recentlyAdded;

    public async Task AddNewPerson()
    {
        //TODO:navigate to AddPersonViewModel

        _navigationService.GoTo<AddPersonViewModel>();
    }
}
