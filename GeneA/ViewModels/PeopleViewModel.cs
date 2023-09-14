using CommunityToolkit.Mvvm.ComponentModel;
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
    public partial class PeopleViewModel : ViewModelBase
    {
        public PeopleViewModel(IRepository<Person> repository)
        {
            _repository = repository;

            People = new ObservableRangeCollection<Person>();

            Load().SafeFireAndForget();
        }

        private readonly IRepository<Person> _repository;

        [ObservableProperty]
        private ObservableRangeCollection<Person> _people;
        public async Task Load()
        {
            await Task.Run(() =>
            {
                People.ReplaceRange(_repository.FindAll());
            });
        }
    }
}
