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
    class KupciViewModel
    {
        public ObservableCollection<KupciModel> kupci = new ObservableCollection<KupciModel>();
        public ObservableCollection<KupciModel> Kupci
        {
            get;
            set;
        }

        public KupciViewModel()
        {
            DobiKupce();
            Kupci = kupci;
        }

        public ObservableCollection<KupciModel> DobiKupce()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["myDatabaseConnection"].ConnectionString;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                var query = "select * from kupci where izbrisano != 1 order by podjetje";

                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        KupciModel kupci_temp = new KupciModel();
                        if (reader["id"] != DBNull.Value)
                            kupci_temp.Id = Convert.ToInt32(reader["id"]);
                        if (reader["podjetje"] != DBNull.Value)
                            kupci_temp.Podjetje = Convert.ToString(reader["podjetje"]);
                        if(reader["naslov"] != DBNull.Value)
                            kupci_temp.Naslov = Convert.ToString(reader["naslov"]);
                        if (reader["posta"] != DBNull.Value)
                            kupci_temp.Posta = Convert.ToString(reader["posta"]);
                        if (reader["drzava"] != DBNull.Value)
                            kupci_temp.Drzava = Convert.ToString(reader["drzava"]);
                        if (reader["davcna_st"] != DBNull.Value)
                            kupci_temp.DavcnaSt = Convert.ToString(reader["davcna_st"]);

                        kupci.Add(kupci_temp);
                    }
                }
                connection.Close();
                return kupci;
            }
        }
    }
}
