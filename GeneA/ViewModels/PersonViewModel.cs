using CommunityToolkit.Mvvm.ComponentModel;
using GeneA._Services;
using Microsoft.CodeAnalysis.CSharp;
using Model.Core;
using Model.Interfaces;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneA.ViewModels;

public partial class PersonViewModel : ViewModelBase
{
    public PersonViewModel(IRepository<Person> repository)
    {
        _repository = repository;

        Load().SafeFireAndForget();
    }

    private readonly IRepository<Person> _repository;

    [ObservableProperty]
    private Person? _person;

    private async Task Load()
    {
        await Task.Run(() =>
        {
            Person = Param == null
            ? new Person()
            : _repository.FindById((long)Param);
        });
    }

    public async Task SaveCommand()
    {
        throw new NotImplementedException();
        await Task.Run(() => 
        {
            //TODO: validate(data annotations)

            _repository.Upsert(Person!);

            //TODO: show success popup
        });
    }

    public async Task DeleteCommand()
    {
        throw new NotImplementedException();
        await Task.Run(() => 
        {
            //TODO: show popup confirmation
            _repository.Delete(Person!);
            //TODO: show popup success
        });
    }
}
//TODO:implement features for add, edit and delete one person