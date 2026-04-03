using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Contacts.Model;
using Contacts.Model.Services;

namespace Contacts.ViewModel
{
    /// <summary>
    /// Представляет основную модель представления приложения контактов.
    /// Обеспечивает привязку данных, валидацию и работу с командами сохранения и загрузки.
    /// </summary>
    public class MainVM : INotifyPropertyChanged, IDataErrorInfo
    {
        private readonly ContactSerializer _serializer;

        /// <summary>
        /// Получает текущий объект контакта, связанный с моделью представления.
        /// </summary>
        public Contact Contact { get; private set; }

        /// <summary>
        /// Получает общее сообщение об ошибке валидации.
        /// </summary>
        public string Error => string.Empty;

        /// <summary>
        /// Возвращает сообщение об ошибке для указанного свойства.
        /// </summary>
        /// <param name="columnName">Имя свойства, для которого требуется проверить корректность значения.</param>
        /// <returns>Текст ошибки валидации или пустая строка, если значение корректно.</returns>
        public string this[string columnName]
        {
            get
            {
                return columnName switch
                {
                    nameof(Name) => ValidateName(),
                    nameof(PhoneNumber) => ValidatePhone(),
                    nameof(Email) => ValidateEmail(),
                    _ => string.Empty,
                };
            }
        }

        /// <summary>
        /// Получает значение, указывающее, прошли ли все поля модели представления проверку валидации.
        /// </summary>
        public bool IsValid =>
            string.IsNullOrWhiteSpace(ValidateName())
            && string.IsNullOrWhiteSpace(ValidatePhone())
            && string.IsNullOrWhiteSpace(ValidateEmail());

        /// <summary>
        /// Проверяет корректность имени контакта.
        /// </summary>
        /// <returns>Сообщение об ошибке или пустая строка при успешной проверке.</returns>
        private string ValidateName()
        {
            if (string.IsNullOrWhiteSpace(Name))
                return "Имя не должно быть пустым.";
            if (Name.Trim().Length < 2)
                return "Имя слишком короткое.";
            return string.Empty;
        }

        /// <summary>
        /// Проверяет корректность номера телефона.
        /// </summary>
        /// <returns>Сообщение об ошибке или пустая строка при успешной проверке.</returns>
        private string ValidatePhone()
        {
            if (string.IsNullOrWhiteSpace(PhoneNumber))
                return "Телефон не должен быть пустым.";

            var ok = Regex.IsMatch(PhoneNumber, @"^[0-9\+\-\s\(\)]{5,}$");
            return ok ? string.Empty : "Телефон содержит недопустимые символы.";
        }

        /// <summary>
        /// Проверяет корректность адреса электронной почты.
        /// </summary>
        /// <returns>Сообщение об ошибке или пустая строка при успешной проверке.</returns>
        private string ValidateEmail()
        {
            if (string.IsNullOrWhiteSpace(Email))
                return "Email не должен быть пустым.";

            var ok = Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return ok ? string.Empty : "Некорректный формат email.";
        }

        private string _name = string.Empty;

        /// <summary>
        /// Получает или задаёт имя контакта, используемое в привязке данных.
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                if (_name == value)
                    return;
                _name = value;
                Contact.Name = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsValid));
                RaiseCanExecuteChanged();
            }
        }

        private string _phoneNumber = string.Empty;

        /// <summary>
        /// Получает или задаёт номер телефона контакта, используемый в привязке данных.
        /// </summary>
        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                if (_phoneNumber == value)
                    return;
                _phoneNumber = value;
                Contact.PhoneNumber = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsValid));
                RaiseCanExecuteChanged();
            }
        }

        private string _email = string.Empty;

        /// <summary>
        /// Получает или задаёт адрес электронной почты контакта, используемый в привязке данных.
        /// </summary>
        public string Email
        {
            get => _email;
            set
            {
                if (_email == value)
                    return;
                _email = value;
                Contact.Email = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsValid));
                RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Получает команду сохранения текущего контакта.
        /// </summary>
        public ICommand SaveCommand { get; }

        /// <summary>
        /// Получает команду загрузки контакта из файла.
        /// </summary>
        public ICommand LoadCommand { get; }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="MainVM"/>.
        /// </summary>
        public MainVM()
        {
            _serializer = new ContactSerializer();
            Contact = _serializer.Load();

            ApplyContact(_serializer.Load());

            SaveCommand = new SaveCommand(this, _serializer);
            LoadCommand = new LoadCommand(this, _serializer);
        }

        /// <summary>
        /// Применяет данные указанного контакта к модели представления и обновляет связанные свойства.
        /// </summary>
        /// <param name="contact">Контакт, данные которого необходимо отобразить.</param>
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

        /// <summary>
        /// Уведомляет команды о необходимости повторной проверки доступности выполнения.
        /// </summary>
        private void RaiseCanExecuteChanged()
        {
            if (SaveCommand is SaveCommand sc)
                sc.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Происходит при изменении значения свойства модели представления.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Вызывает событие <see cref="PropertyChanged"/> для указанного свойства.
        /// </summary>
        /// <param name="propName">Имя изменившегося свойства.</param>
        private void OnPropertyChanged([CallerMemberName] string? propName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
