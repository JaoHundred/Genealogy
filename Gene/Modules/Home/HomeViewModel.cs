using Model.Core;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Gene.Modules.Home
{
    public partial class HomeViewModel : ObservableObject
    {
        //TODO: add localization to view, en-us and pr-br

        public HomeViewModel(IRepository<Person> personRepository)
        {
            personRepository = _personRepository;
        }

        private readonly IRepository<Person> _personRepository;

        [ObservableProperty]
        private string textToButton;

        [RelayCommand]
        private async Task ButtonTest()
        {
            await Task.Run(() => { TextToButton = "clicked"; });
        }
    }
}
