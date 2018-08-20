using AccountHelper.Models;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AccountHelper.Utilities
{
    class AccountToVisibilityConverter : IValueConverter
    {
        public Visibility FalseVisibility { get; set; }
        public AccountToVisibilityConverter()
        {
            FalseVisibility = Visibility.Collapsed;
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Account)
            {
                return Visibility.Visible;
            }
            return FalseVisibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
