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

namespace Client.ViewModel
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        string _name ;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if(value == _name) return;
                _name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }

        AllergyType _allergyType;
        public AllergyType AllergyType
        {
            get
            {
                return _allergyType;
            }
            set
            {
                if(value == _allergyType) return;
                _allergyType= value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AllergyType)));
            }
        }
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

    }
}
