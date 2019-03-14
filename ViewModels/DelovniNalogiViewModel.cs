using POSSavkovic.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Windows;

namespace POSSavkovic.ViewModels
{
    class DelovniNalogiViewModel
    {
        public ObservableCollection<DelovniNalogiModel> delovni_nalogi = new ObservableCollection<DelovniNalogiModel>();
        public ObservableCollection<DelovniNalogiModel> DelovniNalogi
        {
            get;
            set;
        }

        public DelovniNalogiViewModel()
        {
            DobiDelovneNaloge();
            DelovniNalogi = delovni_nalogi;
        }

        public ObservableCollection<DelovniNalogiModel> DobiDelovneNaloge()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["myDatabaseConnection"].ConnectionString;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    //var query = "select * from delovni_nalogi";
                    var query = "select dn.id as id, dn.opis, k.id as kupec_id, k.podjetje, dn.strosek, dn.prihodek, dn.dobicek, dn.status, dn.strosek2, dn.strosek3, dn.strosek4, " +
                        "dn.ustvarjeno, dn.strosek2_opis, dn.strosek3_opis, dn.strosek4_opis, dn.racun_id, dn.dobavnica_id, dn.st_narocila from delovni_nalogi dn " +
                        "join kupci k on dn.kupec = k.id where dn.izbrisano != 1 order by ustvarjeno; ";

                    connection.Open();
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DelovniNalogiModel dn_temp = new DelovniNalogiModel();
                            if (reader["id"] != DBNull.Value)
                                dn_temp.Id = Convert.ToInt32(reader["id"]);
                            if (reader["opis"] != DBNull.Value)
                                dn_temp.Opis = Convert.ToString(reader["opis"]);
                            if (reader["kupec_id"] != DBNull.Value)
                                dn_temp.KupecId = Convert.ToInt32(reader["kupec_id"]);
                            if (reader["podjetje"] != DBNull.Value)
                                dn_temp.Kupec = Convert.ToString(reader["podjetje"]);

                            if (reader["strosek"] != DBNull.Value)
                            {
                                dn_temp.Strosek = Convert.ToDecimal(reader["strosek"]) +
                                    Convert.ToDecimal(reader["strosek2"]) +
                                    Convert.ToDecimal(reader["strosek3"]) +
                                    Convert.ToDecimal(reader["strosek4"]);
                            }
                            if (reader["prihodek"] != DBNull.Value)
                                dn_temp.Prihodek = Convert.ToDecimal(reader["prihodek"]);

                            dn_temp.Dobicek = dn_temp.Prihodek - dn_temp.Strosek;

                            if (reader["ustvarjeno"] != DBNull.Value)
                            {
                                DateTime temp = Convert.ToDateTime(reader["ustvarjeno"]);
                                dn_temp.Ustvarjeno = temp.ToString("dd.MM.yyyy");
                            }
                            if (reader["ustvarjeno"] != DBNull.Value)
                            {
                                DateTime temp = Convert.ToDateTime(reader["ustvarjeno"]);
                                dn_temp.Mesec = temp.ToString("MMMM");
                            }
                            if (reader["status"] != DBNull.Value)
                            {
                                if (Convert.ToInt32(reader["status"]) == 0)
                                    dn_temp.Status = "";
                                if (Convert.ToInt32(reader["status"]) == 1)
                                    dn_temp.Status = "Pripravljen Dokument";
                                if (Convert.ToInt32(reader["status"]) == 2)
                                    dn_temp.Status = "Izdan Račun";
                                if (Convert.ToInt32(reader["status"]) == 3)
                                    dn_temp.Status = "Plačano";
                            }
                            if (reader["strosek2"] != DBNull.Value)
                                dn_temp.Strosek2 = Convert.ToDecimal(reader["strosek2"]);
                            if (reader["strosek3"] != DBNull.Value)
                                dn_temp.Strosek3 = Convert.ToDecimal(reader["strosek3"]);
                            if (reader["strosek4"] != DBNull.Value)
                                dn_temp.Strosek4 = Convert.ToDecimal(reader["strosek4"]);
                            if (reader["strosek2_opis"] != DBNull.Value)
                                dn_temp.Strosek2Opis = Convert.ToString(reader["strosek2_opis"]);
                            if (reader["strosek3_opis"] != DBNull.Value)
                                dn_temp.Strosek3Opis = Convert.ToString(reader["strosek3_opis"]);
                            if (reader["strosek4_opis"] != DBNull.Value)
                                dn_temp.Strosek4Opis = Convert.ToString(reader["strosek4_opis"]);
                            if (reader["racun_id"] != DBNull.Value)
                                dn_temp.RacunId = Convert.ToInt32(reader["racun_id"]);
                            else dn_temp.RacunId = 0;
                            if (reader["dobavnica_id"] != DBNull.Value)
                                dn_temp.DobavnicaId = Convert.ToInt32(reader["dobavnica_id"]);
                            else dn_temp.DobavnicaId = 0;
                            if (reader["st_narocila"] != DBNull.Value)
                                dn_temp.StNarocila = Convert.ToString(reader["st_narocila"]);
                            else dn_temp.StNarocila = "";

                            delovni_nalogi.Add(dn_temp);
                        }
                    }
                    connection.Close();
                    return delovni_nalogi;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return delovni_nalogi;
            }
        }
    }
}
