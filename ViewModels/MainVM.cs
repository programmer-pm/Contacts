// ViewModel/MainVM.cs
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Lab2Mvvm.Model;
using Lab2Mvvm.Model.Services;

namespace Lab2Mvvm.ViewModel
{
    public class MainVM : INotifyPropertyChanged
    {
        private readonly ContactSerializer _serializer;

        public Contact Contact { get; private set; }

        private string _name = "";
        public string Name
        {
            get => _name;
            set
            {
                if (_name == value) return;
                _name = value;
                Contact.Name = value;
                OnPropertyChanged();
            }
        }

        private string _phoneNumber = "";
        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                if (_phoneNumber == value) return;
                _phoneNumber = value;
                Contact.PhoneNumber = value;
                OnPropertyChanged();
            }
        }

        private string _email = "";
        public string Email
        {
            get => _email;
            set
            {
                if (_email == value) return;
                _email = value;
                Contact.Email = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand LoadCommand { get; }

        public MainVM()
        {
            Contact = new Contact("", "", "");
            _serializer = new ContactSerializer();

            SaveCommand = new SaveCommand(this, _serializer);
            LoadCommand = new LoadCommand(this, _serializer);
        }

        public void ApplyContact(Contact contact)
        {
            Contact = contact;

            // обновляем свойства (и UI через уведомления)
            _name = Contact.Name ?? "";
            _phoneNumber = Contact.PhoneNumber ?? "";
            _email = Contact.Email ?? "";

            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(PhoneNumber));
            OnPropertyChanged(nameof(Email));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}