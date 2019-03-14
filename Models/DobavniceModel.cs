using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSSavkovic.Models
{
    class DobavniceModel : INotifyPropertyChanged
    {
        private int _id;
        private string _ustvarjeno;
        private string _st_dobavnice;
        private string _kupec;
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

        public string StDobavnice
        {
            get { return _st_dobavnice; }
            set
            {
                if (value != _st_dobavnice)
                {
                    _st_dobavnice = value;
                    NotifyPropertyChanged("StDobavnice");
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
