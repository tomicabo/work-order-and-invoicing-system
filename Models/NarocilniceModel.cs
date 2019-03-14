using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSSavkovic.Models
{
    class NarocilniceModel : INotifyPropertyChanged
    {
        private int _id;
        private string _ustvarjeno;
        private string _st_narocilnice;
        private string _dobavitelj;
        private string _skupaj_cena;

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

        public string StNarocilnice
        {
            get { return _st_narocilnice; }
            set
            {
                if (value != _st_narocilnice)
                {
                    _st_narocilnice = value;
                    NotifyPropertyChanged("StNarocilnice");
                }
            }
        }

        public string Dobavitelj
        {
            get { return _dobavitelj; }
            set
            {
                if (value != _dobavitelj)
                {
                    _dobavitelj = value;
                    NotifyPropertyChanged("Dobavitelj");
                }
            }
        }

        public string SkupajCena
        {
            get { return _skupaj_cena; }
            set
            {
                if (value != _skupaj_cena)
                {
                    _skupaj_cena = value;
                    NotifyPropertyChanged("SkupajCena");
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
