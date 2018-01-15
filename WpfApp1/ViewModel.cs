using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Threading;

namespace WpfApp1
{
    class ViewModel : INotifyPropertyChanged
    {
        public class ExitCommand : ICommand
        {
            ViewModel viewModel;
            public ExitCommand(ViewModel parent) { this.viewModel = parent; }

            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                return viewModel.Exitable;
            }

            public void Execute(object parameter)
            {
                var command = (string)parameter;
                switch (command)
                {
                    case "exitWnd":
                        viewModel.Close();
                        break;
                    case "openDlg":
                        var wnd = new Window1();
                        var result = wnd.ShowDialog();
                        viewModel.DialogResult = result.ToString();
                        break;
                }
            }
        }
        public ExitCommand MyExit { private set; get; }

        public class CloseViewMessenger
        {
            public event EventHandler ViewModelCloseEvent;
            public void RaiseViewModelCloseEvent()
            {
                ViewModelCloseEvent?.Invoke(this, new EventArgs());
            }
        }
        public CloseViewMessenger MyCloseViewMessenger { private set; get; } = new CloseViewMessenger();

        public event PropertyChangedEventHandler PropertyChanged;

            string dialogResult;
            public string DialogResult
            {
                set { if (dialogResult != value) { dialogResult = value;PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DialogResult))); } }
                get { return dialogResult; }
            }
            
        public bool Exitable { private set; get; }
        public DateTime ClockTime { private set; get; }
        DispatcherTimer dispatcherTimer;
        public ViewModel()
        {
            MyExit = new ExitCommand(this);
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Start();
            Exitable = true;
        }

        static PropertyChangedEventArgs clockTimePropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(ClockTime));
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            ClockTime = DateTime.Now;
            PropertyChanged?.Invoke(this, clockTimePropertyChangedEventArgs);
        }

        public void Close()
        {
            //something to do.Resource Clear, etc. 
            dispatcherTimer.Stop();
            dispatcherTimer.Tick -= DispatcherTimer_Tick;

            MyCloseViewMessenger.RaiseViewModelCloseEvent();
        }
    }

    class DialogViewModel : INotifyPropertyChanged
    {
        public class DialogButtonCommand : ICommand
        {
            DialogViewModel parent;
            public DialogButtonCommand(DialogViewModel parent) { this.parent = parent; }

            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                var command = (string)parameter;
                switch(command)
                {
                    case "cancelCmd":
                        parent.MyMessenger.RaiseMessengerEvent(false);
                        break;
                    case "confirmCmd":
                        parent.MyMessenger.RaiseMessengerEvent(true);
                        break;
                    default:
                        break;
                }
            }
        }
        public DialogButtonCommand MyButtonCommand { private set; get; }

        public Messenger MyMessenger { private set; get; } = new Messenger();

        public event PropertyChangedEventHandler PropertyChanged;

        public DialogViewModel()
        {
            MyButtonCommand = new DialogButtonCommand(this);
        }

    }
}
