// ViewModel/SaveCommand.cs
using System;
using System.Windows.Input;
using Lab2Mvvm.Model.Services;

namespace Lab2Mvvm.ViewModel
{
    public class SaveCommand : ICommand
    {
        private readonly MainVM _vm;
        private readonly ContactSerializer _serializer;

        public SaveCommand(MainVM vm, ContactSerializer serializer)
        {
            _vm = vm;
            _serializer = serializer;
        }

        public bool CanExecute(object? parameter) => _vm.IsValid;

        public void Execute(object? parameter)
        {
            _serializer.Save(_vm.Contact);
        }

        public event EventHandler? CanExecuteChanged;

        public void RaiseCanExecuteChanged()
            => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}