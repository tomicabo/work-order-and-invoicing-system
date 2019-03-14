using MySql.Data.MySqlClient;
using POSSavkovic.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace POSSavkovic.Views
{
    /// <summary>
    /// Interaction logic for DodajDelovniNalogView.xaml
    /// </summary>
    public partial class DodajDelovniNalogView : Window
    {
        private int id_kupca = -1;
        private int id_delovnega_naloga;

        private int racun_id;
        private int dobavnica_id;


        public DodajDelovniNalogView(int a, int id_dn, int r_id, int d_id)
        {
            InitializeComponent();

            //10.3.2019
            cb_podjetje.DataContext = new KupciViewModel();
            //MessageBox.Show(id_kupca.ToString());

            racun_id = r_id;
            dobavnica_id = d_id;

            if (a == 0) //Ustvari
            {
                //Button_Click_2(null, null);
                btn_dodaj.Visibility = Visibility.Visible;
                btn_uredi.Visibility = Visibility.Collapsed;
            }

            else if (a == 1) //Uredi
            {
                id_delovnega_naloga = id_dn;
                btn_uredi.Visibility = Visibility.Visible;
                btn_dodaj.Visibility = Visibility.Collapsed;
                NapolniTextBoxe();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DialogResult = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["myDatabaseConnection"].ConnectionString;

            MessageBoxResult result = MessageBox.Show("Ali želite dodati Delovni Nalog?", "Dodaj Delovni Nalog", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        connection.ConnectionString = connectionString;
                        var queryGetId = "select max(id)+1 from delovni_nalogi;";

                        var queryInsertDelovniNalog = "insert into delovni_nalogi (id, opis, strosek, prihodek, dobicek, mesec, status, strosek2, strosek3, strosek4, ustvarjeno, strosek2_opis, strosek3_opis, strosek4_opis, kupec, izbrisano)" +
                            "values (@Id, @Opis, @Strosek, @Prihodek, @Dobicek, @Mesec, @Status, @Strosek2, @Strosek3, @Strosek4, @Ustvarjeno, @Strosek2Opis, @Strosek3Opis, @Strosek4Opis, @Kupec, @Izbrisano);";

                        connection.Open();

                        MySqlCommand commandgetid = new MySqlCommand(queryGetId, connection);
                        int maxid = Convert.ToInt32(commandgetid.ExecuteScalar());

                        using (MySqlCommand cmd = new MySqlCommand(queryInsertDelovniNalog, connection))
                        {
                            cmd.Parameters.AddWithValue("@Id", maxid);
                            if (tb_opis.Text != "")
                                cmd.Parameters.AddWithValue("@Opis", tb_opis.Text);
                            else cmd.Parameters.AddWithValue("@Opis", "");
                            if (tb_strosek.Text != "")
                                cmd.Parameters.AddWithValue("@Strosek", tb_strosek.Text);
                            else cmd.Parameters.AddWithValue("@Strosek", 0);
                            if (tb_prihodek.Text != "")
                                cmd.Parameters.AddWithValue("@Prihodek", tb_prihodek.Text);
                            else cmd.Parameters.AddWithValue("@Prihodek", 0);
                            cmd.Parameters.AddWithValue("@Dobicek", "0");

                            cmd.Parameters.AddWithValue("@Mesec", DateTime.Today.ToString("MM"));

                            cmd.Parameters.AddWithValue("@Status", "0");
                            if (tb_strosek2.Text != "")
                                cmd.Parameters.AddWithValue("@Strosek2", tb_strosek2.Text);
                            else cmd.Parameters.AddWithValue("@Strosek2", 0);
                            if (tb_strosek3.Text != "")
                                cmd.Parameters.AddWithValue("@Strosek3", tb_strosek3.Text);
                            else cmd.Parameters.AddWithValue("@Strosek3", 0);
                            if (tb_strosek4.Text != "")
                                cmd.Parameters.AddWithValue("@Strosek4", tb_strosek4.Text);
                            else cmd.Parameters.AddWithValue("@Strosek4", 0);
                            cmd.Parameters.AddWithValue("@Ustvarjeno", DateTime.Now);
                            if (tb_strosek2_opis.Text != "")
                                cmd.Parameters.AddWithValue("@Strosek2Opis", tb_strosek2_opis.Text);
                            else cmd.Parameters.AddWithValue("@Strosek2Opis", "");
                            if (tb_strosek3_opis.Text != "")
                                cmd.Parameters.AddWithValue("@Strosek3Opis", tb_strosek3_opis.Text);
                            else cmd.Parameters.AddWithValue("@Strosek3Opis", "");
                            if (tb_strosek4_opis.Text != "")
                                cmd.Parameters.AddWithValue("@Strosek4Opis", tb_strosek4_opis.Text);
                            else cmd.Parameters.AddWithValue("@Strosek4Opis", "");
                            if (id_kupca != 0)
                                cmd.Parameters.AddWithValue("@Kupec", id_kupca);
                            else cmd.Parameters.AddWithValue("@Kupec", 0);
                            cmd.Parameters.AddWithValue("@Izbrisano", 0);

                            cmd.ExecuteNonQuery();
                        }

                        connection.Close();
                    }
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) //PREKLIČI
        {
            this.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e) //IZBERI KUPCA
        {
            var okno = new KupciView(1);
            if (okno.ShowDialog() == true)
            {
                id_kupca = okno.id;
                lbl_id_kupca.Content = id_kupca;
                cb_podjetje.Text = okno.podjetje;
            }
        }

        private void NapolniTextBoxe()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["myDatabaseConnection"].ConnectionString;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                var query = "select dn.opis, k.podjetje, dn.strosek, dn.prihodek, dn.strosek2, dn.strosek3, dn.strosek4, " +
                    "dn.strosek2_opis, dn.strosek3_opis, dn.strosek4_opis, k.id from delovni_nalogi dn " +
                    "join kupci k on dn.kupec = k.id where dn.id = " + id_delovnega_naloga + "; ";

                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["id"] != DBNull.Value)
                        {
                            id_kupca = Convert.ToInt32(reader["id"]);
                            //lbl_id_kupca.Content = Convert.ToInt32(reader["id"]);
                        }
                        if (reader["opis"] != DBNull.Value)
                            tb_opis.Text = Convert.ToString(reader["opis"]);
                        if (reader["podjetje"] != DBNull.Value)
                            cb_podjetje.Text = Convert.ToString(reader["podjetje"]);
                        if (reader["strosek"] != DBNull.Value)
                            tb_strosek.Text = Convert.ToString(reader["strosek"], new NumberFormatInfo() { NumberDecimalSeparator = "." });
                        if (reader["prihodek"] != DBNull.Value)
                            tb_prihodek.Text = Convert.ToString(reader["prihodek"], new NumberFormatInfo() { NumberDecimalSeparator = "." });
                        if (reader["strosek2"] != DBNull.Value)
                            tb_strosek2.Text = Convert.ToString(reader["strosek2"], new NumberFormatInfo() { NumberDecimalSeparator = "." });
                        if (reader["strosek3"] != DBNull.Value)
                            tb_strosek3.Text = Convert.ToString(reader["strosek3"], new NumberFormatInfo() { NumberDecimalSeparator = "." });
                        if (reader["strosek4"] != DBNull.Value)
                            tb_strosek4.Text = Convert.ToString(reader["strosek4"], new NumberFormatInfo() { NumberDecimalSeparator = "." });
                        if (reader["strosek2_opis"] != DBNull.Value)
                            tb_strosek2_opis.Text = Convert.ToString(reader["strosek2_opis"]);
                        if (reader["strosek3_opis"] != DBNull.Value)
                            tb_strosek3_opis.Text = Convert.ToString(reader["strosek3_opis"]);
                        if (reader["strosek4_opis"] != DBNull.Value)
                            tb_strosek4_opis.Text = Convert.ToString(reader["strosek4_opis"]);
                    }
                }
                connection.Close();
            }
        }

        private void btn_uredi_Click(object sender, RoutedEventArgs e)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["myDatabaseConnection"].ConnectionString;

            MessageBoxResult result = MessageBox.Show("Ali želite urediti Delovni Nalog?", "Uredi Delovni Nalog", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        var queryUpdateDelovniNalog = "update delovni_nalogi set kupec = @Kupec, opis = @Opis, strosek = @Strosek, prihodek = @Prihodek, strosek2 = @Strosek2, strosek3 = @Strosek3, strosek4 = @strosek4, strosek2_opis = @Strosek2Opis, strosek3_opis = @Strosek3Opis, strosek4_opis = @Strosek4Opis where id = " + id_delovnega_naloga + ";";
                        
                        connection.Open();

                        using (MySqlCommand cmd = new MySqlCommand(queryUpdateDelovniNalog, connection))
                        {
                            if (id_kupca != -1)
                                cmd.Parameters.AddWithValue("@Kupec", id_kupca);
                            else cmd.Parameters.AddWithValue("@Kupec", -1);
                            if (tb_opis.Text != "")
                                cmd.Parameters.AddWithValue("@Opis", tb_opis.Text);
                            else cmd.Parameters.AddWithValue("@Opis", "");
                            if (tb_strosek.Text != "")
                                cmd.Parameters.AddWithValue("@Strosek", tb_strosek.Text);
                            else cmd.Parameters.AddWithValue("@Strosek", 0);
                            if (tb_prihodek.Text != "")
                                cmd.Parameters.AddWithValue("@Prihodek", tb_prihodek.Text);
                            else cmd.Parameters.AddWithValue("@Prihodek", 0);
                            if (tb_strosek2.Text != "")
                                cmd.Parameters.AddWithValue("@Strosek2", tb_strosek2.Text);
                            else cmd.Parameters.AddWithValue("@Strosek2", 0);
                            if (tb_strosek3.Text != "")
                                cmd.Parameters.AddWithValue("@Strosek3", tb_strosek3.Text);
                            else cmd.Parameters.AddWithValue("@Strosek3", 0);
                            if (tb_strosek4.Text != "")
                                cmd.Parameters.AddWithValue("@Strosek4", tb_strosek4.Text);
                            else cmd.Parameters.AddWithValue("@Strosek4", 0);
                            if (tb_strosek2_opis.Text != "")
                                cmd.Parameters.AddWithValue("@Strosek2Opis", tb_strosek2_opis.Text);
                            else cmd.Parameters.AddWithValue("@Strosek2Opis", "");
                            if (tb_strosek3_opis.Text != "")
                                cmd.Parameters.AddWithValue("@Strosek3Opis", tb_strosek3_opis.Text);
                            else cmd.Parameters.AddWithValue("@Strosek3Opis", "");
                            if (tb_strosek4_opis.Text != "")
                                cmd.Parameters.AddWithValue("@Strosek4Opis", tb_strosek4_opis.Text);
                            else cmd.Parameters.AddWithValue("@Strosek4Opis", "");

                            cmd.ExecuteNonQuery();
                        }

                        if (racun_id != 0)
                        {
                            var queryUpdateRacun = "update racuni set kupec = " + id_kupca + " where id = " + racun_id + ";";

                            MySqlCommand commandupdateracun = new MySqlCommand(queryUpdateRacun, connection);
                            commandupdateracun.ExecuteNonQuery();
                        }

                        if (dobavnica_id != 0)
                        {
                            var queryUpdateDobavnica = "update dobavnice set kupec = " + id_kupca + " where id = " + dobavnica_id + ";";

                            MySqlCommand commandupdatedobavnica = new MySqlCommand(queryUpdateDobavnica, connection);
                            commandupdatedobavnica.ExecuteNonQuery();
                        }

                        connection.Close();
                    }
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void Cb_podjetje_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cb_podjetje.SelectedIndex == -1)
            {
                id_kupca = -1;
            }

            if (cb_podjetje.SelectedIndex != -1)
            {
                id_kupca = int.Parse(lbl_id_kupca.Content.ToString());
            }
        }
    }
}
