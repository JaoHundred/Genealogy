using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using GeneA._Helper;
using GeneA._Services;
using GeneA.Interfaces;
using GeneA.ViewModelItems;
using Model.Core;
using Model.Interfaces;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneA.ViewModels
{

    public partial class SelectionPopupViewModel : ViewModelBase, IpopupViewModel
    {
        public SelectionPopupViewModel(IRepository<Person> repository, NavigationService navigationService)
        {
            _repository = repository;
            _navigationService = navigationService;
            OffSprings = new ObservableRangeCollection<PersonItemViewModel>();

            LoadAction = () => { Load().SafeFireAndForget(); };
        }

        private Person? _person;
        private IRepository<Person> _repository;
        private NavigationService _navigationService;

        [ObservableProperty]
        private ObservableRangeCollection<PersonItemViewModel> _offSprings;
        private List<PersonItemViewModel>? _offSpringsOriginal;

        public string? Title { get; set; }
        public string? Message { get; set; }
        public Action? ConfirmAction { get; set; }
        public Action? CancelAction { get; set; }

        private async Task Load()
        {
            await Task.Run(async() =>
            {
                if (Param != null)
                {
                    _person = _repository.FindById((long)Param);
                        
                    var people = _repository.FindAll().ToPersonItemViewModels().ToList();
                    people.RemoveAll(p => p.Id == _person.Id);//remove self

                    foreach (var ps in people)//set as checked if person.offsprings
                        ps.IsSelected = _person.Offsprings.Any(p => p.Id == ps.Id);

                    _offSpringsOriginal = people;

                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        OffSprings.ReplaceRange(_offSpringsOriginal);
                    });
                }
            });
        }

        [RelayCommand]
        public async Task TextFilter(string searchText)
        {
            await Dispatcher.UIThread.InvokeAsync(() => 
            {
                OffSprings.ReplaceRange(_offSpringsOriginal!.Where(p => p.Name.ToLower().StartsWith(searchText.ToLower()))); 
            });
        }

        [RelayCommand]
        public async Task Confirm()
        {
            await Task.Run(async() =>
            {
                _person!.Offsprings = _offSpringsOriginal!.Where(p => p.IsSelected).ToPeople().ToList();

                _repository.Upsert(_person);

                await _navigationService.GoBackAsync();

                //ConfirmAction?.Invoke();
            });
        }

        [RelayCommand]
        public void Cancel()
        {
            CancelAction?.Invoke();
        }
    }
}
