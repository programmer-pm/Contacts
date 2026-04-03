using System.Windows;
using Contacts.ViewModel;

namespace Contacts;

/// <summary>
/// Представляет главное окно приложения для просмотра и редактирования контактных данных.
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// Инициализирует новый экземпляр окна <see cref="MainWindow"/> и назначает модель представления.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainVM();
    }
}
