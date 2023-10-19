using Avalonia.Threading;
using Avalonia.Xaml.Interactions.Custom;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GeneA._Helper;
using GeneA._Services;
using GeneA.ViewModelItems;
using Model.Core;
using Model.Interfaces;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneA.ViewModels
{
    public partial class PersonListingViewModel : ViewModelBase
    {
        public PersonListingViewModel(IRepository<Person> repository, NavigationService navigationService)
        {
            _repository = repository;
            _navigationService = navigationService;

            People = new ObservableRangeCollection<PersonItemViewModel>();

            LoadAction = () => { Load().SafeFireAndForget(); };
        }

        private readonly IRepository<Person> _repository;
        private readonly NavigationService _navigationService;

        private List<PersonItemViewModel>? _originalPeople;
        [ObservableProperty]
        private ObservableRangeCollection<PersonItemViewModel> _people;

        public async Task Load()
        {
            await Task.Run(async () =>
            {
                _originalPeople = _repository.FindAll().ToPersonItemViewModels(isSelected: false).ToList();

                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    People.ReplaceRange(_originalPeople);
                });
            });
        }

        [RelayCommand]
        private async Task EditPerson(Person person)
        {
            if (person != null)
                await _navigationService.GoToAsync<PersonViewModel>(person.Id);
        }

        [RelayCommand]
        private async Task DeleteSelectedPeople()
        {
            //TODO: ask for delete confirmation here with dialog


            var entitiesToDelete = _originalPeople!.Where(x => x.IsSelected).ToList();

            Task databaseDeleteTask = _repository.DeleteBatchAsync(entitiesToDelete);

            _originalPeople!.RemoveAll(p => p.IsSelected);

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                People.ReplaceRange(_originalPeople);
            });

            await databaseDeleteTask;
        }
    }
}
