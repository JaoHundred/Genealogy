﻿using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GeneA._Services;
using GeneA.Interfaces;
using GeneA.ViewModelItems;
using Model.Core;
using Model.Database;
using Model.Interfaces;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneA._Helper;

namespace GeneA.ViewModels
{
    //TODO: edit NationalitySelectionPopupView to make possible to register nationalities, the base overall functonality of the
    //popup is the same as OffspringSelectionPopupView and Spouse

    public partial class NationalitySelectionPopupViewModel : ViewModelBase, IpopupViewModel
    {
        public NationalitySelectionPopupViewModel(
            IRepository<Person> personRepo, IRepository<Nationality> nationalityRepo, NavigationService navigationService)
        {
            _personRepo = personRepo;
            _nationalityRepo = nationalityRepo;
            _navigationService = navigationService;

            Nationalities = new ObservableRangeCollection<NationalityItemViewModel>();
            SearchedNationality = string.Empty;

            LoadAction = () => { Load().SafeFireAndForget(); };
        }

        private readonly IRepository<Person> _personRepo;
        private readonly IRepository<Nationality> _nationalityRepo;
        private readonly NavigationService _navigationService;
        private Person? _person;

        private List<NationalityItemViewModel>? _originalNationalities;
        [ObservableProperty]
        private ObservableRangeCollection<NationalityItemViewModel> _nationalities;

        [ObservableProperty]
        private bool _canAdd;
        [ObservableProperty]
        private string _searchedNationality;

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
                    _person = _personRepo.FindById((long)Param);
                    _originalNationalities = _nationalityRepo.FindAll().ToNationalityItemViewModels().ToList();

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
            await Task.Run(() =>
            {

                //TODO: extract the information from SearchedNationality and create an Nationality
                //object, add it in the Nationalities, _originalNationalities and database
                //see if the lists remain syncronized

            });
        }

        [RelayCommand]
        private async Task TextFilter(string searchText)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                Nationalities.ReplaceRange(_originalNationalities!.Where(p => p.Name.ToLower().StartsWith(searchText.ToLower())));

                if (Nationalities.Count == 0 || Nationalities.Count == 1)
                {
                    //TODO: make possible to create new nationality, it will be addede to the listing and to the DB
                    //the rule is: country(can contain spaces) space place(2 letters), regex could work for this
                    //$"{country} {place.length == 2}" valid format
                    //TODO: regex here, if its the correct form enable CanAdd
                    CanAdd = true;
                }
            });
        }

        [RelayCommand]
        private async Task DeleteNationality()
        {
            await Task.Run(() =>
            {
                //TODO: make possible to delete existing nationality, it will be removed from the list and DB
            });
        }

        [RelayCommand]
        public async Task Confirm()
        {
            await Task.Run(async () =>
            {
                _person!.Nationality = _originalNationalities!.FirstOrDefault(p => p.IsSelected)!.ToNationality();

                _personRepo.Upsert(_person);

                await _navigationService.GoBackAsync();

                ConfirmAction?.Invoke();
            });
        }

        [RelayCommand]
        public async Task Cancel()
        {
            await _navigationService.GoBackAsync();

            CancelAction?.Invoke();
        }
    }
}
