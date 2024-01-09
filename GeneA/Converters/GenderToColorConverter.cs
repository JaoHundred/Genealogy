using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;
using ModelA.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneA.Converters
{
    public class GenderToColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is GenderEnum.Gender gender)
                return gender == GenderEnum.Gender.Male ? 
                    SolidColorBrush.Parse("LightBlue") : new SolidColorBrush(Color.FromRgb(226, 191, 207));//pink

            return BindingOperations.DoNothing;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
