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
    public class FamilyTreeViewModel : ViewModelBase
    {
        public FamilyTreeViewModel(IRepository<Person> personRepository)
        {
            _personRepository = personRepository;

            LoadAction = () => { Load().SafeFireAndForget(); };
        }

        private readonly IRepository<Person> _personRepository;

        private async Task Load()
        {
            long id = (long)Param!;

            Person person = _personRepository.FindById(id);

            //TODO: create a tree builder helper and return a compatible data to avalonia Graph property
        }
    }
}
