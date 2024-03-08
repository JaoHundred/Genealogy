using AvaloniaGraphControl;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GeneA._Helper;
using GeneA._Services;
using GeneA.ViewModelItems;
using GeneA.Views;
using Model.Interfaces;
using ModelA.Core;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneA.ViewModels
{
    public partial class FamilyTreeViewModel : ViewModelBase
    {
        public FamilyTreeViewModel(GraphService graphService, NavigationService navigationService, FileService fileService)
        {
            _graphService = graphService;
            _navigationService = navigationService;
            _fileService = fileService;

            LoadAction = () => { Load().SafeFireAndForget(); };
        }
        
        private readonly GraphService _graphService;
        private readonly NavigationService _navigationService;
        private readonly FileService _fileService;

        [ObservableProperty]
        private Graph? _graph;

        private PersonItemViewModel? _person;

        private async Task Load()
        {
            _person = (PersonItemViewModel)Param!;

            //TODO: panzoom dont work with pinch gesture in android, see how should this will be implemented

            Graph = await _graphService.ToGraphAsync(_person, _person.Generations.GetValueOrDefault());
        }

        [RelayCommand]
        private async Task OpenPerson(Person person)
        {
            await _navigationService.GoToAsync<PersonViewModel>(person.Id);
        }

        [RelayCommand]
        private async Task SaveGraphAsImage()
        {
            await _fileService.SaveGraphImageAsync($"{_person!.Name} family.png");
        }
    }
}
