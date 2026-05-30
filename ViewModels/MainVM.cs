using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Contacts.Commands;
using Contacts.Model;
using Contacts.Model.Services;

namespace Contacts.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Contact> _contacts;
        private Contact _selectedContact;
        private Contact _editingContact;
        private bool _isAdding;
        private bool _isEditing;

        // Команды
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand RemoveCommand { get; }
        public ICommand ApplyCommand { get; }

        public void LoadTestData()
        {
            Contacts.Clear();
            Contacts.Add(
                new Contact("Иван Иванов", "+7 (999) 123-45-67", "ivan.ivanov@example.com")
            );
            Contacts.Add(
                new Contact("Петр Петров", "+7 (999) 234-56-78", "petr.petrov@example.com")
            );
            Contacts.Add(
                new Contact("Мария Смирнова", "+7 (999) 345-67-89", "maria.smirnova@example.com")
            );
            Contacts.Add(
                new Contact("Елена Козлова", "+7 (999) 456-78-90", "elena.kozlova@example.com")
            );
            Contacts.Add(
                new Contact("Дмитрий Соколов", "+7 (999) 567-89-01", "dmitry.sokolov@example.com")
            );
            Contacts.Add(
                new Contact("Анна Попова", "+7 (999) 678-90-12", "anna.popova@example.com")
            );
            Contacts.Add(
                new Contact("Сергей Лебедев", "+7 (999) 789-01-23", "sergey.lebedev@example.com")
            );
            Contacts.Add(
                new Contact("Ольга Новикова", "+7 (999) 890-12-34", "olga.novikova@example.com")
            );
            Contacts.Add(
                new Contact("Алексей Морозов", "+7 (999) 901-23-45", "alexey.morozov@example.com")
            );
            Contacts.Add(
                new Contact("Татьяна Волкова", "+7 (999) 012-34-56", "tatiana.volkova@example.com")
            );
        }

        public MainViewModel()
        {
            /// <summary>
            /// Загружаем контакты при запуске
            /// </summary>
            _contacts = ContactsSerializer.LoadContacts();
            /// <summary>
            // _contacts = new ObservableCollection<Contact>();
            // LoadTestData();
            /// </summary>
            _editingContact = new Contact(); // ← Добавьте это

            /// <summary>
            // Инициализация команд
            /// </summary>
            AddCommand = new RelayCommand(_ => StartAdding(), _ => IsAddEnabled);
            EditCommand = new RelayCommand(_ => StartEditing(), _ => IsEditRemoveEnabled);
            RemoveCommand = new RelayCommand(_ => RemoveContact(), _ => IsEditRemoveEnabled);
            ApplyCommand = new RelayCommand(_ => ApplyChanges(), _ => CanApply());

            /// <summary>
            // Если контактов нет, создаем пустую коллекцию
            /// </summary>
            if (_contacts == null)
            {
                _contacts = new ObservableCollection<Contact>();
            }
        }

        /// <summary>
        // Коллекция контактов для отображения в списке
        /// </summary>
        public ObservableCollection<Contact> Contacts
        {
            get => _contacts;
            set
            {
                _contacts = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        // Выбранный контакт в списке
        /// </summary>
        public Contact SelectedContact
        {
            get => _selectedContact;
            set
            {
                if (_selectedContact != value)
                {
                    _selectedContact = value;
                    OnPropertyChanged();
                    CancelEditing();

                    /// <summary>
                    // КЛЮЧЕВОЕ: Копируем выбранный контакт в EditingContact
                    /// </summary>
                    if (value != null && !IsAdding && !IsEditing)
                    {
                        EditingContact = new Contact(value.Name, value.PhoneNumber, value.Email);
                    }
                }
            }
        }

        /// <summary>
        // Контакт, который сейчас редактируется или создается
        /// </summary>
        public Contact EditingContact
        {
            get => _editingContact;
            set
            {
                if (_editingContact != null)
                {
                    _editingContact.ErrorsChanged -= OnEditingContactErrorsChanged;
                }

                _editingContact = value;

                if (_editingContact != null)
                {
                    _editingContact.ErrorsChanged += OnEditingContactErrorsChanged;
                }

                OnPropertyChanged();
            }
        }

        /// <summary>
        // Обновляет доступность команды Apply при изменении ошибок проверки
        /// </summary>
        private void OnEditingContactErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            (ApplyCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        /// <summary>
        // Флаги состояния
        /// </summary>
        public bool IsAdding
        {
            get => _isAdding;
            set
            {
                _isAdding = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsReadOnly));
                OnPropertyChanged(nameof(IsEditRemoveEnabled));
                OnPropertyChanged(nameof(IsAddEnabled));
                OnPropertyChanged(nameof(ApplyVisibility));

                // Обновляем команды
                (AddCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (EditCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (RemoveCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (ApplyCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                _isEditing = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsReadOnly));
                OnPropertyChanged(nameof(IsEditRemoveEnabled));
                OnPropertyChanged(nameof(IsAddEnabled));
                OnPropertyChanged(nameof(ApplyVisibility));

                // Обновляем команды
                (AddCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (EditCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (RemoveCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (ApplyCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        // Вычисляемые свойства для UI
        /// </summary>
        public bool IsReadOnly => !(IsAdding || IsEditing);

        public bool IsEditRemoveEnabled => !(IsAdding || IsEditing) && SelectedContact != null;

        public bool IsAddEnabled => !(IsAdding || IsEditing);

        public Visibility ApplyVisibility =>
            (IsAdding || IsEditing) ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>
        // Определяет, можно ли применить изменения: идёт редактирование
        // или добавление и введённые данные прошли проверку
        /// </summary>
        private bool CanApply()
        {
            return (IsAdding || IsEditing) && EditingContact != null && !EditingContact.HasErrors;
        }

        /// <summary>
        // Методы команд
        /// </summary>
        private void StartAdding()
        {
            // Снимаем выделение
            SelectedContact = null;

            // Создаем пустой контакт для редактирования
            EditingContact = new Contact();

            // Включаем режим добавления
            IsAdding = true;
        }

        private void StartEditing()
        {
            if (SelectedContact == null)
                return;

            /// <summary>
            // Создаем копию контакта для редактирования
            /// </summary>
            EditingContact = new Contact(
                SelectedContact.Name,
                SelectedContact.PhoneNumber,
                SelectedContact.Email
            );

            /// <summary>
            // Включаем режим редактирования
            /// </summary>
            IsEditing = true;
        }

        private void ApplyChanges()
        {
            if (IsAdding)
            {
                // Добавляем новый контакт
                Contacts.Add(EditingContact);
                SelectedContact = EditingContact;
            }
            else if (IsEditing)
            {
                // Находим оригинальный контакт и обновляем его
                var originalContact = Contacts.FirstOrDefault(c => c == SelectedContact);
                if (originalContact != null)
                {
                    originalContact.Name = EditingContact.Name;
                    originalContact.PhoneNumber = EditingContact.PhoneNumber;
                    originalContact.Email = EditingContact.Email;

                    // Обновляем выделение, чтобы UI обновился
                    SelectedContact = originalContact;
                }
            }

            // Сохраняем изменения
            ContactsSerializer.SaveContacts(Contacts);

            // Выходим из режима редактирования
            ExitEditingMode();
        }

        private void RemoveContact()
        {
            if (SelectedContact == null)
                return;

            int selectedIndex = Contacts.IndexOf(SelectedContact);
            Contact contactToRemove = SelectedContact;

            /// <summary>
            // Снимаем выделение перед удалением
            /// </summary>
            SelectedContact = null;

            /// <summary>
            // Удаляем контакт
            /// </summary>
            Contacts.Remove(contactToRemove);

            /// <summary>
            // Устанавливаем выделение на следующий или предыдущий контакт
            /// </summary>
            if (Contacts.Count > 0)
            {
                if (selectedIndex < Contacts.Count)
                {
                    // Следующий контакт
                    SelectedContact = Contacts[selectedIndex];
                }
                else if (selectedIndex > 0)
                {
                    // Предыдущий (если удалили последний)
                    SelectedContact = Contacts[selectedIndex - 1];
                }
            }

            /// <summary>
            // Сохраняем изменения
            /// </summary>
            ContactsSerializer.SaveContacts(Contacts);
        }

        private void CancelEditing()
        {
            if (IsAdding || IsEditing)
            {
                ExitEditingMode();
            }
        }

        private void ExitEditingMode()
        {
            IsAdding = false;
            IsEditing = false;
            EditingContact = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
