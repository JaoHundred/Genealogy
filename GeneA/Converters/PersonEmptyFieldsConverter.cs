using Avalonia.Data;
using Avalonia.Data.Converters;
using ModelA.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneA.Converters
{
    public class PersonEmptyFieldsConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is Person person)
            {
                return
                    person.BirthDate == null
                    || person.DeathDate == null
                    || person.BaptismDate == null
                    || person.WeddingDate == null
                    || person.Father == null
                    || person.Mother == null
                    || person.Spouses == null
                    || person.Nationality == null
                    || person.Spouses?.Count == 0
                    || person.Offsprings?.Count == 0
                    || person.Description.Length == 0
                    ;
            }

            return BindingOperations.DoNothing;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
