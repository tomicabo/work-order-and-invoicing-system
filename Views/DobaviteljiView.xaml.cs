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
    /// Interaction logic for DobaviteljiView.xaml
    /// </summary>
    public partial class DobaviteljiView : Window
    {
        public int id;
        public string podjetje;
        private string naslov;
        private string posta;
        private string drzava;
        private string davcna_st;

        public DobaviteljiView(int a)
        {
            InitializeComponent();
            lv_dobavitelji.DataContext = new DobaviteljiViewModel();

            if (lv_dobavitelji.SelectedIndex == -1)
            {
                btn_uredi_dobavitelja.IsEnabled = false;
                btn_izbrisi_dobavitelja.IsEnabled = false;
                btn_izberi_dobavitelja.IsEnabled = false;
            }
            else
            {
                btn_uredi_dobavitelja.IsEnabled = true;
                btn_izbrisi_dobavitelja.IsEnabled = true;
                btn_izberi_dobavitelja.IsEnabled = true;
            }

            if (a == 0)
            {
                btn_izberi_dobavitelja.Visibility = Visibility.Hidden;
                this.Title = "Dobavitelji";
            }
            else if (a == 1)
            {
                btn_izberi_dobavitelja.Visibility = Visibility.Visible;
                this.Title = "Izberi dobavitelja";
            }
        }

        private void lv_dobavitelji_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lv_dobavitelji.SelectedIndex != -1)
            {
                btn_uredi_dobavitelja.IsEnabled = true;
                btn_izbrisi_dobavitelja.IsEnabled = true;
                btn_izberi_dobavitelja.IsEnabled = true;

                id = int.Parse(lbl_id.Content.ToString());
                podjetje = lbl_podjetje.Content.ToString();
                naslov = lbl_naslov.Content.ToString();
                posta = lbl_posta.Content.ToString();
                drzava = lbl_drzava.Content.ToString();
                davcna_st = lbl_davcna.Content.ToString();
            }
            else
            {
                btn_uredi_dobavitelja.IsEnabled = false;
                btn_izbrisi_dobavitelja.IsEnabled = false;
                btn_izberi_dobavitelja.IsEnabled = false;
            }
        }

        private void btn_dodaj_dobavitelja_Click(object sender, RoutedEventArgs e)
        {
            DodajUrediDobavitelja okno = new DodajUrediDobavitelja(0, 0, "", "", "", "", "");
            okno.ShowDialog();

            if (okno.DialogResult == true)
                lv_dobavitelji.DataContext = new DobaviteljiViewModel();
        }

        private void btn_uredi_dobavitelja_Click(object sender, RoutedEventArgs e)
        {
            DodajUrediDobavitelja okno = new DodajUrediDobavitelja(1, id, podjetje, naslov, posta, drzava, davcna_st);
            okno.ShowDialog();

            if (okno.DialogResult == true)
                lv_dobavitelji.DataContext = new DobaviteljiViewModel();
        }

        private void btn_izbrisi_dobavitelja_Click(object sender, RoutedEventArgs e)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["myDatabaseConnection"].ConnectionString;

            MessageBoxResult result = MessageBox.Show("Ali želite izbrisati dobavitelja?", "Izbriši dobavitelja", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        var queryUpdateKupec = "update dobavitelji set izbrisano = 1 where id = " + id + ";";

                        connection.Open();

                        using (MySqlCommand cmd = new MySqlCommand(queryUpdateKupec, connection))
                        {
                            cmd.ExecuteNonQuery();
                        }
                        connection.Close();

                        MessageBox.Show("Dobavitelj je bil izbrisan");
                    }

                    lv_dobavitelji.DataContext = new DobaviteljiViewModel();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btn_preklici_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_izberi_dobavitelja_Click(object sender, RoutedEventArgs e)
        {
            if (lv_dobavitelji.SelectedIndex != -1)
                this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DialogResult = true;
        }
    }
}
