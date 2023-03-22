using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace MCLauncher.SettingsWindow;

public enum LauncherVisibility
{
    KeepOpen,
    Close,
    Hide
}

public sealed class EnumToStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
        {
            return null;
        }

        return value.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var str = (string)value;

        foreach (object enumValue in Enum.GetValues(targetType))
        {
            if (str == enumValue.ToString())
            {
                return enumValue;
            }
        }

        throw new ArgumentException(null, "value");
    }
}

public sealed class EnumerateExtension : MarkupExtension
{
    public Type Type { get; set; }

    public EnumerateExtension(Type type)
    {
        this.Type = type;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        var names = Enum.GetNames(Type);
        var values = new string[names.Length];

        for (int i = 0; i < names.Length; i++)
        {
            values[i] = names[i];
        }

        return values;
    }
}