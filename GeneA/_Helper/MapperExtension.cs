using GeneA.ViewModelItems;
using GeneA.ViewModels;
using Model.Core;
using ModelA.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneA._Helper;

public static class MapperExtension
{
    public static Gender ToGenderTypes(this GenderEnum.Gender genderEnum)
    {
        return new Gender
        {
            GenderEnum = genderEnum,
            Name = DynamicTranslate.Translate(genderEnum.ToString()),
        };
    }

    public static IEnumerable<PersonItemViewModel> ToPersonItemViewModels(this IEnumerable<Person> people, bool isSelected = true)
    {
        return people.Select(p => new PersonItemViewModel
        {
            BaptismDate = p.BaptismDate,
            BirthDate = p.BirthDate,
            DeathDate = p.DeathDate,
            Description = p.Description,
            Father = p.Father,
            Mother = p.Mother,
            Gender = p.Gender,
            Id = p.Id,
            IsSelected = isSelected,
            Nationality = p.Nationality,
            Name = p.Name,
            Offsprings = p.Offsprings,
            Spouses = p.Spouses,
            WeddingDate = p.WeddingDate,
        });
    }

    public static IEnumerable<Person> ToPeople(this IEnumerable<PersonItemViewModel> people)
    {
        return people.Select(p => new Person
        {
            BaptismDate = p.BaptismDate,
            BirthDate = p.BirthDate,
            DeathDate = p.DeathDate,
            Description = p.Description,
            Father = p.Father,
            Mother = p.Mother,
            Gender = p.Gender,
            Id = p.Id,
            Nationality = p.Nationality,
            Name = p.Name,
            Offsprings = p.Offsprings,
            Spouses = p.Spouses,
            WeddingDate = p.WeddingDate,
        });
    }

    public static IEnumerable<NationalityItemViewModel> ToNationalityItemViewModels(this IEnumerable<Nationality> nationalities)
    {
        return nationalities.Select(p => new NationalityItemViewModel
        {
            Abbreviation = p.Abbreviation,
            Id = p.Id,
            Name = p.Name,
            IsSelected = false
        });
    }

    public static Nationality ToNationality(this NationalityItemViewModel nationality)
    {
        return new Nationality
        {
            Abbreviation = nationality.Abbreviation,
            Id = nationality.Id,
            Name = nationality.Name,
        };
    }

    public static NationalityItemViewModel ToNationalityItemViewModel(this Nationality nationality)
    {
        return new NationalityItemViewModel
        {
            Abbreviation = nationality.Abbreviation,
            Id = nationality.Id,
            Name = nationality.Name,
            IsSelected = false,
        };
    }
}
