using CommunityToolkit.Mvvm.ComponentModel;
using GeneA._Services;
using Model.Core;
using Model.Interfaces;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneA.ViewModels;

public partial class HomeViewModel : ViewModelBase
{
    public HomeViewModel(NavigationService navigationService, IRepository<Person> repository)
    {
        _navigationService = navigationService;
        _repository = repository;

        RecentlyAdded = new ObservableRangeCollection<Person>();

        Load().SafeFireAndForget();
    }

    private readonly NavigationService _navigationService;
    private readonly IRepository<Person> _repository;

    [ObservableProperty]
    private ObservableRangeCollection<Person> _recentlyAdded;

    private async Task Load()
    {
        await Task.Run(() =>
        {
            RecentlyAdded.ReplaceRange(_repository.Take(20));
        });
    }

    public async Task AddNewPersonCommand()
    {
        await _navigationService.GoToAsync<PersonViewModel>();
    }

    public async Task EditPersonCommand(Person person)
    {
        if (person != null)
            await _navigationService.GoToAsync<PersonViewModel>(person.Id);
    }
}
