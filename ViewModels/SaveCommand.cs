using System;
using System.Windows.Input;
using Contacts.Model.Services;

namespace Contacts.ViewModel
{
    /// <summary>
    /// Представляет команду сохранения текущего контакта в файл.
    /// </summary>
    public class SaveCommand : ICommand
    {
        private readonly MainVM _vm;
        private readonly ContactSerializer _serializer;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="SaveCommand"/>.
        /// </summary>
        /// <param name="vm">Модель представления, содержащая сохраняемые данные.</param>
        /// <param name="serializer">Сервис сериализации контактов.</param>
        public SaveCommand(MainVM vm, ContactSerializer serializer)
        {
            _vm = vm;
            _serializer = serializer;
        }

        /// <summary>
        /// Определяет, доступна ли команда сохранения.
        /// </summary>
        /// <param name="parameter">Неиспользуемый параметр команды.</param>
        /// <returns><see langword="true"/>, если данные проходят валидацию; иначе <see langword="false"/>.</returns>
        public bool CanExecute(object? parameter) => _vm.IsValid;

        /// <summary>
        /// Сохраняет текущий контакт из модели представления в файл.
        /// </summary>
        /// <param name="parameter">Неиспользуемый параметр команды.</param>
        public void Execute(object? parameter)
        {
            _serializer.Save(_vm.Contact);
        }

        /// <summary>
        /// Происходит при изменении состояния доступности команды.
        /// </summary>
        public event EventHandler? CanExecuteChanged;

        /// <summary>
        /// Инициирует повторную проверку доступности команды у подписчиков.
        /// </summary>
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
