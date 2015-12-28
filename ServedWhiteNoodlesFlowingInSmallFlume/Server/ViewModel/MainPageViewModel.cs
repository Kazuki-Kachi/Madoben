using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Server.ViewModel
{
    public class MainPageViewModel : INotifyPropertyChanged
    {

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

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

    }
}
