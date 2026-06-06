using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Contacts.Model;
using Contacts.Model.Services;

namespace Contacts.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        private ObservableCollection<Contact> _contacts;
        private Contact? _selectedContact;
        private Contact? _editingContact;
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
            AddCommand = new RelayCommand(StartAdding, () => IsAddEnabled);
            EditCommand = new RelayCommand(StartEditing, () => IsEditRemoveEnabled);
            RemoveCommand = new RelayCommand(RemoveContact, () => IsEditRemoveEnabled);
            ApplyCommand = new RelayCommand(ApplyChanges, CanApply);

            /// <summary>
            // Если контактов нет, создаем пустую коллекцию
            /// </summary>
            if (_contacts == null)
            {
                _contacts = new ObservableCollection<Contact>();
            }

            if (_contacts.Count > 0)
            {
                SelectedContact = _contacts[0];
            }
        }

        /// <summary>
        // Коллекция контактов для отображения в списке
        /// </summary>
        public ObservableCollection<Contact> Contacts
        {
            get => _contacts;
            set => SetProperty(ref _contacts, value);
        }

        /// <summary>
        // Выбранный контакт в списке
        /// </summary>
        public Contact? SelectedContact
        {
            get => _selectedContact;
            set
            {
                if (SetProperty(ref _selectedContact, value))
                {
                    CancelEditing();
                    OnPropertyChanged(nameof(IsEditRemoveEnabled));
                    (EditCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    (RemoveCommand as RelayCommand)?.NotifyCanExecuteChanged();

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
        public Contact? EditingContact
        {
            get => _editingContact;
            set
            {
                if (_editingContact == value)
                {
                    return;
                }

                if (_editingContact != null)
                {
                    _editingContact.ErrorsChanged -= OnEditingContactErrorsChanged;
                }

                SetProperty(ref _editingContact, value);

                if (_editingContact != null)
                {
                    _editingContact.ErrorsChanged += OnEditingContactErrorsChanged;
                }

                (ApplyCommand as RelayCommand)?.NotifyCanExecuteChanged();
            }
        }

        /// <summary>
        // Обновляет доступность команды Apply при изменении ошибок проверки
        /// </summary>
        private void OnEditingContactErrorsChanged(object? sender, DataErrorsChangedEventArgs e)
        {
            (ApplyCommand as RelayCommand)?.NotifyCanExecuteChanged();
        }

        /// <summary>
        // Флаги состояния
        /// </summary>
        public bool IsAdding
        {
            get => _isAdding;
            set
            {
                if (!SetProperty(ref _isAdding, value))
                {
                    return;
                }
                OnPropertyChanged(nameof(IsReadOnly));
                OnPropertyChanged(nameof(IsEditRemoveEnabled));
                OnPropertyChanged(nameof(IsAddEnabled));
                OnPropertyChanged(nameof(IsApplyVisible));

                // Обновляем команды
                (AddCommand as RelayCommand)?.NotifyCanExecuteChanged();
                (EditCommand as RelayCommand)?.NotifyCanExecuteChanged();
                (RemoveCommand as RelayCommand)?.NotifyCanExecuteChanged();
                (ApplyCommand as RelayCommand)?.NotifyCanExecuteChanged();
            }
        }

        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                if (!SetProperty(ref _isEditing, value))
                {
                    return;
                }
                OnPropertyChanged(nameof(IsReadOnly));
                OnPropertyChanged(nameof(IsEditRemoveEnabled));
                OnPropertyChanged(nameof(IsAddEnabled));
                OnPropertyChanged(nameof(IsApplyVisible));

                // Обновляем команды
                (AddCommand as RelayCommand)?.NotifyCanExecuteChanged();
                (EditCommand as RelayCommand)?.NotifyCanExecuteChanged();
                (RemoveCommand as RelayCommand)?.NotifyCanExecuteChanged();
                (ApplyCommand as RelayCommand)?.NotifyCanExecuteChanged();
            }
        }

        /// <summary>
        // Вычисляемые свойства для UI
        /// </summary>
        public bool IsReadOnly => !(IsAdding || IsEditing);

        public bool IsEditRemoveEnabled => !(IsAdding || IsEditing) && SelectedContact != null;

        public bool IsAddEnabled => !(IsAdding || IsEditing);

        public bool IsApplyVisible => IsAdding || IsEditing;

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
            var selectedContact = SelectedContact;
            if (selectedContact == null)
                return;

            /// <summary>
            // Создаем копию контакта для редактирования
            /// </summary>
            EditingContact = new Contact(
                selectedContact.Name,
                selectedContact.PhoneNumber,
                selectedContact.Email
            );

            /// <summary>
            // Включаем режим редактирования
            /// </summary>
            IsEditing = true;
        }

        private void ApplyChanges()
        {
            if (EditingContact == null)
            {
                return;
            }

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
            var selectedContact = SelectedContact;
            if (selectedContact == null)
                return;

            int selectedIndex = Contacts.IndexOf(selectedContact);
            Contact contactToRemove = selectedContact;

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

    }
}
