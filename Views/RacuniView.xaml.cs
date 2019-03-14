using MySql.Data.MySqlClient;
using POSSavkovic.ViewModels;
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
    /// Interaction logic for RacuniView.xaml
    /// </summary>
    public partial class RacuniView : Window
    {
        public RacuniView()
        {
            InitializeComponent();
            lb_racuni.DataContext = new RacuniViewModel();
        }

        private void btn_preklici_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_natisni_racun_Click(object sender, RoutedEventArgs e)
        {
            if (lb_racuni.SelectedIndex != -1)
            {
                var connectionString = ConfigurationManager.ConnectionStrings["myDatabaseConnection"].ConnectionString;

                try
                {
                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        connection.ConnectionString = connectionString;
                        connection.Open();

                        var queryGetJezik = "select jezik from racuni where id = " + int.Parse(lbl_id_racuna.Content.ToString()) + ";";
                        MySqlCommand commandgetjezik = new MySqlCommand(queryGetJezik, connection);
                        int jezik = Convert.ToInt32(commandgetjezik.ExecuteScalar());

                        connection.Close();

                        if (jezik == 0)
                            NatisniSloRacun(int.Parse(lbl_id_racuna.Content.ToString()));
                        else if (jezik == 1)
                            NatisniAngRacun(int.Parse(lbl_id_racuna.Content.ToString()));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void NatisniAngRacun(int id_racuna)
        {
            NatisniRacunAng racun = new NatisniRacunAng(id_racuna);
        }

        private void NatisniSloRacun(int id_racuna)
        {
            NatisniRacun racun = new NatisniRacun(id_racuna);
        }
    }
}
