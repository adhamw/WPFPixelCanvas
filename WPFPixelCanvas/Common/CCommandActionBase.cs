//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Input;

//namespace gfx2021B.common
//{
//    //Defines a simple structure for action commands
//    public class CCommandActionBase : ICommand
//    {
//        public Action<object> ExecuteFunction { get; set; }
//        public Predicate<object> CanExecuteFunction { get; set; }

//        public event EventHandler CanExecuteChanged;
//        public bool CanExecute(object parameter)
//        {
//            bool retval = false;
//            if (CanExecuteFunction != null) { retval = CanExecuteFunction(parameter); }
//            return retval;
//        }
//        public void Execute(object parameter) { ExecuteFunction?.Invoke(parameter); }
//    }
//}
