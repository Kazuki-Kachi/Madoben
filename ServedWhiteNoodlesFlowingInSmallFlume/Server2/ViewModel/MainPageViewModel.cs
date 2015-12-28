using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.AspNet.SignalR.Client;
using ServedWhiteNoodlesFlowingInSmallFlumeLibraries;

namespace Server.ViewModel
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        IHubProxy hub;
        bool _isCompleted = true;

        public async Task SetHub(IHubProxy hub)
        {
            this.hub = hub;
            hub.On<string>("info", info => ServedInformation = string.Join("\r\n", ServedInformation, info));
            hub.On("completed", () => _isCompleted = true);
            await hub.Invoke("SetupServer");
        }

        bool? _isSelectedWhiteNoodle = true;
        public bool? IsSelectedWhiteNoodle
        {
            get
            {
                return _isSelectedWhiteNoodle;
            }
            set
            {
                if(value == _isSelectedWhiteNoodle) return;
                _isSelectedWhiteNoodle = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelectedWhiteNoodle)));
            }
        }

        bool? _isSelectedUdon;
        public bool? IsSelectedUdon
        {
            get
            {
                return _isSelectedUdon;
            }
            set
            {
                if(value == _isSelectedUdon) return;
                _isSelectedUdon= value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelectedUdon)));
            }
        }

        bool? _isSelectedBuckwheatNoodle;
        public bool? IsSelectedBuckwheatNoodle
        {
            get
            {
                return _isSelectedBuckwheatNoodle;
            }
            set
            {
                if(value == _isSelectedBuckwheatNoodle) return;
                _isSelectedBuckwheatNoodle = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelectedBuckwheatNoodle)));
            }
        }

        string _servedInformation;
        public string ServedInformation
        {
            get
            {
                return _servedInformation;
            }
            set
            {
                if(_servedInformation == value) return;
                _servedInformation = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ServedInformation)));
            }
        }

        ServerCommand _command;
        public ServerCommand Serve =>
            _command ?? (_command = new ServerCommand(async () =>
            {
                _isCompleted = false;
                await hub.Invoke("Serve", NoodleType);
            }, () => _isCompleted));

        private Type NoodleType => IsSelectedWhiteNoodle ?? false ? typeof(WhiteNoodle) : IsSelectedUdon ?? false ? typeof(Udon) : IsSelectedBuckwheatNoodle ?? false ? typeof(BuckwheatNoodle) : null;

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

    }

    public class ServerCommand : ICommand
    {
        Func<bool> canExecute;
        Action execute;

        public ServerCommand(Action execute) : this(execute, () => true) { }

       public ServerCommand(Action execute, Func<bool> canExecute)
        {
            if(execute == null) throw new ArgumentNullException(nameof(execute));

            if(canExecute == null) throw new ArgumentNullException(nameof(canExecute));

            this.execute = execute;
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute() => canExecute?.Invoke() ?? false;

        public void Execute() => execute?.Invoke();

        bool ICommand.CanExecute(object parameter) => canExecute?.Invoke() ?? false;

        void ICommand.Execute(object parameter) => execute?.Invoke();
    }

}
