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

        [ObservableProperty]
        private bool _canDelete;

        [ObservableProperty]
        private bool _isAllChecked;

        [ObservableProperty]
        private PersonItemViewModel? _selectedPersonItem;

        [ObservableProperty]
        private List<FilterItemViewModel>? _filterItems;

        [ObservableProperty]
        private FilterItemViewModel? _selectedFilterItem;

        public async Task Load()
        {
            await Task.Run(async () =>
            {
                var filters = FilterHelper.FillFilters();
                _originalPeople = _repository.FindAll().ToPersonItemViewModels(isSelected: false).ToList();

                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    People.ReplaceRange(_originalPeople);
                    FilterItems = filters;
                });
            });
        }

        [RelayCommand]
        private async Task EditPerson()
        {
            if (SelectedPersonItem != null)
            {
                CanDelete = false;
                await _navigationService.GoToAsync<PersonViewModel>(SelectedPersonItem.Id);
            }
        }

        [RelayCommand]
        private void CheckAll(bool state)
        {
            if (People.Count == 0)
            {
                CanDelete = false;
                return;
            }

            foreach (var item in People)
                item.IsSelected = state;

            CanDelete = state;
        }

        [RelayCommand]
        private void Checked()
        {
            CanDelete = People.Any(p => p.IsSelected);
        }

        [RelayCommand]
        private async Task TextFilter(string searchText)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                People.ReplaceRange(_originalPeople!.Where(p => p.Name
                .ToLower().Contains(searchText.ToLower())));
            });

            CanDelete = People.Count > 0 && People.Any(p => p.IsSelected);
        }

        //TODO: implement other filtering here, use birth date and death date with slider

        [RelayCommand]
        private async Task DeleteSelectedPeople()
        {
            await _navigationService.PopUpAsync<ConfirmationPopupViewModel>().ConfigurePopUpProperties
                (
                 confirmAction: async () =>
                 {
                     var entitiesToDelete = _originalPeople!.Where(x => x.IsSelected).ToList();

                     Task databaseDeleteTask = _repository.DeleteBatchAsync(entitiesToDelete);

                     _originalPeople!.RemoveAll(p => p.IsSelected);

                     await Dispatcher.UIThread.InvokeAsync(() =>
                     {
                         People.ReplaceRange(_originalPeople);
                     });

                     await databaseDeleteTask;
                     await _navigationService.GoBackAsync();
                 },
                 cancelAction: async () =>
                 {
                     _originalPeople!.ForEach((personVM) => { personVM.IsSelected = false; });
                     IsAllChecked = false;

                     await _navigationService.GoBackAsync();
                 },
                 title: DynamicTranslate.Translate(MessageConsts.ConfirmationDialogTitle),
                 message: DynamicTranslate.Translate(MessageConsts.Confirmation)
                );
        }
    }
}
