using GeneA.ViewModelItems;
using GeneA.ViewModels;
using Model.Core;
using ModelA.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneA._Helper
{
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

        public static IEnumerable<PersonItemViewModel> ToPersonItemViewModels(this IEnumerable<Person> people)
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
                IsSelected = true,
                Nacionality = p.Nacionality,
                Name = p.Name,
                Offsprings = p.Offsprings,
                Spouses = p.Spouses,
                WeddingDate = p.WeddingDate,
            });
        }
    }
}
