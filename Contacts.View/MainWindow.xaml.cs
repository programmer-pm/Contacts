using System.Windows;
using Contacts.Converters;
using Contacts.ViewModel;

namespace Contacts
{
    public partial class MainWindow : Window
    {
        private MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            _viewModel = new MainViewModel();
            DataContext = _viewModel;

            // Опционально: автосохранение при закрытии окна
            this.Closing += (s, e) => {
                // ContactsSerializer.SaveContacts(_viewModel.Contacts);
            };
        }
    }
}
