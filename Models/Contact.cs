using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Contacts.Model
{
    /// <summary>
    /// Представляет контакт с основными данными пользователя.
    /// Реализует проверку вводимых данных через <see cref="INotifyDataErrorInfo"/>.
    /// </summary>
    public class Contact : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        /// <summary>
        /// Максимально допустимая длина текстовых полей контакта.
        /// </summary>
        private const int MaxLength = 100;

        private string _name;
        private string _firstName;
        private string _lastName;
        private string _phone;
        private string _email;

        private readonly Dictionary<string, List<string>> _errors = new();

        /// <summary>
        /// Получает или задаёт имя контакта.
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                    ValidateName();
                }
            }
        }

        /// <summary>
        /// Получает или задаёт номер телефона контакта.
        /// </summary>
        public string PhoneNumber
        {
            get => _phone;
            set
            {
                if (_phone != value)
                {
                    _phone = value;
                    OnPropertyChanged();
                    ValidatePhoneNumber();
                }
            }
        }

        /// <summary>
        /// Получает или задаёт адрес электронной почты контакта.
        /// </summary>
        public string Email
        {
            get => _email;
            set
            {
                if (_email != value)
                {
                    _email = value;
                    OnPropertyChanged();
                    ValidateEmail();
                }
            }
        }

        /// <summary>
        /// Проверяет, является ли символ допустимым для номера телефона.
        /// </summary>
        /// <param name="symbol">Проверяемый символ.</param>
        /// <returns>true, если символ можно ввести в номер телефона.</returns>
        public static bool IsPhoneCharAllowed(char symbol)
        {
            return char.IsDigit(symbol)
                || symbol == '+'
                || symbol == '-'
                || symbol == '('
                || symbol == ')'
                || symbol == ' ';
        }

        private void ValidateName()
        {
            var errors = new List<string>();
            if (!string.IsNullOrEmpty(_name) && _name.Length > MaxLength)
            {
                errors.Add($"Имя должно быть не длиннее {MaxLength} символов.");
            }

            SetErrors(nameof(Name), errors);
        }

        private void ValidatePhoneNumber()
        {
            var errors = new List<string>();
            var phone = _phone ?? string.Empty;

            if (phone.Length > MaxLength)
            {
                errors.Add($"Номер телефона должен быть не длиннее {MaxLength} символов.");
            }

            if (phone.Any(symbol => !IsPhoneCharAllowed(symbol)))
            {
                errors.Add(
                    "Номер телефона может содержать только цифры и символы + - ( ). "
                        + "Пример: +7 (999) 111-22-33"
                );
            }

            SetErrors(nameof(PhoneNumber), errors);
        }

        private void ValidateEmail()
        {
            var errors = new List<string>();
            var email = _email ?? string.Empty;

            if (email.Length > MaxLength)
            {
                errors.Add($"Email должен быть не длиннее {MaxLength} символов.");
            }

            if (!email.Contains('@'))
            {
                errors.Add("Email должен содержать символ @.");
            }

            SetErrors(nameof(Email), errors);
        }

        private void SetErrors(string propertyName, List<string> errors)
        {
            if (errors.Count > 0)
            {
                _errors[propertyName] = errors;
            }
            else
            {
                _errors.Remove(propertyName);
            }

            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            OnPropertyChanged(nameof(HasErrors));
        }

        /// <summary>
        /// Возвращает значение, показывающее, есть ли у контакта ошибки проверки.
        /// </summary>
        public bool HasErrors => _errors.Count > 0;

        /// <summary>
        /// Событие, возникающее при изменении набора ошибок проверки.
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// Возвращает ошибки проверки для указанного свойства.
        /// </summary>
        /// <param name="propertyName">Имя свойства или null для всех ошибок.</param>
        /// <returns>Список сообщений об ошибках.</returns>
        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return _errors.SelectMany(pair => pair.Value);
            }

            return _errors.TryGetValue(propertyName, out var errors)
                ? errors
                : Enumerable.Empty<string>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Инициализирует новый пустой экземпляр класса <see cref="Contact"/>.
        /// </summary>
        public Contact()
        {
            Name = string.Empty;
            PhoneNumber = string.Empty;
            Email = string.Empty;
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="Contact"/> с указанными данными.
        /// </summary>
        /// <param name="name">Имя контакта.</param>
        /// <param name="phoneNumber">Номер телефона контакта.</param>
        /// <param name="email">Адрес электронной почты контакта.</param>
        public Contact(string name, string phoneNumber, string email)
        {
            Name = name;
            PhoneNumber = phoneNumber;
            Email = email;
        }
    }
}
