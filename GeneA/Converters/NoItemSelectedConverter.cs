using Avalonia.Data;
using Avalonia.Data.Converters;
using GeneA.Interfaces;
using GeneA.ViewModelItems;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneA.Converters
{
    public class NoItemSelectedConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is IEnumerable<ISelectable> people)
                return people.Any(p => p.IsSelected);

            return BindingOperations.DoNothing;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
