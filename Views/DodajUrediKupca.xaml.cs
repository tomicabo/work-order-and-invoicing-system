using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
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
    /// Interaction logic for DodajUrediKupca.xaml
    /// </summary>
    public partial class DodajUrediKupca : Window
    {
        private int id_kupca;

        public DodajUrediKupca(int a, int id, string podjetje, string naslov, string posta, string drzava, string davcna_st)
        {
            InitializeComponent();

            if (a==0)
            {
                btn_dodaj.Visibility = Visibility.Visible;
                btn_uredi.Visibility = Visibility.Collapsed;
                this.Title = "Dodaj Kupca";
            }
            else if (a==1)
            {
                btn_dodaj.Visibility = Visibility.Collapsed;
                btn_uredi.Visibility = Visibility.Visible;

                tb_podjetje.Text = podjetje;
                tb_naslov.Text = naslov;
                tb_posta.Text = posta;
                tb_drzava.Text = drzava;
                tb_davcna_st.Text = davcna_st;

                id_kupca = id;
                this.Title = "Uredi Kupca";
            }
        }

        private void btn_preklici_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DialogResult = true;
        }

        private void btn_dodaj_Click(object sender, RoutedEventArgs e)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["myDatabaseConnection"].ConnectionString;

            MessageBoxResult result = MessageBox.Show("Ali želite dodati kupca?", "Dodaj kupca", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        connection.ConnectionString = connectionString;
                        var queryGetId = "select max(id)+1 from kupci;";

                        var queryInsertKupec = "insert into kupci (id, podjetje, naslov, posta, drzava, davcna_st, izbrisano)" +
                            "values (@Id, @Podjetje, @Naslov, @Posta, @Drzava, @DavcnaSt, 0);";

                        connection.Open();

                        MySqlCommand commandgetid = new MySqlCommand(queryGetId, connection);
                        int maxid = Convert.ToInt32(commandgetid.ExecuteScalar());

                        using (MySqlCommand cmd = new MySqlCommand(queryInsertKupec, connection))
                        {
                            cmd.Parameters.AddWithValue("@Id", maxid);
                            if (tb_podjetje.Text != "")
                                cmd.Parameters.AddWithValue("@Podjetje", tb_podjetje.Text);
                            else cmd.Parameters.AddWithValue("@Podjetje", "");
                            if (tb_naslov.Text != "")
                                cmd.Parameters.AddWithValue("@Naslov", tb_naslov.Text);
                            else cmd.Parameters.AddWithValue("@Naslov", "");
                            if (tb_posta.Text != "")
                                cmd.Parameters.AddWithValue("@Posta", tb_posta.Text);
                            else cmd.Parameters.AddWithValue("@Posta", "");
                            if (tb_drzava.Text != "")
                                cmd.Parameters.AddWithValue("@Drzava", tb_drzava.Text);
                            else cmd.Parameters.AddWithValue("@Drzava", "");
                            if (tb_davcna_st.Text != "")
                                cmd.Parameters.AddWithValue("@DavcnaSt", tb_davcna_st.Text);
                            else cmd.Parameters.AddWithValue("@DavcnaSt", "");

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

        private void btn_uredi_Click(object sender, RoutedEventArgs e)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["myDatabaseConnection"].ConnectionString;

            MessageBoxResult result = MessageBox.Show("Ali želite urediti kupca?", "Uredi kupca", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        var queryUpdateKupec = "update kupci set podjetje = @Podjetje, naslov = @Naslov, posta = @Posta, drzava = @Drzava, davcna_st = @DavcnaSt where id = " + id_kupca + ";";

                        connection.Open();

                        using (MySqlCommand cmd = new MySqlCommand(queryUpdateKupec, connection))
                        {
                            if (tb_podjetje.Text != "")
                                cmd.Parameters.AddWithValue("@Podjetje", tb_podjetje.Text);
                            else cmd.Parameters.AddWithValue("@Podjetje", "");
                            if (tb_naslov.Text != "")
                                cmd.Parameters.AddWithValue("@Naslov", tb_naslov.Text);
                            else cmd.Parameters.AddWithValue("@Naslov", "");
                            if (tb_posta.Text != "")
                                cmd.Parameters.AddWithValue("@Posta", tb_posta.Text);
                            else cmd.Parameters.AddWithValue("@Posta", "");
                            if (tb_drzava.Text != "")
                                cmd.Parameters.AddWithValue("@Drzava", tb_drzava.Text);
                            else cmd.Parameters.AddWithValue("@Drzava", "");
                            if (tb_davcna_st.Text != "")
                                cmd.Parameters.AddWithValue("@DavcnaSt", tb_davcna_st.Text);
                            else cmd.Parameters.AddWithValue("@DavcnaSt", "");

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
    }
}
