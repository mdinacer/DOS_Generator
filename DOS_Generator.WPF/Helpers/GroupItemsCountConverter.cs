using System;
using System.Globalization;
using System.Windows.Data;

namespace DOS_Generator.WPF.Helpers
{
    public class GroupItemsCountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is int count)) return null;
            if (!(parameter is string param)) return null;

            return param switch
            {
                "Text" => count switch
                {
                    0 => "No declarations",
                    1 => "One declaration",
                    _ => $"{count} declarations"
                },
                _ => count
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}