using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace POSSavkovic.Models
{
    public class DelovniNalogiModel :INotifyPropertyChanged
    {
        private int _id;
        private string _opis;
        private decimal _strosek;
        private decimal _prihodek;
        private decimal _dobicek;
        private string _mesec;
        private string _status;
        private decimal _strosek2;
        private decimal _strosek3;
        private decimal _strosek4;
        private string _strosek2_opis;
        private string _strosek3_opis;
        private string _strosek4_opis;
        private string _ustvarjeno;
        private string _kupec;
        private int _kupec_id;
        private int _racun_id;
        private int _dobavnica_id;
        private string _st_narocila;

        public int Id
        {
            get { return _id; }
            set
            {
                if (value != _id)
                {
                    _id = value;
                    NotifyPropertyChanged("Id");
                }
            }
        }

        public string Opis
        {
            get { return _opis; }
            set
            {
                if (value != _opis)
                {
                    _opis = value;
                    NotifyPropertyChanged("Opis");
                }
            }
        }

        public decimal Strosek
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

        public decimal Prihodek
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

        public decimal Dobicek
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

        public string Mesec
        {
            get { return _mesec; }
            set
            {
                if (value != _mesec)
                {
                    _mesec = value;
                    NotifyPropertyChanged("Mesec");
                }
            }
        }

        public string Status
        {
            get { return _status; }
            set
            {
                if (value != _status)
                {
                    _status = value;
                    NotifyPropertyChanged("Status");
                }
            }
        }

        public decimal Strosek2
        {
            get { return _strosek2; }
            set
            {
                if (value != _strosek2)
                {
                    _strosek2 = value;
                    NotifyPropertyChanged("Strosek2");
                }
            }
        }

        public decimal Strosek3
        {
            get { return _strosek3; }
            set
            {
                if (value != _strosek3)
                {
                    _strosek3 = value;
                    NotifyPropertyChanged("Strosek3");
                }
            }
        }

        public decimal Strosek4
        {
            get { return _strosek4; }
            set
            {
                if (value != _strosek4)
                {
                    _strosek4 = value;
                    NotifyPropertyChanged("Strosek4");
                }
            }
        }

        public string Strosek2Opis
        {
            get { return _strosek2_opis; }
            set
            {
                if (value != _strosek2_opis)
                {
                    _strosek2_opis = value;
                    NotifyPropertyChanged("Strosek2Opis");
                }
            }
        }

        public string Strosek3Opis
        {
            get { return _strosek3_opis; }
            set
            {
                if (value != _strosek3_opis)
                {
                    _strosek3_opis = value;
                    NotifyPropertyChanged("Strosek3Opis");
                }
            }
        }

        public string Strosek4Opis
        {
            get { return _strosek4_opis; }
            set
            {
                if (value != _strosek4_opis)
                {
                    _strosek4_opis = value;
                    NotifyPropertyChanged("Strosek4Opis");
                }
            }
        }

        public string Ustvarjeno
        {
            get { return _ustvarjeno; }
            set
            {
                if (value != _ustvarjeno)
                {
                    _ustvarjeno = value;
                    NotifyPropertyChanged("Ustvarjeno");
                }
            }
        }

        public string Kupec
        {
            get { return _kupec; }
            set
            {
                if (value != _kupec)
                {
                    _kupec = value;
                    NotifyPropertyChanged("Kupec");
                }
            }
        }

        public int KupecId
        {
            get { return _kupec_id; }
            set
            {
                if (value != _kupec_id)
                {
                    _kupec_id = value;
                    NotifyPropertyChanged("KupecId");
                }
            }
        }

        public int RacunId
        {
            get { return _racun_id; }
            set
            {
                if (value != _racun_id)
                {
                    _racun_id = value;
                    NotifyPropertyChanged("RacunId");
                }
            }
        }

        public int DobavnicaId
        {
            get { return _dobavnica_id; }
            set
            {
                if (value != _dobavnica_id)
                {
                    _dobavnica_id = value;
                    NotifyPropertyChanged("DobavnicaId");
                }
            }
        }

        public string StNarocila
        {
            get { return _st_narocila; }
            set
            {
                if (value != _st_narocila)
                {
                    _st_narocila = value;
                    NotifyPropertyChanged("StNarocila");
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
