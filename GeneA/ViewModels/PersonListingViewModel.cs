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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneA.ViewModels
{
    public partial class PersonListingViewModel : ViewModelBase, IDisposable
    {
        public PersonListingViewModel(IRepository<Person> personRepository, IRepository<Nationality> nationalityRepository, 
            NavigationService navigationService)
        {
            _personRepository = personRepository;
            _nationalityRepository = nationalityRepository;
            _navigationService = navigationService;

            People = new ObservableRangeCollection<PersonItemViewModel>();
            SelectedFilterItems = new ObservableRangeCollection<FilterItemViewModel>();
            IsAscendingChecked = true;

            LoadAction = () => { Load().SafeFireAndForget(); };
        }

        private readonly IRepository<Person> _personRepository;
        private readonly IRepository<Nationality> _nationalityRepository;
        private readonly NavigationService _navigationService;
        private IDisposable? disposable;

        private List<PersonItemViewModel>? _originalPeople;
        [ObservableProperty]
        private ObservableRangeCollection<PersonItemViewModel> _people;

        [ObservableProperty]
        private bool _canDelete;

        [ObservableProperty]
        private bool _isAllChecked;

        [ObservableProperty]
        private bool _isAscendingChecked;

        [ObservableProperty]
        private PersonItemViewModel? _selectedPersonItem;

        [ObservableProperty]
        private List<FilterItemViewModel>? _filterItems;

        [ObservableProperty]
        private ObservableRangeCollection<FilterItemViewModel> _selectedFilterItems;

        [ObservableProperty]
        private bool _isFilterPaneOpen;

        [ObservableProperty]
        private List<NationalityItemViewModel>? _nationalityItems;

        [ObservableProperty]
        private NationalityItemViewModel? _selectedNationalityItem;

        [ObservableProperty]
        private List<Gender> _genders;

        [ObservableProperty]
        private Gender? _selectedGender;

        [ObservableProperty]
        private DateTime? _birthDateStart;

        [ObservableProperty]
        private DateTime? _deathDateStart;

        [ObservableProperty]
        private DateTime? _birthDateEnd;

        [ObservableProperty]
        private DateTime? _deathDateEnd;

        public async Task Load()
        {
            await Task.Run(async () =>
            {
                var filters = FilterHelper.FillFilters();
                
                var nationalityItems = _nationalityRepository.FindAll().ToNationalityItemViewModels().ToList();
                var genders = StaticList.FillGenders().ToList();

                _originalPeople = _personRepository.FindAll().ToPersonItemViewModels(isSelected: false).ToList();

                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    People.ReplaceRange(_originalPeople);
                    FilterItems = filters;
                    Genders = genders;
                    NationalityItems = nationalityItems;
                    
                    BirthDateStart = _originalPeople.Select(p => p.BirthDate).Min();
                    BirthDateEnd = _originalPeople.Select(p => p.BirthDate).Max();

                    DeathDateStart = _originalPeople.Select(p => p.DeathDate).Min();
                    DeathDateEnd = _originalPeople.Select(p => p.DeathDate).Max();
                });

                disposable = Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>
                 (
                   handler => SelectedFilterItems.CollectionChanged += handler,
                   handler => SelectedFilterItems.CollectionChanged -= handler
                 )
                 .Throttle(TimeSpan.FromSeconds(1))
                 .Subscribe(_ =>
                 {
                     SelectFilters();
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
        private void OpenFilterPane(bool isOpen)
        {
            IsFilterPaneOpen = isOpen;
        }

        [RelayCommand]
        private void Checked()
        {
            CanDelete = People.Any(p => p.IsSelected!.Value);
        }

        [RelayCommand]
        private async Task TextFilter(string searchText)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                People.ReplaceRange(_originalPeople!.Where(p => p.Name
                .ToLower().Contains(searchText.ToLower())));
            });

            CanDelete = People.Count > 0 && People.Any(p => p.IsSelected!.Value);
        }

        private List<PersonItemViewModel> _filteredList = new List<PersonItemViewModel>();

        private void SelectFilters()
        {
            _filteredList = _originalPeople!.ToList();

            if (SelectedFilterItems.Count  == 0) 
            {
                People.ReplaceRange(_originalPeople!);
                return;
            }

            bool groupOperationSelected = false;

            //TODO: dont let opposite filters came into play, everyone which have the prefixe "has"
            //need to be removed here, or create another set of listbox for those filters with only "single"
            //selection enabled, maybe it is the better way

            foreach (var filter in SelectedFilterItems)
            {
                switch (filter.FilterType)
                {
                    //case FilterType.BirthDate:
                    //    _filteredList = IsAscendingChecked
                    //        ? _filteredList.OrderBy(p => p.BirthDate).ToList()
                    //        : _filteredList.OrderByDescending(p => p.BirthDate).ToList();
                    //    break;
                    //case FilterType.DeathDate:
                    //    _filteredList = IsAscendingChecked
                    //        ? _filteredList.OrderBy(p => p.DeathDate).ToList()
                    //        : _filteredList.OrderByDescending(p => p.DeathDate).ToList();
                    //    break;

                    //TODO: in the date cases, work with intervals(slider control, left side is the smallest date and right side
                    //highest date, 

                    //case FilterType.Wedding: break;

                    //case FilterType.Baptism:

                    //    _filteredList = IsAscendingChecked
                    //        ? _filteredList.OrderBy(p => p.BaptismDate).ToList()
                    //        : _filteredList.OrderByDescending(p => p.BaptismDate).ToList();
                    //    break;
                    
                    case FilterType.HasChildren: 

                        _filteredList = _filteredList.Where(p => p.Offsprings.Count > 0).ToList();
                        break;
                    case FilterType.HasParents:

                        _filteredList = _filteredList
                            .Where(p => p.Father != null && p.Mother != null).ToList();
                        break;
                    case FilterType.HasSpouse:
                        _filteredList = _filteredList
                            .Where(p => p.Spouses.Count > 0).ToList();
                        break;
                    //case FilterType.Nationality: 
                    //    //TODO: maybe it will be better to use a combobox for this case
                    //    //the user picks the nationality and it will be grouped by that
                    //    break;
                    

                    //case FilterType.Gender:
                    //    groupOperationSelected = true;
                    //    break;
                }
            }

            if(groupOperationSelected)
                _filteredList = _filteredList.GroupBy(p => p.Gender).SelectMany(p => p).ToList();

            People.ReplaceRange(_filteredList);
        }

        //TODO: implement other filtering here, use birth date and death date with slider

        [RelayCommand]
        private async Task DeleteSelectedPeople()
        {
            await _navigationService.PopUpAsync<ConfirmationPopupViewModel>().ConfigurePopUpProperties
                (
                 confirmAction: async () =>
                 {
                     var entitiesToDelete = _originalPeople!.Where(x => x.IsSelected!.Value).ToList();

                     Task databaseDeleteTask = _personRepository.DeleteBatchAsync(entitiesToDelete);

                     _originalPeople!.RemoveAll(p => p.IsSelected!.Value);

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

        public void Dispose()
        {
            disposable?.Dispose();
        }
    }
}
