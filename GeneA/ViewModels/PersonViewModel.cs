using CommunityToolkit.Mvvm.ComponentModel;
using GeneA._Helper;
using GeneA._Services;
using Microsoft.CodeAnalysis.CSharp;
using Model.Core;
using Model.Interfaces;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        SelectedGender = new Gender();

        Load().SafeFireAndForget();
    }

    //TODO: DateTime binded in TextBox dont work, try using string(create string property in ViewModel, it will receive the 
    //"d" format from DateTime, dont use IValueConverter

    private readonly IRepository<Person> _repository;

    [ObservableProperty]
    private Person? _person;

    public IEnumerable<Gender> Genders { get; set; } = StaticList.FillGenders();

    [ObservableProperty]
    private Gender _selectedGender;

    //workaround to binding DateTime with avalonia TextBox, when its fixed undo this and bind directly DateTime with
    //MaskedTextBox

    [ObservableProperty]
    private ObservableRangeCollection<Person> _fatherList;

    [ObservableProperty]
    private ObservableRangeCollection<Person> _motherList;

    private async Task Load()
    {
        await Task.Run(() =>
        {
            if (Param == null)
            {
                Person = new Person();
            }
            else
            {
                Person = _repository.FindById((long)Param);

                //workaround to binding DateTime with avalonia TextBox, when its fixed undo this and bind directly DateTime with
                //MaskedTextBox

                //TODO: view is not showing the SelectedItem in combobox
                SelectedGender = Person.Gender.ToGenderTypes();
            }

            //TODO: odd bug happening here, no matter what selecting any person for the second time results into
            //null param, look more into GoAsync method in navigation

            var people = _repository.FindAll();

            FatherList.ReplaceRange(people.Where(p => p.Gender == ModelA.Enums.GenderEnum.Gender.Male && p.Id != Person.Id));
            MotherList.ReplaceRange(people.Where(p => p.Gender == ModelA.Enums.GenderEnum.Gender.Female && p.Id != Person.Id));

            //FatherList.Remove(Person);
            //MotherList.Remove(Person);
        });
    }

    public async Task SaveCommand()
    {
        await Task.Run(() =>
        {
            //TODO: validate(data annotations)


            //workaround to binding DateTime with avalonia TextBox, when its fixed undo this and bind directly DateTime with
            //MaskedTextBox
            if (SelectedGender != null)
                Person!.Gender = SelectedGender.GenderEnum;

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
