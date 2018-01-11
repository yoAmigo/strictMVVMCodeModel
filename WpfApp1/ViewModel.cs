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
                viewModel.Close();
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
}
