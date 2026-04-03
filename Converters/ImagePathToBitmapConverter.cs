using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace ContactsApp.Converters;

public class ImagePathToBitmapConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        /// <summary>
        /// parameter = путь к placeholder в ресурсах
        /// </summary>
        var placeholder = parameter as string ?? "";

        string? path = value as string;
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            return LoadPackImage(placeholder);

        try
        {
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.UriSource = new Uri(path, UriKind.Absolute);
            bmp.EndInit();
            bmp.Freeze();
            return bmp;
        }
        catch
        {
            return LoadPackImage(placeholder);
        }
    }

    private static BitmapImage LoadPackImage(string packPath)
    {
        /// <summary>
        /// packPath пример: "pack://application:,,,/Assets/photo_placeholder_100x100.png"
        /// </summary>
        var bmp = new BitmapImage();
        bmp.BeginInit();
        bmp.UriSource = new Uri(packPath, UriKind.Absolute);
        bmp.EndInit();
        bmp.Freeze();
        return bmp;
    }

    public object ConvertBack(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture
    ) => Binding.DoNothing;
}
