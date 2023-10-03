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
    public PersonViewModel(IRepository<Person> repository, NavigationService navigation)
    {
        _repository = repository;
        _navigation = navigation;

        LoadAction = () => { Load().SafeFireAndForget(); };
    }

    private readonly IRepository<Person> _repository;
    private readonly NavigationService _navigation;

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

                //constraints to dont show offsprings in father/mother list of father/mother
                fatherList = fatherList.ExceptBy(Person.Offsprings.Select(p => p.Id), q => q.Id).ToList();
                motherList = motherList.ExceptBy(Person.Offsprings.Select(p => p.Id), q => q.Id).ToList();

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

            if (SelectedFather != null)
                Person!.Father = SelectedFather;

            _repository.Upsert(Person!);

            //TODO: show success popup
        });
    }

    [RelayCommand]
    private async Task Delete()
    {
        await Task.Run(async () =>
        {
            string title = DynamicTranslate.Translate(MessageConsts.ConfirmationDialogTitle);
            string message = DynamicTranslate.Translate(MessageConsts.Confirmation);

            Action confirm = async () =>
            {
                _repository.Delete(Person!);

                await _navigation.GoBackAsync();//close popup
                await _navigation.GoBackAsync();//back to HomeView
            };
            Action cancel = async () =>
            {
                await _navigation.GoBackAsync();
            };

            await _navigation.PopUpAsync<ConfirmationPopupViewModel>()
            .ConfigurePopUpProperties(title, message, confirm, cancel);
        });
    }

    public async Task<IEnumerable<object>> FatherStartsWithAsync(string str, CancellationToken token)
    {
        return await Task.Run(() =>
        {
            var list = FatherList!.Where(p => p.Name.ToLower().StartsWith(str.ToLower()));
            return list;
        });
    }

    public async Task<IEnumerable<object>> MotherStartsWithAsync(string str, CancellationToken token)
    {
        return await Task.Run(() =>
        {
            var list = MotherList!.Where(p => p.Name.ToLower().StartsWith(str.ToLower()));
            return list;
        });
    }
}
