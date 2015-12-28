using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.AspNet.SignalR.Client;
using lib = ServedWhiteNoodlesFlowingInSmallFlumeLibraries;

namespace Client.ViewModel
{
    public class GuestViewModel : INotifyPropertyChanged
    {
        public lib.Guest Guest { get; } = new lib.Guest();

        volatile bool _flowing;
        /// <summary>
        /// 流れているかどうか
        /// </summary>
        public bool Flowing
        {
            get { return _flowing; }
            set
            {
                if(value == _flowing) return;
                _flowing = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Flowing)));
            }
        }
        IHubProxy hub;
        /// <summary>
        /// Hubをセットする
        /// </summary>
        /// <param name="hub"></param>
        public void SetHub(IHubProxy hub)
        {
            if(hub == null) throw new ArgumentNullException(nameof(hub));
            this.hub = hub;
            
            hub.On<string, Type>("serve",async (json, type) =>
              {
                  var noodles = lib.NoodleListConverter.Convert(json, type);
                  if(!noodles.Any()) return;
                  
                  ServedInformation = string.Join("\r\n", ServedInformation, $"{noodles.First().Name}が流れてきたよ！");
                  _flowing = true;

                  try
                  {
                      await Task.Delay(5000);
                      if(!_isPick)
                      {
                          await hub.Invoke("Picking", 0);
                          return;
                      }

                      var pickedCount = Guest.Picking(noodles);
                      ServedInformation = string.Join("\r\n", ServedInformation, Guest.Eat(noodles.Take(pickedCount)));
                      await hub.Invoke("Picking", pickedCount);
                  }
                  finally
                  {
                      _flowing = false;
                      _isPick = false;
                  }
              });
        }

        string _servedInformation;
        /// <summary>
        /// 情報表示欄
        /// </summary>
        public string ServedInformation
        {
            get { return _servedInformation; }
            set
            {
                if(_servedInformation == value) return;
                _servedInformation = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ServedInformation)));
            }
        }


        volatile bool _isPick = false;
        ServerCommand _command;

        public ServerCommand Serve => _command ?? (_command = new ServerCommand(async () => await Task.Run(() => _isPick = true), () => _flowing));

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

    }

    /// <summary>
    /// ICommand実装（手抜き）
    /// </summary>
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
