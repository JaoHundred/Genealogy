using AvaloniaGraphControl;
using CommunityToolkit.Mvvm.ComponentModel;
using GeneA._Helper;
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
        public FamilyTreeViewModel(IRepository<Person> personRepository)
        {
            _personRepository = personRepository;

            LoadAction = () => { Load().SafeFireAndForget(); };
        }

        private readonly IRepository<Person> _personRepository;


        [ObservableProperty]
        private Graph? _graph;

        private async Task Load()
        {
            Person person = (Person)Param!;

            //TODO: create a tree builder helper and return a compatible data to avalonia Graph property

            Graph = person.ToGraph();
        }
    }
}
