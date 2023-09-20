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

        FatherList = new ObservableRangeCollection<Person>();
        MotherList = new ObservableRangeCollection<Person>();

        Load().SafeFireAndForget();
    }

    //TODO: DateTime binded in TextBox dont work, try using string(create string property in ViewModel, it will receive the 
    //"d" format from DateTime, dont use IValueConverter

    private readonly IRepository<Person> _repository;

    [ObservableProperty]
    private Person? _person;

    [ObservableProperty]
    private ObservableRangeCollection<Person> _fatherList;

    [ObservableProperty]
    private ObservableRangeCollection<Person> _motherList;

    private async Task Load()
    {
        await Task.Run(() =>
        {
            var people = _repository.FindAll();
            
            FatherList.ReplaceRange(people.Where(p => p.Gender == ModelA.Enums.GenderEnum.Gender.Male));
            MotherList.ReplaceRange(people.Where(p => p.Gender == ModelA.Enums.GenderEnum.Gender.Female));

            if (Param == null)
            {
                Person = new Person();
            }
            else
            {
                Person = _repository.FindById((long)Param);
            }

            FatherList.Remove(Person);
            MotherList.Remove(Person);
        });
    }

    public async Task SaveCommand()
    {
       // throw new NotImplementedException();
        await Task.Run(() =>
        {
            //TODO: validate(data annotations)

         //   _repository.Upsert(Person!);

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
