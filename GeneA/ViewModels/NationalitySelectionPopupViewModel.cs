using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GeneA._Services;
using GeneA.Interfaces;
using GeneA.ViewModelItems;
using ModelA.Core;
using Model.Database;
using Model.Interfaces;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneA._Helper;
using System.Text.RegularExpressions;

namespace GeneA.ViewModels
{
    public partial class NationalitySelectionPopupViewModel : ViewModelBase, IpopupViewModel
    {
        public NationalitySelectionPopupViewModel(IRepository<Nationality> nationalityRepo)
        {
            _nationalityRepo = nationalityRepo;

            Nationalities = new ObservableRangeCollection<NationalityItemViewModel>();
            SearchedNationality = string.Empty;
            FlyoutNationalityHelp = DynamicTranslate.Translate(nameof(FlyoutNationalityHelp));

            LoadAction = () => { Load().SafeFireAndForget(); };
        }

        private readonly IRepository<Nationality> _nationalityRepo;

        private Person? _person;
        private Match? _canAddNationalityMatch;

        private List<NationalityItemViewModel>? _originalNationalities;
        [ObservableProperty]
        private ObservableRangeCollection<NationalityItemViewModel> _nationalities;

        [ObservableProperty]
        private bool _canAdd;

        [ObservableProperty]
        private string _searchedNationality;

        [ObservableProperty]
        private string _flyoutNationalityHelp;

        public string? Title { get; set; }
        public string? Message { get; set; }
        public Action? ConfirmAction { get; set; }
        public Action? CancelAction { get; set; }

        public async Task Load()
        {
            await Task.Run(async () =>
            {
                if (Param != null)
                {
                    _person = (Person)Param;

                    if (_person.Nationality?.Id > 0)
                        _person.Nationality = _nationalityRepo.FindById(_person.Nationality.Id);

                    _originalNationalities = _nationalityRepo.FindAll().ToNationalityItemViewModels().ToList();

                    foreach (var item in _originalNationalities)
                    {
                        if (item.Id == _person.Nationality?.Id)
                        {
                            item.IsSelected = true;
                            break;
                        }
                    }

                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        Nationalities.ReplaceRange(_originalNationalities);
                    });
                }
            });
        }

        [RelayCommand]
        private async Task NewNationality()
        {
            await Task.Run(async () =>
            {
                if (_canAddNationalityMatch!.Success)
                {
                    var nationality = new Nationality
                    {
                        Name = _canAddNationalityMatch.Groups[1].Value
                        ,
                        Abbreviation = _canAddNationalityMatch.Groups[2].Value
                    };

                    var nationalityItemViewModel =
                     _nationalityRepo.Upsert(nationality).ToNationalityItemViewModel();

                    _originalNationalities?.Add(nationalityItemViewModel);

                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        Nationalities.Add(nationalityItemViewModel);
                        SearchedNationality = string.Empty;
                        CanAdd = false;
                    });
                }
            });
        }

        [RelayCommand]
        private async Task DeleteNationality(NationalityItemViewModel nationalityItem)
        {
            await Task.Run(async () =>
            {
                _originalNationalities!.Remove(nationalityItem);

                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    Nationalities.Remove(nationalityItem);
                    SearchedNationality = string.Empty;
                });

                _nationalityRepo.Delete(nationalityItem);
            });
        }

        [RelayCommand]
        private async Task TextFilter(string searchText)
        {
            if(_originalNationalities == null) 
                return;

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                Nationalities.ReplaceRange(_originalNationalities.Where(p => p.ToString()
                .ToLower().Contains(searchText.ToLower())));

                if (!Nationalities.Any(p => p.ToString() == searchText))
                {
                    _canAddNationalityMatch = RegexHelper.CanAddNationality(searchText);
                    CanAdd = _canAddNationalityMatch.Success;
                }
                else
                    CanAdd = false;
            });
        }

        [RelayCommand]
        private async Task SelectChanged(NationalityItemViewModel nationalityItem)
        {
            if (nationalityItem == null)
                return;

            await Task.Run(() =>
            {
                //desselect anything
                _originalNationalities!.ForEach((NationalityItemViewModel nationality) =>
                {
                    if (nationality.Id != nationalityItem.Id)
                    {
                        nationality.IsSelected = false;
                    }
                });

            });
        }

        [RelayCommand]
        public void Confirm()
        {
            _person!.Nationality = _originalNationalities!.FirstOrDefault(p => p.IsSelected!.Value)?.ToNationality();

            ConfirmAction?.Invoke();// save in not popup side
        }

        [RelayCommand]
        public void Cancel()
        {
            CancelAction?.Invoke();
        }
    }
}
