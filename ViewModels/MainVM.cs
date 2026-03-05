// ViewModel/MainVM.cs
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Lab2Mvvm.Model;
using Lab2Mvvm.Model.Services;

namespace Lab2Mvvm.ViewModel
{
    public class MainVM : INotifyPropertyChanged, IDataErrorInfo
    {
        private readonly ContactSerializer _serializer;
        public Contact Contact { get; private set; }

        // --- ВАЛИДАЦИЯ ---
        public string Error => string.Empty;

        public string this[string columnName]
        {
            get
            {
                return columnName switch
                {
                    nameof(Name) => ValidateName(),
                    nameof(PhoneNumber) => ValidatePhone(),
                    nameof(Email) => ValidateEmail(),
                    _ => string.Empty
                };
            }
        }

        public bool IsValid =>
            string.IsNullOrWhiteSpace(ValidateName()) &&
            string.IsNullOrWhiteSpace(ValidatePhone()) &&
            string.IsNullOrWhiteSpace(ValidateEmail());

        private string ValidateName()
        {
            if (string.IsNullOrWhiteSpace(Name))
                return "Имя не должно быть пустым.";
            if (Name.Trim().Length < 2)
                return "Имя слишком короткое.";
            return string.Empty;
        }

        private string ValidatePhone()
        {
            if (string.IsNullOrWhiteSpace(PhoneNumber))
                return "Телефон не должен быть пустым.";

            // Разрешим: цифры, пробелы, +, -, (, )
            var ok = Regex.IsMatch(PhoneNumber, @"^[0-9\+\-\s\(\)]{5,}$");
            return ok ? string.Empty : "Телефон содержит недопустимые символы.";
        }

        private string ValidateEmail()
        {
            if (string.IsNullOrWhiteSpace(Email))
                return "Email не должен быть пустым.";

            // Простая проверка для лаб. (не RFC)
            var ok = Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return ok ? string.Empty : "Некорректный формат email.";
        }

        // --- СВОЙСТВА ДЛЯ BINDING ---
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
                OnPropertyChanged(nameof(IsValid));
                RaiseCanExecuteChanged();
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
                OnPropertyChanged(nameof(IsValid));
                RaiseCanExecuteChanged();
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
                OnPropertyChanged(nameof(IsValid));
                RaiseCanExecuteChanged();
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

            _name = Contact.Name ?? "";
            _phoneNumber = Contact.PhoneNumber ?? "";
            _email = Contact.Email ?? "";

            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(PhoneNumber));
            OnPropertyChanged(nameof(Email));
            OnPropertyChanged(nameof(IsValid));
            RaiseCanExecuteChanged();
        }

        private void RaiseCanExecuteChanged()
        {
            if (SaveCommand is SaveCommand sc) sc.RaiseCanExecuteChanged();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}