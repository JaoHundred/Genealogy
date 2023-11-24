using Avalonia.Threading;
using Avalonia.Xaml.Interactions.Custom;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GeneA._Helper;
using GeneA._Services;
using GeneA.ViewModelItems;
using ModelA.Core;
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
    public partial class PersonListingViewModel : ViewModelBase
    {
        public PersonListingViewModel(IRepository<Person> personRepository, IRepository<Nationality> nationalityRepository,
            NavigationService navigationService)
        {
            _personRepository = personRepository;
            _nationalityRepository = nationalityRepository;
            _navigationService = navigationService;

            People = new ObservableRangeCollection<PersonItemViewModel>();
            IsAscendingChecked = true;

            LoadAction = () => { Load().SafeFireAndForget(); };
        }

        private readonly IRepository<Person> _personRepository;
        private readonly IRepository<Nationality> _nationalityRepository;
        private readonly NavigationService _navigationService;

        private List<PersonItemViewModel>? _originalPeople;
        [ObservableProperty]
        private ObservableRangeCollection<PersonItemViewModel> _people;

        [ObservableProperty]
        private bool _canDelete;

        [ObservableProperty]
        private bool _isAllChecked;

        [ObservableProperty]
        private bool? _isAscendingChecked;

        [ObservableProperty]
        private PersonItemViewModel? _selectedPersonItem;

        [ObservableProperty]
        private List<FilterItemViewModel>? _filterItems;

        [ObservableProperty]
        private bool _isFilterPaneOpen;

        [ObservableProperty]
        private List<NationalityItemViewModel>? _nationalityItems;

        [ObservableProperty]
        private NationalityItemViewModel? _selectedNationalityItem;

        [ObservableProperty]
        private List<Gender>? _genders;

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

        [ObservableProperty]
        private DateTime? _bapTismStart;
        [ObservableProperty]
        private DateTime? _bapTismEnd;

        [ObservableProperty]
        private DateTime? _weddingStart;
        [ObservableProperty]
        private DateTime? _weddingEnd;

        [ObservableProperty]
        private DateTime? _selectedBirthDate;
        partial void OnSelectedBirthDateChanged(DateTime? value)
        {
            if (value > SelectedDeathDate)
                SelectedBirthDate = SelectedDeathDate;
        }

        [ObservableProperty]
        private DateTime? _selectedDeathDate;
        partial void OnSelectedDeathDateChanged(DateTime? value)
        {
            if (value < SelectedBirthDate)
                SelectedDeathDate = SelectedBirthDate;
        }

        [ObservableProperty]
        private DateTime? _selectedBaptismDateLeft;
        partial void OnSelectedBaptismDateLeftChanged(DateTime? value)
        {
            if (value > SelectedBaptismDateRight)
                SelectedBaptismDateLeft = SelectedBaptismDateRight;
        }

        [ObservableProperty]
        private DateTime? _selectedBaptismDateRight;
        partial void OnSelectedBaptismDateRightChanged(DateTime? value)
        {
            if (value < SelectedBaptismDateLeft)
                SelectedBaptismDateRight = SelectedBaptismDateLeft;
        }

        [ObservableProperty]
        private DateTime? _selectedWeddingDateLeft;
        partial void OnSelectedWeddingDateLeftChanged(DateTime? value)
        {
            if (value > SelectedWeddingDateRight)
                SelectedWeddingDateLeft = SelectedWeddingDateRight;
        }

        [ObservableProperty]
        private DateTime? _selectedWeddingDateRight;
        partial void OnSelectedWeddingDateRightChanged(DateTime? value)
        {
            if (value < SelectedWeddingDateLeft)
                SelectedWeddingDateRight = SelectedWeddingDateLeft;
        }

        [ObservableProperty]
        private string _searchText = string.Empty;

        public async Task Load()
        {
            await Task.Run(async () =>
            {
                var filters = FilterHelper.FillFilters();

                var nationalityItems = _nationalityRepository.FindAll().ToNationalityItemViewModels().ToList();
                var genders = StaticList.FillGenders().ToList();

                _originalPeople = _personRepository.FindAll().ToPersonItemViewModels(isSelected: false).ToList();

                foreach (var person in _originalPeople)
                {
                    if (person.Father != null)
                        person.Father = _personRepository.FindById(person.Father.Id);

                    if (person.Mother != null)
                        person.Mother = _personRepository.FindById(person.Mother.Id);
                }

                foreach (var nationality in nationalityItems)
                {
                    var person = _originalPeople.FirstOrDefault(p => p.Nationality?.Id == nationality.Id);

                    if (person != null)
                        person.Nationality = nationality;
                }

                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    FilterItems = filters;
                    Genders = genders;
                    NationalityItems = nationalityItems;

                    BirthDateStart = _originalPeople.Select(p => p.BirthDate).Min();
                    BirthDateEnd = _originalPeople.Select(p => p.BirthDate).Max();

                    DeathDateStart = _originalPeople.Select(p => p.DeathDate).Min();
                    DeathDateEnd = _originalPeople.Select(p => p.DeathDate).Max();

                    BapTismStart = _originalPeople.Select(p => p.BaptismDate).Min();
                    BapTismEnd = _originalPeople.Select(p => p.BaptismDate).Max();

                    WeddingStart = _originalPeople.Select(p => p.WeddingDate).Min();
                    WeddingEnd = _originalPeople.Select(p => p.WeddingDate).Max();

                    People.ReplaceRange(_originalPeople);
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
        private void TextFilter()
        {
            Filter();
        }

        private IEnumerable<PersonItemViewModel>? _filteredList;

        [RelayCommand]
        private void ApplyFilters()
        {
            Filter();
        }

        private void Filter()
        {
            if (SearchText == null || FilterItems == null)
                return;

            _filteredList = _originalPeople!;

            _filteredList = _originalPeople!.Where(p => p.Name
                .ToLower().Contains(SearchText.ToLower()));

            if (SelectedBirthDate != null && SelectedDeathDate != null)
            {
                _filteredList = _filteredList
                     .Where(p => p.BirthDate >= SelectedBirthDate && p.DeathDate <= SelectedDeathDate);
            }

            if (SelectedBaptismDateLeft != null && SelectedBaptismDateRight != null)
            {
                _filteredList = _filteredList
                     .Where(p => p.BaptismDate >= SelectedBaptismDateLeft && p.BaptismDate <= SelectedBaptismDateRight);
            }

            if (SelectedWeddingDateLeft != null && SelectedWeddingDateRight != null)
            {
                _filteredList = _filteredList
                     .Where(p => p.WeddingDate >= SelectedWeddingDateLeft && p.WeddingDate <= SelectedWeddingDateRight);
            }

            foreach (var filter in FilterItems!)
            {
                if (filter.IsSelected.HasValue && !filter.IsSelected.Value)
                    continue;

                switch (filter.FilterType)
                {
                    case FilterType.HasChildren:

                        _filteredList = _filteredList.Where(p => p.Offsprings.Count > 0);

                        break;

                    case FilterType.HasNotChildren:

                        _filteredList = _filteredList.Where(p => p.Offsprings.Count == 0);

                        break;

                    case FilterType.HasParents:

                        _filteredList = _filteredList
                            .Where(p => !string.IsNullOrEmpty(p.Father?.Name) && !string.IsNullOrEmpty(p.Mother?.Name));
                        break;

                    case FilterType.HasNotParents:

                        _filteredList = _filteredList
                            .Where(p => string.IsNullOrEmpty(p.Father?.Name) || string.IsNullOrEmpty(p.Mother?.Name));

                        break;

                    case FilterType.HasSpouse:

                        _filteredList = _filteredList
                            .Where(p => p.Spouses.Count > 0);

                        break;

                    case FilterType.HasNotSpouse:

                        _filteredList = _filteredList
                            .Where(p => p.Spouses.Count == 0);

                        break;
                }
            }

            if (SelectedNationalityItem != null)
                _filteredList = _filteredList.Where(p => p.Nationality == SelectedNationalityItem);

            switch (SelectedGender?.GenderEnum)
            {
                case ModelA.Enums.GenderEnum.Gender.Male:
                    _filteredList = _filteredList.Where(p => p.Gender == ModelA.Enums.GenderEnum.Gender.Male);
                    break;
                case ModelA.Enums.GenderEnum.Gender.Female:
                    _filteredList = _filteredList.Where(p => p.Gender == ModelA.Enums.GenderEnum.Gender.Female);
                    break;
                default:
                    break;
            }

            if (IsAscendingChecked != null)
                _filteredList = IsAscendingChecked.Value ?
                    _filteredList.OrderBy(p => p.Name) :
                    _filteredList.OrderByDescending(p => p.Name);

            People.ReplaceRange(_filteredList);
            CanDelete = People.Count > 0 && People.Any(p => p.IsSelected!.Value);
        }

        [RelayCommand]
        private void ResetFilters()
        {
            IsAscendingChecked = true;
            SelectedBirthDate = null;
            SelectedDeathDate = null;
            SelectedBaptismDateLeft = null;
            SelectedBaptismDateRight = null;
            SelectedWeddingDateLeft = null;
            SelectedWeddingDateRight = null;
            SelectedGender = null;
            SelectedNationalityItem = null;
            FilterItems?.ForEach(p => p.IsSelected = false);

            People.ReplaceRange(_originalPeople!);
        }


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
                         IsAllChecked = false;
                     });

                     await databaseDeleteTask;
                     await _navigationService.GoBackAsync(needToReload: false);
                 },
                 cancelAction: async () =>
                 {
                     _originalPeople!.ForEach((personVM) => { personVM.IsSelected = false; });
                     IsAllChecked = false;

                     await _navigationService.GoBackAsync(needToReload: false);
                 },
                 title: DynamicTranslate.Translate(MessageConsts.ConfirmationDialogTitle),
                 message: DynamicTranslate.Translate(MessageConsts.Confirmation)
                );
        }
    }
}
