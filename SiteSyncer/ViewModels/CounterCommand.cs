using System;
using System.Windows.Input;

namespace SiteSyncer.ViewModels
{
    class CounterCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private Action<object> _execute;
        private int _counter;

        public CounterCommand(Action<object> execute)
        {
            _execute = execute;
        }

        public bool CanExecute(object parameter)
        {
            return _counter == 0;
        }

        public void ChangeCanExecute(int count)
        {
            lock (_execute) { _counter += count; }
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}
