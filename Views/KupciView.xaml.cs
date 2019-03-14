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
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace POSSavkovic.Views
{
    /// <summary>
    /// Interaction logic for KupciView.xaml
    /// </summary>
    public partial class KupciView : Window
    {
        public int id;
        public string podjetje;
        private string naslov;
        private string posta;
        private string drzava;
        private string davcna_st;

        private bool urejanje = false;

        public KupciView(int a)
        {
            InitializeComponent();
            lv_kupci.DataContext = new KupciViewModel();

            if (lv_kupci.SelectedIndex == -1)
            {
                btn_uredi_kupca.IsEnabled = false;
                btn_izbrisi_kupca.IsEnabled = false;
                btn_izberi_kupca.IsEnabled = false;
            }
            else
            {
                btn_uredi_kupca.IsEnabled = true;
                btn_izbrisi_kupca.IsEnabled = true;
                btn_izberi_kupca.IsEnabled = true;
            }

            if (a == 0)
            {
                btn_izberi_kupca.Visibility = Visibility.Hidden;
                this.Title = "Kupci";
            }
            else if (a == 1)
            {
                urejanje = true;
                btn_izberi_kupca.Visibility = Visibility.Visible;
                this.Title = "Izberi Kupca";
            }
        }

        private void lv_kupci_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lv_kupci.SelectedIndex != -1)
            {
                btn_uredi_kupca.IsEnabled = true;
                btn_izbrisi_kupca.IsEnabled = true;
                btn_izberi_kupca.IsEnabled = true;

                id = int.Parse(lbl_id.Content.ToString());
                podjetje = lbl_podjetje.Content.ToString();
                naslov = lbl_naslov.Content.ToString();
                posta = lbl_posta.Content.ToString();
                drzava = lbl_drzava.Content.ToString();
                davcna_st = lbl_davcna.Content.ToString();
            }
            else
            {
                btn_uredi_kupca.IsEnabled = false;
                btn_izbrisi_kupca.IsEnabled = false;
                btn_izberi_kupca.IsEnabled = false;
            }
        }

        private void btn_dodaj_kupca_Click(object sender, RoutedEventArgs e)
        {
            DodajUrediKupca okno = new DodajUrediKupca(0, 0, "", "", "", "", "");
            okno.ShowDialog();

            if (okno.DialogResult == true)
                lv_kupci.DataContext = new KupciViewModel();
        }

        private void btn_uredi_kupca_Click(object sender, RoutedEventArgs e)
        {
            DodajUrediKupca okno = new DodajUrediKupca(1, id, podjetje, naslov, posta, drzava, davcna_st);
            okno.ShowDialog();

            if (okno.DialogResult == true)
                lv_kupci.DataContext = new KupciViewModel();
        }

        private void btn_izbrisi_kupca_Click(object sender, RoutedEventArgs e)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["myDatabaseConnection"].ConnectionString;

            MessageBoxResult result = MessageBox.Show("Ali želite izbrisati kupca?", "Izbriši kupca", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        var queryUpdateKupec = "update kupci set izbrisano = 1 where id = " + id + ";";

                        connection.Open();

                        using (MySqlCommand cmd = new MySqlCommand(queryUpdateKupec, connection))
                        {
                            cmd.ExecuteNonQuery();
                        }
                        connection.Close();
                        MessageBox.Show("Kupec je bil izbrisan");
                    }
                    lv_kupci.DataContext = new KupciViewModel();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btn_izberi_kupca_Click(object sender, RoutedEventArgs e)
        {
            if (lv_kupci.SelectedIndex != -1)
            {
                DialogResult = true;
                this.Close();
            }
        }

        protected void HandleDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var track = ((ListViewItem)sender).Content as Track;

            if (urejanje == true)
            {
                if (lv_kupci.SelectedIndex != -1)
                {
                    DialogResult = true;
                    this.Close();
                }
            }
        }

        private void btn_preklici_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
