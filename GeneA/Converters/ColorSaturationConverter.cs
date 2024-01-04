using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace GeneA.Converters
{
    public class ColorSaturationConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if(value is SolidColorBrush solidColor)
            {
                var hsla = solidColor.Color.ToHsl();
                var newColor = new HslColor(hsla.A, hsla.H, hsla.S * 0.8, hsla.L).ToRgb();

                //TODO:color is blinking, see what is happening
               return new SolidColorBrush(newColor);
            }

            return BindingOperations.DoNothing;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
