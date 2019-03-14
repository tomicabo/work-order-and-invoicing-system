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
    class DobaviteljiViewModel
    {
        public ObservableCollection<DobaviteljiModel> dobavitelji = new ObservableCollection<DobaviteljiModel>();
        public ObservableCollection<DobaviteljiModel> Dobavitelji
        {
            get;
            set;
        }

        public DobaviteljiViewModel()
        {
            DobiDobavitelje();
            Dobavitelji = dobavitelji;
        }

        public ObservableCollection<DobaviteljiModel> DobiDobavitelje()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["myDatabaseConnection"].ConnectionString;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                var query = "select * from dobavitelji where izbrisano != 1";

                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DobaviteljiModel dobavitelji_temp = new DobaviteljiModel();
                        if (reader["id"] != DBNull.Value)
                            dobavitelji_temp.Id = Convert.ToInt32(reader["id"]);
                        if (reader["podjetje"] != DBNull.Value)
                            dobavitelji_temp.Podjetje = Convert.ToString(reader["podjetje"]);
                        if (reader["naslov"] != DBNull.Value)
                            dobavitelji_temp.Naslov = Convert.ToString(reader["naslov"]);
                        if (reader["posta"] != DBNull.Value)
                            dobavitelji_temp.Posta = Convert.ToString(reader["posta"]);
                        if (reader["drzava"] != DBNull.Value)
                            dobavitelji_temp.Drzava = Convert.ToString(reader["drzava"]);
                        if (reader["davcna_st"] != DBNull.Value)
                            dobavitelji_temp.DavcnaSt = Convert.ToString(reader["davcna_st"]);

                        dobavitelji.Add(dobavitelji_temp);
                    }
                }
                connection.Close();
                return dobavitelji;
            }
        }
    }
}
