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
    class DobavniceViewModel
    {
        public ObservableCollection<DobavniceModel> dobavnice = new ObservableCollection<DobavniceModel>();
        public ObservableCollection<DobavniceModel> Dobavnice
        {
            get;
            set;
        }

        public DobavniceViewModel()
        {
            DobiDobavnice();
            Dobavnice = dobavnice;
        }

        public ObservableCollection<DobavniceModel> DobiDobavnice()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["myDatabaseConnection"].ConnectionString;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                var query = "select d.id, d.ustvarjeno, d.st_dobavnice, k.podjetje, d.st_narocila " +
                            "from dobavnice d " +
                            "join kupci k on d.kupec = k.id order by ustvarjeno; ";

                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DobavniceModel d_temp = new DobavniceModel();
                        if (reader["id"] != DBNull.Value)
                            d_temp.Id = Convert.ToInt32(reader["id"]);
                        if (reader["ustvarjeno"] != DBNull.Value)
                        {
                            DateTime temp = Convert.ToDateTime(reader["ustvarjeno"]);
                            d_temp.Ustvarjeno = temp.ToString("dd.MM.yyyy");
                        }
                        if (reader["st_dobavnice"] != DBNull.Value)
                        {
                            DateTime temp = Convert.ToDateTime(reader["ustvarjeno"]);
                            d_temp.StDobavnice = Convert.ToString(reader["st_dobavnice"]) + "-" + temp.ToString("yyyy");
                        }
                        if (reader["podjetje"] != DBNull.Value || reader["podjetje"] != "0")
                            d_temp.Kupec = Convert.ToString(reader["podjetje"]);
                        else d_temp.Kupec = "";
                        if (reader["st_narocila"] != DBNull.Value)
                            d_temp.StNarocila = Convert.ToString(reader["st_narocila"]);

                        dobavnice.Add(d_temp);
                    }
                }
                connection.Close();
                return dobavnice;
            }
        }
    }
}
