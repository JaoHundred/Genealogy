using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GeneA._Helper;
using GeneA._Services;
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
    public partial class SpouseSelectionPopupViewModel : ViewModelBase, IpopupViewModel
    {

        public SpouseSelectionPopupViewModel(IRepository<Person> repository)
        {
            _repository = repository;
            _spouses = new ObservableRangeCollection<PersonItemViewModel>();

            LoadAction = () => { Load().SafeFireAndForget(); };
        }

        private Person? _person;

        private readonly IRepository<Person> _repository;
        private List<PersonItemViewModel>? _originalSpouses;

        [ObservableProperty]
        private ObservableRangeCollection<PersonItemViewModel> _spouses;

        public string? Title { get; set; }
        public string? Message { get; set; }
        public Action? ConfirmAction { get; set; }
        public Action? CancelAction { get; set; }

        private async Task Load()
        {
            await Task.Run(async () =>
            {
                if (Param != null)
                {
                    _person = (Person)Param;

                    if (_person.Gender == ModelA.Enums.GenderEnum.Gender.Male)
                    {
                        _originalSpouses = _repository.FindAll().Where(p => p.Gender == ModelA.Enums.GenderEnum.Gender.Female)
                        .ToPersonItemViewModels().ToList();
                    }
                    else
                    {
                        _originalSpouses = _repository.FindAll().Where(p => p.Gender == ModelA.Enums.GenderEnum.Gender.Male)
                        .ToPersonItemViewModels().ToList();
                    }

                    foreach (var os in _originalSpouses)//set as checked if person.Spouses
                        os.IsSelected = _person.Spouses.Any(p => p.Id == os.Id);

                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        Spouses.ReplaceRange(_originalSpouses);
                    });

                }
            });
        }

        [RelayCommand]
        public async Task TextFilter(string searchText)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                Spouses.ReplaceRange(_originalSpouses!.Where(p => p.Name.ToLower().StartsWith(searchText.ToLower())));
            });
        }

        [RelayCommand]
        public void Confirm()
        {
            _person!.Spouses = _originalSpouses!.Where(p => p.IsSelected!.Value).ToPeople().ToList();

            ConfirmAction?.Invoke();
        }

        [RelayCommand]
        public void Cancel()
        {
            CancelAction?.Invoke();
        }
    }
}
