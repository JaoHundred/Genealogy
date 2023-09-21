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
    #region workaround
    [DisplayFormat(DataFormatString = "00/00/0000")]
    [DataType(DataType.Date)]
    public string BirthDateString { get; set; }

    [DisplayFormat(DataFormatString = "00/00/0000")]
    [DataType(DataType.Date)]
    public string DeathDateString { get; set; }

    [DisplayFormat(DataFormatString = "00/00/0000")]
    [DataType(DataType.Date)]
    public string WeddingString { get; set; }
    #endregion

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

                //workaround to binding DateTime with avalonia TextBox, when its fixed undo this and bind directly DateTime with
                //MaskedTextBox
                if (Person.BirthDate.HasValue)
                {
                    BirthDateString = Person.BirthDate.Value.ToString("d");
                }
                if (Person.DeathDate.HasValue)
                {
                    DeathDateString = Person.DeathDate.Value.ToString("d");
                }
                if (Person.WeddingDate.HasValue)
                {
                    WeddingString = Person.WeddingDate.Value.ToString("d");
                }

                SelectedGender = Person.Gender.ToGenderTypes();
            }

            FatherList.Remove(Person);
            MotherList.Remove(Person);
        });
    }

    public async Task SaveCommand()
    {
        await Task.Run(() =>
        {
            //TODO: validate(data annotations)


            //workaround to binding DateTime with avalonia TextBox, when its fixed undo this and bind directly DateTime with
            //MaskedTextBox
            if (DateTime.TryParse(BirthDateString, out var bDate))
                Person!.BirthDate = bDate;

            if (DateTime.TryParse(DeathDateString, out var dDate))
                Person!.DeathDate = dDate;

            if (DateTime.TryParse(WeddingString, out var wDate))
                Person!.WeddingDate = wDate;

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
