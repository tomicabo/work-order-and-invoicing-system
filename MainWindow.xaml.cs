using MySql.Data.MySqlClient;
using POSSavkovic.ViewModels;
using POSSavkovic.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace POSSavkovic
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int id_dn;
        private int id_kupca;
        private int id_racuna;
        private int id_dobavnice;
        private string st_narocila;
        private int jezik;

        public MainWindow()
        {
            InitializeComponent();
            lb_delovni_nalogi.DataContext = new DelovniNalogiViewModel();
            lb_izracun_po_mescih.DataContext = new IzracunPoMescihViewModel();

            IzracunSkupaj();
        }

        private void lb_delovni_nalogi_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbl_status.Content != null)
            {
                if (lbl_status.Content.ToString() == "")
                {
                    btn_uredi_dn.IsEnabled = true;
                    btn_izbrisi_dn.IsEnabled = true;
                    btn_ustvari_dokument.IsEnabled = true;
                    btn_uredi_dokument.IsEnabled = false;
                    btn_izdaj_dobavnico.IsEnabled = false;
                    btn_izdaj_racun.IsEnabled = false;
                    btn_placano.IsEnabled = false;
                    btn_natisni_dobavnico.IsEnabled = false;
                    btn_natisni_racun.IsEnabled = false;
                }

                if (lbl_status.Content.ToString() == "Pripravljen Dokument")
                {
                    btn_uredi_dn.IsEnabled = true;
                    btn_izbrisi_dn.IsEnabled = true;
                    btn_ustvari_dokument.IsEnabled = false;
                    btn_uredi_dokument.IsEnabled = true;
                    if (lbl_dobavnica_id.Content.ToString() == "0")
                        btn_izdaj_dobavnico.IsEnabled = true;
                    else btn_izdaj_dobavnico.IsEnabled = false;
                    btn_izdaj_racun.IsEnabled = true;
                    btn_placano.IsEnabled = false;
                    if (lbl_dobavnica_id.Content.ToString() == "0")
                        btn_natisni_dobavnico.IsEnabled = false;
                    else btn_natisni_dobavnico.IsEnabled = true;
                    btn_natisni_racun.IsEnabled = false;
                }

                if (lbl_status.Content.ToString() == "Izdan Račun")
                {
                    btn_uredi_dn.IsEnabled = true;
                    btn_izbrisi_dn.IsEnabled = true;
                    btn_ustvari_dokument.IsEnabled = false;
                    btn_uredi_dokument.IsEnabled = true;
                    if (lbl_dobavnica_id.Content.ToString() == "0")
                        btn_izdaj_dobavnico.IsEnabled = true;
                    else btn_izdaj_dobavnico.IsEnabled = false;
                    btn_izdaj_racun.IsEnabled = false;
                    btn_placano.IsEnabled = true;
                    if (lbl_dobavnica_id.Content.ToString() == "0")
                        btn_natisni_dobavnico.IsEnabled = false;
                    else btn_natisni_dobavnico.IsEnabled = true;
                    if (lbl_racun_id.Content.ToString() == "0")
                        btn_natisni_racun.IsEnabled = false;
                    else btn_natisni_racun.IsEnabled = true;
                }

                if (lbl_status.Content.ToString() == "Plačano")
                {
                    btn_uredi_dn.IsEnabled = true;
                    btn_izbrisi_dn.IsEnabled = true;
                    btn_ustvari_dokument.IsEnabled = false;
                    btn_uredi_dokument.IsEnabled = true;
                    if (lbl_dobavnica_id.Content.ToString() == "0")
                        btn_izdaj_dobavnico.IsEnabled = true;
                    else btn_izdaj_dobavnico.IsEnabled = false;
                    btn_izdaj_racun.IsEnabled = false;
                    btn_placano.IsEnabled = false;
                    if (lbl_dobavnica_id.Content.ToString() == "0")
                        btn_natisni_dobavnico.IsEnabled = false;
                    else btn_natisni_dobavnico.IsEnabled = true;
                    if (lbl_racun_id.Content.ToString() == "0")
                        btn_natisni_racun.IsEnabled = false;
                    else btn_natisni_racun.IsEnabled = true;
                }

                id_dn = int.Parse(lbl_id.Content.ToString());
                id_kupca = int.Parse(lbl_kupec_id.Content.ToString());
                st_narocila = lbl_st_narocila.Content.ToString();

            }
            else
            {
                btn_uredi_dn.IsEnabled = false;
                btn_izbrisi_dn.IsEnabled = false;
                btn_ustvari_dokument.IsEnabled = false;
                btn_uredi_dokument.IsEnabled = false;
                btn_izdaj_dobavnico.IsEnabled = false;
                btn_izdaj_racun.IsEnabled = false;
                btn_placano.IsEnabled = false;
                btn_natisni_dobavnico.IsEnabled = false;
                btn_natisni_racun.IsEnabled = false;

                id_dn = 0;
                id_kupca = 0;
                st_narocila = "";
            }
        }

        private void kupci_okno_Click(object sender, RoutedEventArgs e)
        {
            KupciView okno = new KupciView(0);
            okno.ShowDialog();
        }

        private void nov_delovni_nalog_Click(object sender, RoutedEventArgs e)
        {
            DodajDelovniNalogView okno = new DodajDelovniNalogView(0, 0, 0, 0);
            okno.ShowDialog();

            if (okno.DialogResult == true)
            {
                lb_delovni_nalogi.DataContext = new DelovniNalogiViewModel();
                lb_izracun_po_mescih.DataContext = new IzracunPoMescihViewModel();
                IzracunSkupaj();
            }
        }

        private void btn_izbrisi_dn_Click(object sender, RoutedEventArgs e)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["myDatabaseConnection"].ConnectionString;

            MessageBoxResult result = MessageBox.Show("Ali želite izbrisati Delovni Nalog?", "Izbriši Delovni Nalog", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        connection.ConnectionString = connectionString;
                        var queryUpdateIzbrisano = "update delovni_nalogi set izbrisano = 1 where id = " + id_dn + ";";

                        connection.Open();

                        using (MySqlCommand cmd = new MySqlCommand(queryUpdateIzbrisano, connection))
                        {
                            cmd.ExecuteNonQuery();
                        }
                        connection.Close();
                        MessageBox.Show("Delovni Nalog je bil izbrisan");
                    }

                    lb_delovni_nalogi.DataContext = new DelovniNalogiViewModel();
                    lb_izracun_po_mescih.DataContext = new IzracunPoMescihViewModel();
                    IzracunSkupaj();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btn_uredi_dn_Click(object sender, RoutedEventArgs e)
        {
            if (lb_delovni_nalogi.SelectedIndex != -1)
            {
                DodajDelovniNalogView okno = new DodajDelovniNalogView(1, id_dn, int.Parse(lbl_racun_id.Content.ToString()),
                    int.Parse(lbl_dobavnica_id.Content.ToString()));
                okno.ShowDialog();

                if (okno.DialogResult == true)
                {
                    lb_delovni_nalogi.DataContext = new DelovniNalogiViewModel();
                    lb_izracun_po_mescih.DataContext = new IzracunPoMescihViewModel();
                    IzracunSkupaj();
                }
            }
        }

        protected void HandleDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var track = ((ListViewItem)sender).Content as Track;

            if (lb_delovni_nalogi.SelectedIndex != -1)
            {
                DodajDelovniNalogView okno = new DodajDelovniNalogView(1, id_dn, int.Parse(lbl_racun_id.Content.ToString()),
                    int.Parse(lbl_dobavnica_id.Content.ToString()));
                okno.ShowDialog();

                if (okno.DialogResult == true)
                {
                    lb_delovni_nalogi.DataContext = new DelovniNalogiViewModel();
                    lb_izracun_po_mescih.DataContext = new IzracunPoMescihViewModel();
                    IzracunSkupaj();
                }
            }
        }

        private void btn_ustvari_dokument_Click(object sender, RoutedEventArgs e)
        {
            if (lb_delovni_nalogi.SelectedIndex != -1)
            {
                UstvariDokumentView okno = new UstvariDokumentView(id_dn);
                okno.ShowDialog();

                if (okno.DialogResult == true)
                {
                    lb_delovni_nalogi.DataContext = new DelovniNalogiViewModel();
                    lb_izracun_po_mescih.DataContext = new IzracunPoMescihViewModel();
                    IzracunSkupaj();
                }
            }
        }

        private void btn_uredi_dokument_Click(object sender, RoutedEventArgs e)
        {
            if (lb_delovni_nalogi.SelectedIndex != -1)
            {
                UrediDokumentView okno = new UrediDokumentView(id_dn, lbl_st_narocila.Content.ToString(), int.Parse(lbl_racun_id.Content.ToString()), int.Parse(lbl_dobavnica_id.Content.ToString()), lbl_status.Content.ToString());
                okno.ShowDialog();

                if (okno.DialogResult == true)
                {
                    lb_delovni_nalogi.DataContext = new DelovniNalogiViewModel();
                    lb_izracun_po_mescih.DataContext = new IzracunPoMescihViewModel();
                    IzracunSkupaj();
                }
            }
        }

        private void btn_izdaj_racun_Click(object sender, RoutedEventArgs e)
        {
            int id = id_dn;

            IzdajRacunView okno = new IzdajRacunView();
            okno.ShowDialog();

            if (okno.DialogResult == true)
            {
                DateTime? datum_zapadlosti = okno.DatumZapadlosti;
                DateTime? datum_izdaje = okno.DatumIzdaje;
                InsertRacun(id, id_kupca, datum_zapadlosti, datum_izdaje, okno.Jezik);
                lb_delovni_nalogi.DataContext = new DelovniNalogiViewModel();
                lb_izracun_po_mescih.DataContext = new IzracunPoMescihViewModel();

                NatisniRacun(id);
                IzracunSkupaj();
            }
        }

        private void btn_izdaj_dobavnico_Click(object sender, RoutedEventArgs e)
        {
            int id = id_dn;

            MessageBoxResult result = MessageBox.Show("Ali želite izdati dobavnico?", "Izdaj Dobavnico", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                InsertDobavnica(id, id_kupca);
                lb_delovni_nalogi.DataContext = new DelovniNalogiViewModel();
                lb_izracun_po_mescih.DataContext = new IzracunPoMescihViewModel();

                NatisniDobavnico(id);
                IzracunSkupaj();
            }
        }

        private void InsertRacun(int id_delovnega_naloga, int id_k, DateTime? datum_zapadlosti, DateTime? datum_izdaje, int jezik)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["myDatabaseConnection"].ConnectionString;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.ConnectionString = connectionString;
                    connection.Open();

                    //Insert racuni
                    var queryGetId = "select max(id)+1 from racuni;";
                    MySqlCommand commandgetid = new MySqlCommand(queryGetId, connection);
                    int maxid = Convert.ToInt32(commandgetid.ExecuteScalar());

                    var queryGetBlagoId = "select blago_id from delovni_nalogi where id = " + id_delovnega_naloga + ";";
                    MySqlCommand commandgetstdokumenta = new MySqlCommand(queryGetBlagoId, connection);
                    int st_dokumenta = Convert.ToInt32(commandgetstdokumenta.ExecuteScalar());

                    var queryGetSumSkupajZDDV = "select vrednost from blago where st_dokumenta = " + st_dokumenta + ";";
                    MySqlCommand commandgetskupajzddv = new MySqlCommand(queryGetSumSkupajZDDV, connection);
                    decimal skupaj_z_ddv = Convert.ToDecimal(commandgetskupajzddv.ExecuteScalar());

                    var queryGetSumSkupajBrezDDV = "select SUM(cena_brez_ddv) from blago where st_dokumenta = " + st_dokumenta + ";";
                    MySqlCommand commandgetskupajbrezddv = new MySqlCommand(queryGetSumSkupajBrezDDV, connection);
                    decimal skupaj_brez_ddv = Convert.ToDecimal(commandgetskupajbrezddv.ExecuteScalar());

                    var queryGetStRacuna = "select max(st_racuna) from racuni where YEAR(ustvarjeno) = YEAR(CURDATE());";
                    MySqlCommand commandgetstr = new MySqlCommand(queryGetStRacuna, connection);
                    //int st_racuna = Convert.ToInt32(commandgetstr.ExecuteScalar());
                    int st_racuna;
                    if (commandgetstr.ExecuteScalar() != DBNull.Value)
                        st_racuna = Convert.ToInt32(commandgetstr.ExecuteScalar()) + 1;
                    else st_racuna = 1;

                    var queryInsertRacuni = "insert into racuni (id, ustvarjeno, st_racuna, skupaj_z_ddv, skupaj_brez_ddv, kupec, datum_zap, blago_id, st_narocila, jezik)" +
                        "values (@Id, @Ustvarjeno, @StRacuna, @SkupajZDDV, @SkupajBrezDDV, @Kupec, @DatumZapadlosti, @BlagoId, @StNarocila, @Jezik);";

                    using (MySqlCommand cmd = new MySqlCommand(queryInsertRacuni, connection))
                    {
                        cmd.Parameters.AddWithValue("@Id", maxid);
                        cmd.Parameters.AddWithValue("@Ustvarjeno", datum_izdaje);
                        cmd.Parameters.AddWithValue("@StRacuna", st_racuna);
                        cmd.Parameters.AddWithValue("@SkupajZDDV", skupaj_z_ddv);
                        cmd.Parameters.AddWithValue("@SkupajBrezDDV", skupaj_brez_ddv);
                        cmd.Parameters.AddWithValue("@Kupec", id_k);
                        cmd.Parameters.AddWithValue("@DatumZapadlosti", datum_zapadlosti);
                        cmd.Parameters.AddWithValue("@BlagoId", st_dokumenta);
                        cmd.Parameters.AddWithValue("@StNarocila", st_narocila);
                        cmd.Parameters.AddWithValue("@Jezik", jezik);

                        cmd.ExecuteNonQuery();
                    }

                    //Update status delovni_nalogi and prihodek
                    var queryUpdateDelovniNalog = "update delovni_nalogi set status = 2, prihodek = " + Convert.ToString(skupaj_brez_ddv, new NumberFormatInfo() { NumberDecimalSeparator = "." }) + ", racun_id = " + maxid + " where id = " + id_delovnega_naloga + ";";
                    MySqlCommand commandupdatedn = new MySqlCommand(queryUpdateDelovniNalog, connection);
                    commandupdatedn.ExecuteNonQuery();

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void InsertDobavnica(int id_delovnega_naloga, int id_k)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["myDatabaseConnection"].ConnectionString;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.ConnectionString = connectionString;
                    connection.Open();

                    var queryGetId = "select max(id)+1 from dobavnice;";
                    MySqlCommand commandgetid = new MySqlCommand(queryGetId, connection);
                    int maxid = Convert.ToInt32(commandgetid.ExecuteScalar());

                    var queryGetBlagoId = "select blago_id from delovni_nalogi where id = " + id_delovnega_naloga + ";";
                    MySqlCommand commandgetstdokumenta = new MySqlCommand(queryGetBlagoId, connection);
                    int st_dokumenta = Convert.ToInt32(commandgetstdokumenta.ExecuteScalar());

                    var queryGetStDobavnice = "select max(st_dobavnice) from dobavnice where YEAR(ustvarjeno) = YEAR(CURDATE());";
                    MySqlCommand commandgetstd = new MySqlCommand(queryGetStDobavnice, connection);
                    int st_dobavnice;
                    if (commandgetstd.ExecuteScalar() != DBNull.Value)
                        st_dobavnice = Convert.ToInt32(commandgetstd.ExecuteScalar()) + 1;
                    else st_dobavnice = 1;

                    var queryInsertBlago = "insert into dobavnice (id, ustvarjeno, st_dobavnice, kupec, st_narocila, blago_id)" +
                        "values (@Id, @Ustvarjeno, @StDobavnice, @Kupec, @StNarocila, @BlagoId);";

                    using (MySqlCommand cmd = new MySqlCommand(queryInsertBlago, connection))
                    {
                        cmd.Parameters.AddWithValue("@Id", maxid);
                        cmd.Parameters.AddWithValue("@Ustvarjeno", DateTime.Now);
                        cmd.Parameters.AddWithValue("@StDobavnice", st_dobavnice);
                        cmd.Parameters.AddWithValue("@Kupec", id_k);
                        cmd.Parameters.AddWithValue("@StNarocila", st_narocila);
                        cmd.Parameters.AddWithValue("@BlagoId", st_dokumenta);

                        cmd.ExecuteNonQuery();
                    }

                    var queryUpdateDelovniNalog = "update delovni_nalogi set dobavnica_id = " + maxid + " where id = " + id_delovnega_naloga + ";";

                    MySqlCommand commandupdatedn = new MySqlCommand(queryUpdateDelovniNalog, connection);
                    commandupdatedn.ExecuteNonQuery();

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btn_placano_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Ali želite označiti delovni nalog kot plačano?", "Plačan Račun", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                var connectionString = ConfigurationManager.ConnectionStrings["myDatabaseConnection"].ConnectionString;

                try
                {
                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        connection.ConnectionString = connectionString;
                        connection.Open();

                        var queryUpdateDelovniNalog = "update delovni_nalogi set status = 3 where id = " + id_dn + ";";
                        MySqlCommand commandupdatedn = new MySqlCommand(queryUpdateDelovniNalog, connection);
                        commandupdatedn.ExecuteNonQuery();

                        connection.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                lb_delovni_nalogi.DataContext = new DelovniNalogiViewModel();
                lb_izracun_po_mescih.DataContext = new IzracunPoMescihViewModel();
            }
        }

        private void racuni_okno_Click(object sender, RoutedEventArgs e)
        {
            RacuniView okno = new RacuniView();
            okno.Show();
        }

        private void btn_natisni_racun_Click(object sender, RoutedEventArgs e)
        {
            if (lb_delovni_nalogi.SelectedIndex != -1)
            {
                NatisniRacun(id_dn);
            }
        }

        private void btn_natisni_dobavnico_Click(object sender, RoutedEventArgs e)
        {
            if (lb_delovni_nalogi.SelectedIndex != -1)
            {
                NatisniDobavnico(id_dn);
            }
        }

        private void NatisniRacun(int id_dn)
        {
            int id = id_dn;

            var connectionString = ConfigurationManager.ConnectionStrings["myDatabaseConnection"].ConnectionString;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.ConnectionString = connectionString;
                    connection.Open();

                    var queryGetId = "select racun_id from delovni_nalogi where id = " + id + ";";
                    MySqlCommand commandgetid = new MySqlCommand(queryGetId, connection);
                    id_racuna = Convert.ToInt32(commandgetid.ExecuteScalar());

                    var queryGetJezik = "select jezik from racuni where id = " + id_racuna + ";";
                    MySqlCommand commandgetjezik = new MySqlCommand(queryGetJezik, connection);
                    jezik = Convert.ToInt32(commandgetjezik.ExecuteScalar());

                    connection.Close();

                    if (jezik == 0)
                        NatisniSloRacun(id_racuna);
                    else if (jezik == 1)
                        NatisniAngRacun(id_racuna);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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

        private void NatisniDobavnico(int id_dn)
        {
            int id = id_dn;

            var connectionString = ConfigurationManager.ConnectionStrings["myDatabaseConnection"].ConnectionString;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.ConnectionString = connectionString;
                    connection.Open();

                    var queryGetId = "select dobavnica_id from delovni_nalogi where id = " + id + ";";
                    MySqlCommand commandgetid = new MySqlCommand(queryGetId, connection);
                    id_dobavnice = Convert.ToInt32(commandgetid.ExecuteScalar());

                    connection.Close();

                    NatisniDobavnico racun = new NatisniDobavnico(id_dobavnice);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dobavnice_okno_Click(object sender, RoutedEventArgs e)
        {
            DobavniceView okno = new DobavniceView();
            okno.Show();
        }

        private void dobavitelji_okno_Click(object sender, RoutedEventArgs e)
        {
            DobaviteljiView okno = new DobaviteljiView(0);
            okno.ShowDialog();
        }

        private void IzracunSkupaj()
        {
            decimal strosek_skupaj = 0;
            decimal prihodek_skupaj = 0;

            var connectionString = ConfigurationManager.ConnectionStrings["myDatabaseConnection"].ConnectionString;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                var query = "select sum(strosek + strosek2 + strosek3 + strosek4) as strosek_skupaj, sum(prihodek) as prihodek_skupaj from delovni_nalogi;";

                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        decimal temp_strosek = 0;
                        decimal temp_prihodek = 0;

                        if (reader["strosek_skupaj"] != DBNull.Value)
                            temp_strosek = (Convert.ToDecimal(reader["strosek_skupaj"]));
                        if (reader["prihodek_skupaj"] != DBNull.Value)
                            temp_prihodek = (Convert.ToDecimal(reader["prihodek_skupaj"]));

                        strosek_skupaj = strosek_skupaj + temp_strosek;
                        prihodek_skupaj = prihodek_skupaj + temp_prihodek;
                    }
                }
                connection.Close();

                tb_skupaj_strosek.Text = strosek_skupaj.ToString("#,###.00 €");
                tb_skupaj_prihodek.Text = prihodek_skupaj.ToString("#,###.00 €");
                tb_skupaj_dobicek.Text = (prihodek_skupaj - strosek_skupaj).ToString("#,###.00 €");

                lb_izracun_po_mescih.DataContext = new IzracunPoMescihViewModel();
            }
        }

        private void Izhod_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Ali želite zapustiti aplikacijo?", "Izhod", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                System.Windows.Application.Current.Shutdown();
            }
        }

        private void narocilnice_okno_Click(object sender, RoutedEventArgs e)
        {
            NarocilniceView okno = new NarocilniceView();
            okno.Show();
        }

        private void nov_račun_Click(object sender, RoutedEventArgs e)
        {
            UstvariRacunView okno = new UstvariRacunView();
            okno.ShowDialog();
        }

        private void nova_dobavnica_Click(object sender, RoutedEventArgs e)
        {
            UstvariDobavnicoView okno = new UstvariDobavnicoView();
            okno.Show();
        }

        private void nova_narocilnica_Click(object sender, RoutedEventArgs e)
        {
            UstvariNarocilnico okno = new UstvariNarocilnico();
            okno.Show();
        }
    }
}
 