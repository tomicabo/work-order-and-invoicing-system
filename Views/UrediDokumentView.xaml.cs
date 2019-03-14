using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for UrediDokumentView.xaml
    /// </summary>
    public partial class UrediDokumentView : Window
    {
        private string opis;
        private int kol;
        private decimal cena_brez_ddv;
        private decimal ddv;
        private decimal cena_z_ddv;
        private decimal vrednost;

        private decimal izracun_skupaj_brez_ddv = 0;
        private decimal izracun_ddv = 0;
        private decimal izracun_vrednost = 0;

        private int id_delovnega_naloga;
        private string st_narocila;
        private int racun_id;
        private int dobavnica_id;
        private string status;
        private DateTime? datum_zapadlosti;
        private DateTime? datum_izdano;

        public class LVI
        {
            public string Opis { get; set; }
            public int Kol { get; set; }
            public decimal Cena_brez_ddv { get; set; }
            public decimal Ddv { get; set; }
            public decimal Cena_z_ddv { get; set; }
            public decimal Vrednost { get; set; }
        }

        ObservableCollection<LVI> lvi_list = new ObservableCollection<LVI>();

        public UrediDokumentView(int dn_id, string st_n, int r_id, int d_id, string s)
        {
            InitializeComponent();

            lv_blago.ItemsSource = lvi_list;

            id_delovnega_naloga = dn_id;
            st_narocila = st_n;
            racun_id = r_id;
            dobavnica_id = d_id;
            status = s;

            if (status == "Izdan Račun" || status == "Plačano")
            {
                dp_zapadlost.Visibility = Visibility.Visible;
                lbl_datum_zapadlosti.Visibility = Visibility.Visible;

                if (CheckIzdanRacun(r_id) == true)
                {
                    if (datum_zapadlosti != null)
                        dp_zapadlost.SelectedDate = datum_zapadlosti;
                    if (datum_izdano != null)
                        dp_izdano.SelectedDate = datum_izdano;
                }
            }
            else
            {
                dp_zapadlost.Visibility = Visibility.Hidden;
                lbl_datum_zapadlosti.Visibility = Visibility.Hidden;
            }

            tb_st_narocila.Text = st_narocila.ToString();

            NapolniListView();
            OsveziIzracun();
        }

        private bool CheckIzdanRacun(int id)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["myDatabaseConnection"].ConnectionString;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.ConnectionString = connectionString;
                    var queryGetDatum = "select datum_zap, ustvarjeno from racuni where id = " + id + ";";

                    connection.Open();

                    using (MySqlCommand command = new MySqlCommand(queryGetDatum, connection))
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader["datum_zap"] != DBNull.Value)
                                datum_zapadlosti = Convert.ToDateTime(reader["datum_zap"]);
                            if (reader["ustvarjeno"] != DBNull.Value)
                                datum_izdano = Convert.ToDateTime(reader["ustvarjeno"]);
                        }
                    }

                    connection.Close();

                    if (datum_zapadlosti != null && datum_izdano != null)
                        return true;
                    else return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return false;
        }

        private void btn_preklici_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_dodaj_blago_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (rb_kos.IsChecked == true)
                {
                    opis = tb_vrsta_blaga.Text;
                    kol = int.Parse(tb_kolicina.Text);
                    cena_brez_ddv = Math.Round(decimal.Parse(tb_cena.Text), 2);
                    ddv = Math.Round(((decimal.Parse(tb_ddv.Text) / 100) * decimal.Parse(tb_cena.Text)), 2);
                    cena_z_ddv = Math.Round((kol * (cena_brez_ddv + ddv)), 2);
                    vrednost = Math.Round((kol * cena_brez_ddv), 2);

                    lvi_list.Add(new LVI
                    {
                        Opis = opis,
                        Kol = kol,
                        Cena_brez_ddv = cena_brez_ddv,
                        Ddv = ddv,
                        Cena_z_ddv = cena_z_ddv,
                        Vrednost = vrednost
                    });

                    OsveziIzracun();
                }

                else if (rb_skupaj.IsChecked == true)
                {
                    opis = tb_vrsta_blaga.Text;
                    kol = int.Parse(tb_kolicina.Text);
                    vrednost = Math.Round(decimal.Parse(tb_cena.Text), 2);
                    cena_brez_ddv = Math.Round((vrednost / kol), 2);
                    ddv = Math.Round(((decimal.Parse(tb_ddv.Text) / 100) * cena_brez_ddv), 2);
                    cena_z_ddv = Math.Round((kol * (cena_brez_ddv + ddv)), 2);

                    lvi_list.Add(new LVI
                    {
                        Opis = opis,
                        Kol = kol,
                        Cena_brez_ddv = cena_brez_ddv,
                        Ddv = ddv,
                        Cena_z_ddv = cena_z_ddv,
                        Vrednost = vrednost
                    });

                    OsveziIzracun();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btn_odstrani_blago_Click(object sender, RoutedEventArgs e)
        {
            lvi_list.Remove((LVI)lv_blago.SelectedItem);
            OsveziIzracun();
        }

        private void OsveziIzracun()
        {
            izracun_skupaj_brez_ddv = 0;
            izracun_ddv = 0;
            izracun_vrednost = 0;

            for (int i = 0; i < lvi_list.Count; i++)
            {
                izracun_skupaj_brez_ddv = Math.Round((izracun_skupaj_brez_ddv + lvi_list[i].Vrednost), 2);
                izracun_ddv = Math.Round((izracun_ddv + (lvi_list[i].Cena_z_ddv - lvi_list[i].Vrednost)), 2);
                izracun_vrednost = Math.Round((izracun_skupaj_brez_ddv + izracun_ddv), 2);
            }

            tb_skupaj_brez_ddv.Text = izracun_skupaj_brez_ddv.ToString();
            tb_skupaj_ddv.Text = izracun_ddv.ToString();
            tb_vrednost.Text = izracun_vrednost.ToString();
        }

        private void btn_uredi_dokument_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Ali želite urediti dokument?" + Environment.NewLine + "Spremembe bodo vplivale tudi na izdan račun in dobavnico.", "Uredi Dokument", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                InsertUpdate();
                DialogResult = true;
                this.Close();
            }
        }

        private void NapolniListView()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["myDatabaseConnection"].ConnectionString;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                var query = "select b.opis, b.cena_brez_ddv, b.cena_z_ddv, b.kolicina, b.vrednost, b.ddv from blago b " +
                            "join delovni_nalogi dn on dn.blago_id = b.st_dokumenta " +
                            "where dn.id = " + id_delovnega_naloga + ";";

                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        LVI list_items = new LVI();

                        if (reader["opis"] != DBNull.Value)
                            list_items.Opis = (Convert.ToString(reader["opis"]));
                        if (reader["cena_brez_ddv"] != DBNull.Value)
                            list_items.Cena_brez_ddv = (Convert.ToDecimal(reader["cena_brez_ddv"]));
                        if (reader["cena_z_ddv"] != DBNull.Value)
                            list_items.Cena_z_ddv = (Convert.ToDecimal(reader["cena_z_ddv"]));
                        if (reader["kolicina"] != DBNull.Value)
                            list_items.Kol = (Convert.ToInt32(reader["kolicina"]));
                        if (reader["vrednost"] != DBNull.Value)
                            list_items.Vrednost = (Convert.ToDecimal(reader["vrednost"]));
                        if (reader["ddv"] != DBNull.Value)
                            list_items.Ddv = (Convert.ToDecimal(reader["ddv"]));

                        lvi_list.Add(list_items);
                    }
                    connection.Close();
                }
            }
        }

        private void InsertUpdate()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["myDatabaseConnection"].ConnectionString;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.ConnectionString = connectionString;
                    var queryGetId = "select max(id)+1 from blago;";
                    var queryGetStDokumenta = "select max(st_dokumenta)+1 from blago;";

                    var queryInsertBlago = "insert into blago (id, opis, cena_brez_ddv, cena_z_ddv, kolicina, vrednost, ddv, st_dokumenta)" +
                        "values (@Id, @Opis, @CenaBrezDDV, @CenaZDDV, @Kolicina, @Vrednost, @DDV, @StDokumenta);";


                    connection.Open();

                    MySqlCommand commandgetid = new MySqlCommand(queryGetId, connection);
                    int maxid = Convert.ToInt32(commandgetid.ExecuteScalar());

                    MySqlCommand commandgetstdokumenta = new MySqlCommand(queryGetStDokumenta, connection);
                    int st_dokumenta_blago = Convert.ToInt32(commandgetstdokumenta.ExecuteScalar());

                    foreach (var item in lv_blago.Items.OfType<LVI>())
                    {
                        using (MySqlCommand cmd = new MySqlCommand(queryInsertBlago, connection))
                        {
                            cmd.Parameters.AddWithValue("@Id", maxid);
                            cmd.Parameters.AddWithValue("@Opis", item.Opis);
                            cmd.Parameters.AddWithValue("@CenaBrezDDV", item.Cena_brez_ddv);
                            cmd.Parameters.AddWithValue("@CenaZDDV", item.Cena_z_ddv);
                            cmd.Parameters.AddWithValue("@Kolicina", item.Kol);
                            cmd.Parameters.AddWithValue("@Vrednost", item.Vrednost);
                            cmd.Parameters.AddWithValue("@DDV", item.Ddv);
                            cmd.Parameters.AddWithValue("@StDokumenta", st_dokumenta_blago);

                            cmd.ExecuteNonQuery();
                        }
                        maxid++;
                    }

                    var queryUpdateDelovniNalog = "update delovni_nalogi set blago_id = " + st_dokumenta_blago + ", st_narocila = '" + tb_st_narocila.Text + "', prihodek = " + Convert.ToString(izracun_skupaj_brez_ddv, new NumberFormatInfo() { NumberDecimalSeparator = "." }) + " where id = " + id_delovnega_naloga + ";";

                    MySqlCommand commandupdatedn = new MySqlCommand(queryUpdateDelovniNalog, connection);
                    commandupdatedn.ExecuteNonQuery();

                    if(racun_id != 0)
                    {
                        var queryUpdateRacun = "";
                        if (status == "Izdan Račun" || status == "Plačano")
                        {
                            DateTime temp = Convert.ToDateTime(dp_zapadlost.SelectedDate);
                            DateTime temp2 = Convert.ToDateTime(dp_izdano.SelectedDate);
                            queryUpdateRacun = "update racuni set ustvarjeno = '" + temp2.ToString("yyyy-MM-dd") + "', datum_zap = '" + temp.ToString("yyyy-MM-dd") + "', blago_id = " + st_dokumenta_blago + ", st_narocila = '" + tb_st_narocila.Text + "' where id = " + racun_id + ";";
                        }
                        else queryUpdateRacun = "update racuni set blago_id = " + st_dokumenta_blago + ", st_narocila = '" + tb_st_narocila.Text + "' where id = " + racun_id + ";";

                        MySqlCommand commandupdateracun = new MySqlCommand(queryUpdateRacun, connection);
                        commandupdateracun.ExecuteNonQuery();
                    }

                    if(dobavnica_id != 0)
                    {
                        var queryUpdateDobavnica = "update dobavnice set blago_id = " + st_dokumenta_blago + ", st_narocila = '" + tb_st_narocila.Text + "' where id = " + dobavnica_id + ";";

                        MySqlCommand commandupdatedobavnica = new MySqlCommand(queryUpdateDobavnica, connection);
                        commandupdatedobavnica.ExecuteNonQuery();
                    }

                    connection.Close();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
