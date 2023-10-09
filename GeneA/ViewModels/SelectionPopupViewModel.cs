using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GeneA._Helper;
using GeneA.Interfaces;
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
    public partial class SelectionPopupViewModel : ViewModelBase, IpopupViewModel
    {
        public SelectionPopupViewModel(IRepository<Person> repository)
        {
            _repository = repository;

            OffSprings = new ObservableRangeCollection<PersonItemViewModel>();

            LoadAction = () => { Load().SafeFireAndForget(); };
        }

        private Person _person;
        private IRepository<Person> _repository;

        [ObservableProperty]
        private ObservableRangeCollection<PersonItemViewModel> _offSprings;

        public string? Title { get; set; }
        public string? Message { get; set; }
        public Action? ConfirmAction { get; set; }
        public Action? CancelAction { get; set; }

        private async Task Load()
        {
            await Task.Run(() =>
            {
                if (Param != null)
                {
                    _person = _repository.FindById((long)Param);

                    var people = _repository.FindAll().ToPersonItemViewModels().ToList();
                    people.RemoveAll(p => p.Id == _person.Id);//remove self

                    foreach (var ps in people)//set as checked if person.offsprings
                        ps.IsSelected = _person.Offsprings.Any(p => p.Id == ps.Id);

                    Dispatcher.UIThread.Invoke(() =>
                    {
                        OffSprings.ReplaceRange(people);
                    });
                }
            });
        }

        [RelayCommand]
        public void Confirm()
        {
            //TODO: set all checked offpsprings in _person.offsprings and then save _person in litedb

            ConfirmAction?.Invoke();
        }

        [RelayCommand]
        public void Cancel()
        {
            CancelAction?.Invoke();
        }
    }
}
