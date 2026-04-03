using System;
using System.Windows.Input;

namespace Contacts.Commands;

/// <summary>
/// Реализация интерфейса <see cref="ICommand"/>, позволяющая передавать
/// логику выполнения и условия выполнения команды через делегаты.
/// Используется в MVVM для привязки действий UI к логике приложения.
/// </summary>
public class RelayCommand : ICommand
{
    /// <summary>
    /// Делегат, содержащий логику выполнения команды.
    /// </summary>
    private readonly Action<object?> _execute;

    /// <summary>
    /// Делегат, определяющий, может ли команда быть выполнена.
    /// </summary>
    private readonly Predicate<object?>? _canExecute;

    /// <summary>
    /// Инициализирует новый экземпляр команды <see cref="RelayCommand"/>.
    /// </summary>
    /// <param name="execute">Метод, выполняемый при вызове команды.</param>
    /// <param name="canExecute">
    /// Метод, определяющий доступность команды. Если не задан, команда доступна всегда.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Выбрасывается, если <paramref name="execute"/> равен null.
    /// </exception>
    public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    /// <summary>
    /// Определяет, может ли команда быть выполнена.
    /// </summary>
    /// <param name="parameter">Параметр команды.</param>
    /// <returns>
    /// true, если команда может быть выполнена; иначе false.
    /// </returns>
    public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

    /// <summary>
    /// Выполняет команду.
    /// </summary>
    /// <param name="parameter">Параметр команды.</param>
    public void Execute(object? parameter) => _execute(parameter);

    /// <summary>
    /// Событие, уведомляющее об изменении возможности выполнения команды.
    /// </summary>
    public event EventHandler? CanExecuteChanged;

    /// <summary>
    /// Вызывает событие <see cref="CanExecuteChanged"/>,
    /// уведомляя UI о необходимости обновить состояние команды.
    /// </summary>
    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
