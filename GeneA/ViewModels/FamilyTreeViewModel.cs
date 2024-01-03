using AvaloniaGraphControl;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GeneA._Helper;
using GeneA._Services;
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
        public FamilyTreeViewModel(GraphService graphService, NavigationService navigationService)
        {
            _graphService = graphService;
            _navigationService = navigationService;

            LoadAction = () => { Load().SafeFireAndForget(); };
        }
        
        private readonly GraphService _graphService;
        private readonly NavigationService _navigationService;

        [ObservableProperty]
        private Graph? _graph;

        private async Task Load()
        {
            Person person = (Person)Param!;
            //TODO: panzoom dont work with pinch gesture in android, see how should this will be implemented

            Graph = await _graphService.ToGraphAsync(person);
        }

        [RelayCommand]
        private async Task OpenPerson(Person person)
        {
            await _navigationService.GoToAsync<PersonViewModel>(person.Id);
        }
    }
}
