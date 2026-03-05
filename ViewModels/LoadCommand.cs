// ViewModel/LoadCommand.cs
using System;
using System.Windows.Input;
using Lab2Mvvm.Model.Services;

namespace Lab2Mvvm.ViewModel
{
    public class LoadCommand : ICommand
    {
        private readonly MainVM _vm;
        private readonly ContactSerializer _serializer;

        public LoadCommand(MainVM vm, ContactSerializer serializer)
        {
            _vm = vm;
            _serializer = serializer;
        }

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter)
        {
            var loaded = _serializer.Load();
            _vm.ApplyContact(loaded);
        }

        public event EventHandler? CanExecuteChanged;
    }
}