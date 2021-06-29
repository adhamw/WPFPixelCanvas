using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WPFPixelCanvas.common
{
    public class RelayCommand : ICommand
    {
        //Private fields
        private Action<object> _Execute;
        private Func<object, bool> _CanExecute; //Alternatively Predicate<object> , same thing

        //Constructor
        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _Execute = execute;
            _CanExecute = canExecute;
        }

        //Public events
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        //Public properties
        public bool CanExecute(object param) { return _CanExecute == null || _CanExecute(param); }
        public void Execute(object param) { _Execute?.Invoke(param); }
    }
}
