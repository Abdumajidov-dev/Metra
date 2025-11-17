using System.Globalization;
using System.Windows.Data;

namespace Metra.Desktop.Converters;

/// <summary>
/// DateTime ni string formatga aylantiradi
/// </summary>
public class DateTimeConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTime dateTime)
        {
            string format = parameter as string ?? "dd.MM.yyyy";
            return dateTime.ToString(format, culture);
        }
        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string stringValue && DateTime.TryParse(stringValue, culture, DateTimeStyles.None, out var result))
        {
            return result;
        }
        return null;
    }
}
