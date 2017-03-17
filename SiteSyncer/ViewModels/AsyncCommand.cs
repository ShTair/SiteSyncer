using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SiteSyncer.ViewModels
{
    class AsyncCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private Func<object, Task> _execute;
        private bool _canExecute;

        public AsyncCommand(Func<object, Task> execute)
        {
            _execute = execute;
            _canExecute = true;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public void ChangeCanExecute(bool canExecute)
        {
            if (_canExecute == canExecute) return;
            _canExecute = canExecute;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public async void Execute(object parameter)
        {
            try
            {
                ChangeCanExecute(false);
                await _execute(parameter);
            }
            finally
            {
                ChangeCanExecute(true);
            }
        }
    }
}
