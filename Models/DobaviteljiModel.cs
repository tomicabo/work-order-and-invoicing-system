using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSSavkovic.Models
{
    public class DobaviteljiModel : INotifyPropertyChanged
    {
        private int _id;
        private string _podjetje;
        private string _naslov;
        private string _posta;
        private string _drzava;
        private string _davcna_st;

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

        public string Podjetje
        {
            get { return _podjetje; }
            set
            {
                if (value != _podjetje)
                {
                    _podjetje = value;
                    NotifyPropertyChanged("Podjetje");
                }
            }
        }

        public string Naslov
        {
            get { return _naslov; }
            set
            {
                if (value != _naslov)
                {
                    _naslov = value;
                    NotifyPropertyChanged("Naslov");
                }
            }
        }

        public string Posta
        {
            get { return _posta; }
            set
            {
                if (value != _posta)
                {
                    _posta = value;
                    NotifyPropertyChanged("Posta");
                }
            }
        }

        public string Drzava
        {
            get { return _drzava; }
            set
            {
                if (value != _drzava)
                {
                    _drzava = value;
                    NotifyPropertyChanged("Drzava");
                }
            }
        }

        public string DavcnaSt
        {
            get { return _davcna_st; }
            set
            {
                if (value != _davcna_st)
                {
                    _davcna_st = value;
                    NotifyPropertyChanged("DavcnaSt");
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
