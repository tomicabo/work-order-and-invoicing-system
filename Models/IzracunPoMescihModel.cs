using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSSavkovic.Models
{
    class IzracunPoMescihModel : INotifyPropertyChanged
    {
        private string _strosek;
        private string _prihodek;
        private string _dobicek;
        private string _leto_mesec;

        public string Strosek
        {
            get { return _strosek; }
            set
            {
                if (value != _strosek)
                {
                    _strosek = value;
                    NotifyPropertyChanged("Strosek");
                }
            }
        }

        public string Prihodek
        {
            get { return _prihodek; }
            set
            {
                if (value != _prihodek)
                {
                    _prihodek = value;
                    NotifyPropertyChanged("Prihodek");
                }
            }
        }

        public string Dobicek
        {
            get { return _dobicek; }
            set
            {
                if (value != _dobicek)
                {
                    _dobicek = value;
                    NotifyPropertyChanged("Dobicek");
                }
            }
        }

        public string LetoMesec
        {
            get { return _leto_mesec; }
            set
            {
                if (value != _leto_mesec)
                {
                    _leto_mesec = value;
                    NotifyPropertyChanged("LetoMesec");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        private void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
