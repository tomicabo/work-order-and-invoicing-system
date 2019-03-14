using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSSavkovic.Models
{
    class RacuniModel : INotifyPropertyChanged
    {
        private int _id;
        private string _ustvarjeno;
        private string _st_racuna;
        private decimal _skupaj_z_ddv;
        private decimal _skupaj_brez_ddv;
        private string _kupec;
        private string _datum_zap;
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

        public string StRacuna
        {
            get { return _st_racuna; }
            set
            {
                if (value != _st_racuna)
                {
                    _st_racuna = value;
                    NotifyPropertyChanged("StRacuna");
                }
            }
        }

        public decimal SkupajZDDV
        {
            get { return _skupaj_z_ddv; }
            set
            {
                if (value != _skupaj_z_ddv)
                {
                    _skupaj_z_ddv = value;
                    NotifyPropertyChanged("SkupajZDDV");
                }
            }
        }

        public decimal SkupajBrezDDV
        {
            get { return _skupaj_brez_ddv; }
            set
            {
                if (value != _skupaj_brez_ddv)
                {
                    _skupaj_brez_ddv = value;
                    NotifyPropertyChanged("SkupajBrezDDV");
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

        public string DatumZapadlosti
        {
            get { return _datum_zap; }
            set
            {
                if (value != _datum_zap)
                {
                    _datum_zap = value;
                    NotifyPropertyChanged("DatumZapadlosti");
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
