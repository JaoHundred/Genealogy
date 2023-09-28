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
using System.Threading;
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
    private Person? _selectedFather;

    [ObservableProperty]
    private Person? _selectedMother;

    [ObservableProperty]
    private List<Person>? _fatherList;

    [ObservableProperty]
    private List<Person>? _motherList;

    private async Task Load()
    {
        await Task.Run(() =>
        {
            var people = _repository.FindAll();

            var fatherList = people.Where(p => p.Gender == ModelA.Enums.GenderEnum.Gender.Male).ToList();
            var motherList = people.Where(p => p.Gender == ModelA.Enums.GenderEnum.Gender.Female).ToList();

            if (Param == null)
            {
                Person = new Person();
            }
            else
            {
                Person = _repository.FindById((long)Param);

                SelectedGender = Genders.FirstOrDefault(p => p.GenderEnum == Person.Gender)!;
                SelectedFather = fatherList.FirstOrDefault(p => p.Id == Person.Father?.Id);
                SelectedMother = motherList.FirstOrDefault(p => p.Id == Person.Mother?.Id);

                if (Person.Gender == ModelA.Enums.GenderEnum.Gender.Male)
                    fatherList.RemoveAll(p => p.Id == Person.Id);
                else
                    motherList.RemoveAll(p => p.Id == Person.Id);
            }

            FatherList = fatherList;
            MotherList = motherList;
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

            if (SelectedMother != null)
                Person!.Mother = SelectedMother;

            if(SelectedFather != null)
                Person!.Father = SelectedFather;

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

    //public async Task<IEnumerable<object>> PopulateFatherAsync(string str, CancellationToken token)
    //{
    //    return await Task.Run(() =>
    //    {
    //        var list = FatherList!.Where(p => p.Name.ToLower().StartsWith(str.ToLower()));
    //        return list;
    //    });
    //}
}
