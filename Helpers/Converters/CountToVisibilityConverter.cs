using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace InventoryManagementSystem.Helpers.Converters
{
    [ValueConversion(typeof(int), typeof(Visibility))]
    public class CountToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int count)
            {
                // count == 0 → show message (Visible)
                // count > 0  → hide message (Collapsed)
                return count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Visible; // fallback
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}