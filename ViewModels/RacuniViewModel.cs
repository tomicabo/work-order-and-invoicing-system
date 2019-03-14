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
    class RacuniViewModel
    {
        public ObservableCollection<RacuniModel> racuni = new ObservableCollection<RacuniModel>();
        public ObservableCollection<RacuniModel> Racuni
        {
            get;
            set;
        }

        public RacuniViewModel()
        {
            DobiRacune();
            Racuni = racuni;
        }

        public ObservableCollection<RacuniModel> DobiRacune()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["myDatabaseConnection"].ConnectionString;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                var query = "select r.id, r.ustvarjeno, r.st_racuna, r.skupaj_z_ddv, r.skupaj_brez_ddv, k.podjetje, r.datum_zap, r.st_narocila " +
                            "from racuni r " +
                            "join kupci k on r.kupec = k.id order by id; ";

                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        RacuniModel r_temp = new RacuniModel();
                        if (reader["id"] != DBNull.Value)
                            r_temp.Id = Convert.ToInt32(reader["id"]);
                        if (reader["ustvarjeno"] != DBNull.Value)
                        {
                            DateTime temp = Convert.ToDateTime(reader["ustvarjeno"]);
                            r_temp.Ustvarjeno = temp.ToString("dd.MM.yyyy");
                        }
                        if (reader["st_racuna"] != DBNull.Value)
                        {
                            DateTime temp = Convert.ToDateTime(reader["ustvarjeno"]);
                            r_temp.StRacuna = Convert.ToString(reader["st_racuna"]) + "-" + temp.ToString("yyyy");
                        }
                        if (reader["skupaj_z_ddv"] != DBNull.Value)
                            r_temp.SkupajZDDV = Convert.ToDecimal(reader["skupaj_z_ddv"]);
                        if (reader["skupaj_brez_ddv"] != DBNull.Value)
                            r_temp.SkupajBrezDDV = Convert.ToDecimal(reader["skupaj_brez_ddv"]);
                        if (reader["podjetje"] != DBNull.Value)
                            r_temp.Kupec = Convert.ToString(reader["podjetje"]);
                        if (reader["datum_zap"] != DBNull.Value)
                        {
                            DateTime temp = Convert.ToDateTime(reader["datum_zap"]);
                            r_temp.DatumZapadlosti = temp.ToString("dd.MM.yyyy");
                        }
                        if (reader["st_narocila"] != DBNull.Value)
                            r_temp.StNarocila = Convert.ToString(reader["st_narocila"]);

                        racuni.Add(r_temp);
                    }
                }
                connection.Close();
                return racuni;
            }
        }
    }
}
