using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GeneA._Helper;
using GeneA._Services;
using Microsoft.CodeAnalysis.CSharp;
using Model.Core;
using Model.Interfaces;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        LoadAction = () => { Load().SafeFireAndForget(); };
    }

    private readonly IRepository<Person> _repository;

    [ObservableProperty]
    private Person? _person;

    public IEnumerable<Gender> Genders { get; set; } = StaticList.FillGenders();

    [ObservableProperty]
    private Gender? _selectedGender;

    [ObservableProperty]
    private Gender? _selectedFather;

    [ObservableProperty]
    private Gender? _selectedMother;

    [ObservableProperty]
    private ObservableRangeCollection<Person>? _fatherList;

    [ObservableProperty]
    private ObservableRangeCollection<Person>? _motherList;

    private async Task Load()
    {
        await Task.Run(() =>
        {
            FatherList = new ObservableRangeCollection<Person>();
            MotherList = new ObservableRangeCollection<Person>();

            if (Param == null)
            {
                Person = new Person();
            }
            else
            {
                Person = _repository.FindById((long)Param);

                SelectedGender = Genders.FirstOrDefault(p => p.GenderEnum == Person.Gender)!;
                
            }

            var people = _repository.FindAll();
            
            var fatherList = people.Where(p => p.Gender == ModelA.Enums.GenderEnum.Gender.Male && p.Id != Person.Id);
            var motherList = people.Where(p => p.Gender == ModelA.Enums.GenderEnum.Gender.Female && p.Id != Person.Id);

            //TODO:System.InvalidOperationException: 'Call from invalid thread' when going select on list after add new
            //(new -> home -> list click -> InvalidOperationException)
            FatherList.ReplaceRange(fatherList);
            MotherList.ReplaceRange(motherList);
        });
    }

    [RelayCommand]
    private async Task Save()
    {
        await Task.Run(() =>
        {
            //TODO: add validations here

            if (SelectedGender != null)
                Person!.Gender = SelectedGender.GenderEnum;

            _repository.Upsert(Person!);

            //TODO: show success popup
        });
    }

    [RelayCommand]
    private async Task Delete()
    {
        await Task.Run(() =>
        {
            //TODO: show popup confirmation
            _repository.Delete(Person!);
            //TODO: show popup success
        });
    }
}
