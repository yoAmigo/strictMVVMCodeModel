using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interactivity;

namespace WpfApp1
{
    public class Messenger
    {
        public event EventHandler<MessengerEventArgs> MessengerEvent;
        public void RaiseMessengerEvent(bool result)
        {
            MessengerEvent?.Invoke(this, new MessengerEventArgs(result));
        }
    }

    public class MessengerEventArgs:EventArgs
    {
        public bool Result { private set; get; }
        public MessengerEventArgs(bool result) { Result = result; }
    }

    public class MessengerEventTrigger:EventTrigger
    {
        protected override string GetEventName()
        {
            return "MessengerEvent";
        }
    }

    public class MessengerAction : TriggerAction<System.Windows.Window>
    {
        protected override void Invoke(object parameter)
        {
            AssociatedObject.DialogResult = (parameter as MessengerEventArgs).Result;
            AssociatedObject.Close();
        }
    }
}
