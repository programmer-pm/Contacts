using System;
using System.Windows.Input;
using Contacts.Model.Services;

namespace Contacts.ViewModel
{
    /// <summary>
    /// Представляет команду загрузки контакта из файла в модель представления.
    /// </summary>
    public class LoadCommand : ICommand
    {
        private readonly MainVM _vm;
        private readonly ContactSerializer _serializer;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="LoadCommand"/>.
        /// </summary>
        /// <param name="vm">Модель представления, в которую загружаются данные.</param>
        /// <param name="serializer">Сервис сериализации контактов.</param>
        public LoadCommand(MainVM vm, ContactSerializer serializer)
        {
            _vm = vm;
            _serializer = serializer;
        }

        /// <summary>
        /// Определяет, доступна ли команда загрузки.
        /// </summary>
        /// <param name="parameter">Неиспользуемый параметр команды.</param>
        /// <returns>Всегда возвращает <see langword="true"/>.</returns>
        public bool CanExecute(object? parameter) => true;

        /// <summary>
        /// Загружает контакт из файла и применяет его к модели представления.
        /// </summary>
        /// <param name="parameter">Неиспользуемый параметр команды.</param>
        public void Execute(object? parameter)
        {
            var loaded = _serializer.Load();
            _vm.ApplyContact(loaded);
        }

        /// <summary>
        /// Происходит при изменении состояния доступности команды.
        /// </summary>
        public event EventHandler? CanExecuteChanged;
    }
}
