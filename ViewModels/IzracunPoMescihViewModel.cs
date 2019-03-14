using MySql.Data.MySqlClient;
using POSSavkovic.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSSavkovic.ViewModels
{
    class IzracunPoMescihViewModel
    {
        public ObservableCollection<IzracunPoMescihModel> izracun_po_mescih = new ObservableCollection<IzracunPoMescihModel>();
        public ObservableCollection<IzracunPoMescihModel> IzracunPoMescih
        {
            get;
            set;
        }

        public IzracunPoMescihViewModel()
        {
            DobiIzracun();
            IzracunPoMescih = izracun_po_mescih;
        }

        public ObservableCollection<IzracunPoMescihModel> DobiIzracun()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["myDatabaseConnection"].ConnectionString;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                var query = "select ustvarjeno, sum(strosek + strosek2 + strosek3 + strosek4) as strosek, sum(prihodek) as prihodek from delovni_nalogi GROUP BY YEAR(ustvarjeno), MONTH(ustvarjeno);";

                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        decimal ts;
                        decimal tp;
                        decimal td;

                        IzracunPoMescihModel temp = new IzracunPoMescihModel();
                        if (reader["ustvarjeno"] != DBNull.Value)
                        {
                            DateTime temp_date = Convert.ToDateTime(reader["ustvarjeno"]);
                            temp.LetoMesec = temp_date.ToString("yyyy - MMMM");
                        }
                        if (reader["strosek"] != DBNull.Value)
                        {
                            ts = Convert.ToDecimal(reader["strosek"]);

                            decimal temp_format_strosek = Convert.ToDecimal(reader["strosek"]);
                            temp.Strosek = temp_format_strosek.ToString("#,###.00 €");
                        }
                        else ts = 0;

                        if (reader["prihodek"] != DBNull.Value)
                        {
                            tp = Convert.ToDecimal(reader["prihodek"]);

                            decimal temp_format_prihodek = Convert.ToDecimal(reader["prihodek"]);
                            temp.Prihodek = temp_format_prihodek.ToString("#,###.00 €");
                        }
                        else tp = 0;

                        td = tp - ts;
                        temp.Dobicek = td.ToString("#,###.00 €");

                        izracun_po_mescih.Add(temp);
                    }
                }
                connection.Close();
                return izracun_po_mescih;
            }
        }
    }
}
