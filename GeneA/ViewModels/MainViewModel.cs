using CommunityToolkit.Mvvm.ComponentModel;
using GeneA._Services;
using Model.Core;
using Model.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeneA.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public MainViewModel(NavigationService navigationService, IRepository<Person> repository)
    {
        RecentlyAdded = new List<Person>();
        _navigationService = navigationService;
        _repository = repository;
    }

    private readonly NavigationService _navigationService;
    private readonly IRepository<Person> _repository;
    [ObservableProperty]
    private List<Person> _recentlyAdded;

    public async Task AddNewPerson()
    {
        await _navigationService.GoToAsync<PersonViewModel>();
    }
}
//TODO: create app menu of sorts(like maui shell hamburger menu)