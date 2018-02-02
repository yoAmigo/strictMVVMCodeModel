using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Threading;
using System.Collections.ObjectModel;

namespace WpfApp1
{
    enum ForSelection { NONE, No1, No2, No3, No4, No5 }

    class ViewModel : INotifyPropertyChanged
    {
        public class TextEnterCommand : ICommand
        {
            ViewModel parent;
            public TextEnterCommand(ViewModel parent) { this.parent = parent; }

            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                parent.ModelForList.Add(parameter.ToString());
            }
        }
        public TextEnterCommand MyTextEnter { private set; get; }

        public class ButtonCommand : ICommand
        {
            ViewModel viewModel;
            public ButtonCommand(ViewModel parent) { this.viewModel = parent; }

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
                        if(result.HasValue)
                            if(result.Value)
                            {
                                viewModel.SelectedValue = (ForSelection)wnd.comboBoxEnum.SelectedValue;
                            }
                        viewModel.DialogResult = result?.ToString();
                        break;
                }
            }
        }
        public ButtonCommand MyButtonCommand { private set; get; }

        public class CloseViewMessenger
        {
            public event EventHandler ViewModelCloseEvent;
            public void RaiseViewModelCloseEvent()
            {
                ViewModelCloseEvent?.Invoke(this, new EventArgs());
            }
        }
        public CloseViewMessenger MyCloseViewMessenger { private set; get; } = new CloseViewMessenger();

        public ObservableCollection<string> ModelForList { set; get; } = new ObservableCollection<string>();

        public event PropertyChangedEventHandler PropertyChanged;

        string dialogResult;
        public string DialogResult
        {
            set { if (dialogResult != value) { dialogResult = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DialogResult))); } }
            get { return dialogResult; }
        }

        ForSelection selectedValue;
        public ForSelection SelectedValue
        {
            set { if (value != selectedValue) { selectedValue = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedValue))); } }
            get { return selectedValue; }
        }

        public bool Exitable { private set; get; }
        public DateTime ClockTime { private set; get; }
        DispatcherTimer dispatcherTimer;
        public ViewModel()
        {
            MyTextEnter = new TextEnterCommand(this);
            MyButtonCommand = new ButtonCommand(this);
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
                switch (command)
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
