using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WPFPixelCanvas.common
{
   public class NotificationPropertyBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        //The property that changes, calls this in the property setter.
        //Also the name of the property setter is the name of the property that
        //changed. So we can use CallerMemberName to make a generic function to handle
        //property changes.

        //Only requirement is that we notify chang in the Property setter.
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            //Propertychanged is an event handler. Sort of means it holds one or more 
            //methods that should be called ( subscribers ) if this event triggers.
            // If no subscribers it will be null.
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }


}
