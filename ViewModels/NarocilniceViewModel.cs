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
    class NarocilniceViewModel
    {
        public ObservableCollection<NarocilniceModel> narocilnice = new ObservableCollection<NarocilniceModel>();
        public ObservableCollection<NarocilniceModel> Narocilnice
        {
            get;
            set;
        }

        public NarocilniceViewModel()
        {
            DobiNarocilnice();
            Narocilnice = narocilnice;
        }

        public ObservableCollection<NarocilniceModel> DobiNarocilnice()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["myDatabaseConnection"].ConnectionString;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                var query = "select n.id, n.ustvarjeno, n.st_narocilnice, n.skupaj_cena, d.podjetje " +
                            "from narocilnice n " +
                            "join dobavitelji d on n.dobavitelj = d.id; ";

                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        NarocilniceModel n_temp = new NarocilniceModel();
                        if (reader["id"] != DBNull.Value)
                            n_temp.Id = Convert.ToInt32(reader["id"]);
                        if (reader["ustvarjeno"] != DBNull.Value)
                        {
                            DateTime temp = Convert.ToDateTime(reader["ustvarjeno"]);
                            n_temp.Ustvarjeno = temp.ToString("dd.MM.yyyy");
                        }
                        if (reader["st_narocilnice"] != DBNull.Value)
                            n_temp.StNarocilnice = Convert.ToString(reader["st_narocilnice"]);
                        if (reader["podjetje"] != DBNull.Value)
                            n_temp.Dobavitelj = Convert.ToString(reader["podjetje"]);
                        if (reader["skupaj_cena"] != DBNull.Value)
                            n_temp.SkupajCena = Convert.ToString(reader["skupaj_cena"]);
                        else n_temp.SkupajCena = "posamična cena";

                        narocilnice.Add(n_temp);
                    }
                }
                connection.Close();
                return narocilnice;
            }
        }
    }
}
