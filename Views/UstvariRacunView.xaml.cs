using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for UstvariRacunView.xaml
    /// </summary>
    public partial class UstvariRacunView : Window
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

        private int racun_id;
        private int dobavnica_id;
        private int id_kupca;
        private int st_dokumenta_blago;
        private string podjetje = "";

        private int jezik;

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

        public UstvariRacunView()
        {
            InitializeComponent();
            lv_blago.ItemsSource = lvi_list;
            dp_izdano.SelectedDate = DateTime.Now;
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

        private void btn_ustvari_racun_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Ali želite ustvariti račun?", "Ustvari Račun", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                if (rb_slo.IsChecked == true)
                    jezik = 0;
                else if (rb_ang.IsChecked == true)
                    jezik = 1;

                InsertBlago();
                InsertRacun();

                if (rb_slo.IsChecked == true)
                    NatisniSloRacun();
                else if (rb_ang.IsChecked == true)
                    NatisniAngRacun();

                if (cb_ustvari_dobavnico.IsChecked == true)
                {
                    InsertDobavnica();

                    NatisniDobavnico dobavnica = new NatisniDobavnico(dobavnica_id);

                }
                this.Close();
            }
        }

        private void NatisniAngRacun()
        {
            NatisniRacunAng racun = new NatisniRacunAng(racun_id);
        }

        private void NatisniSloRacun()
        {
            NatisniRacun racun = new NatisniRacun(racun_id);
        }

        private void btn_preklici_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e) //IZBERI KUPCA
        {
            var okno = new KupciView(1);
            if (okno.ShowDialog() == true)
            {
                id_kupca = okno.id;
                lbl_kupec_id.Content = id_kupca;
                podjetje = okno.podjetje;
            }
            else MessageBox.Show("Napaka");

            tb_podjetje.Text = podjetje;
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

            //izracun_skupaj_brez_ddv = izracun_skupaj_brez_ddv + vrednost;
            //izracun_ddv = izracun_ddv + (cena_z_ddv - vrednost);
            //izracun_vrednost = izracun_skupaj_brez_ddv + izracun_ddv;

            tb_skupaj_brez_ddv.Text = izracun_skupaj_brez_ddv.ToString();
            tb_skupaj_ddv.Text = izracun_ddv.ToString();
            tb_vrednost.Text = izracun_vrednost.ToString();
        }

        private void InsertBlago()
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
                    st_dokumenta_blago = Convert.ToInt32(commandgetstdokumenta.ExecuteScalar());

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

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void InsertDobavnica()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["myDatabaseConnection"].ConnectionString;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.ConnectionString = connectionString;
                    var queryGetId = "select max(id)+1 from dobavnice;";

                    var queryGetStDobavnice = "select max(st_dobavnice) from dobavnice where YEAR(ustvarjeno) = YEAR(CURDATE());";

                    var queryInsertBlago = "insert into dobavnice (id, ustvarjeno, st_dobavnice, kupec, st_narocila, blago_id)" +
                        "values (@Id, @Ustvarjeno, @StDobavnice, @Kupec, @StNarocila, @BlagoId);";

                    connection.Open();

                    MySqlCommand commandgetid = new MySqlCommand(queryGetId, connection);
                    dobavnica_id = Convert.ToInt32(commandgetid.ExecuteScalar());

                    MySqlCommand commandgetstd = new MySqlCommand(queryGetStDobavnice, connection);
                    int st_dobavnice;
                    if (commandgetstd.ExecuteScalar() != DBNull.Value)
                        st_dobavnice = Convert.ToInt32(commandgetstd.ExecuteScalar()) + 1;
                    else st_dobavnice = 1;

                    using (MySqlCommand cmd = new MySqlCommand(queryInsertBlago, connection))
                    {
                        cmd.Parameters.AddWithValue("@Id", dobavnica_id);
                        cmd.Parameters.AddWithValue("@Ustvarjeno", DateTime.Now);
                        cmd.Parameters.AddWithValue("@StDobavnice", st_dobavnice);
                        cmd.Parameters.AddWithValue("@Kupec", id_kupca);
                        cmd.Parameters.AddWithValue("@StNarocila", tb_st_narocila.Text);
                        cmd.Parameters.AddWithValue("@BlagoId", st_dokumenta_blago);

                        cmd.ExecuteNonQuery();
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void InsertRacun()
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
                    racun_id = Convert.ToInt32(commandgetid.ExecuteScalar());

                    var queryGetSumSkupajZDDV = "select SUM(cena_z_ddv) from blago where st_dokumenta = " + st_dokumenta_blago + ";";
                    MySqlCommand commandgetskupajzddv = new MySqlCommand(queryGetSumSkupajZDDV, connection);
                    decimal skupaj_z_ddv = Convert.ToDecimal(commandgetskupajzddv.ExecuteScalar());

                    var queryGetSumSkupajBrezDDV = "select SUM(cena_brez_ddv) from blago where st_dokumenta = " + st_dokumenta_blago + ";";
                    MySqlCommand commandgetskupajbrezddv = new MySqlCommand(queryGetSumSkupajBrezDDV, connection);
                    decimal skupaj_brez_ddv = Convert.ToDecimal(commandgetskupajbrezddv.ExecuteScalar());

                    var queryGetStRacuna = "select max(st_racuna) from racuni where YEAR(ustvarjeno) = YEAR(CURDATE());";
                    MySqlCommand commandgetstr = new MySqlCommand(queryGetStRacuna, connection);
                    int st_racuna;
                    if (commandgetstr.ExecuteScalar() != DBNull.Value)
                        st_racuna = Convert.ToInt32(commandgetstr.ExecuteScalar()) + 1;
                    else st_racuna = 1;


                    var queryInsertRacuni = "insert into racuni (id, ustvarjeno, st_racuna, skupaj_z_ddv, skupaj_brez_ddv, kupec, datum_zap, blago_id, st_narocila, jezik)" +
                        "values (@Id, @Ustvarjeno, @StRacuna, @SkupajZDDV, @SkupajBrezDDV, @Kupec, @DatumZapadlosti, @BlagoId, @StNarocila, @Jezik);";

                    using (MySqlCommand cmd = new MySqlCommand(queryInsertRacuni, connection))
                    {
                        cmd.Parameters.AddWithValue("@Id", racun_id);
                        cmd.Parameters.AddWithValue("@Ustvarjeno", dp_izdano.SelectedDate);
                        cmd.Parameters.AddWithValue("@StRacuna", st_racuna);
                        cmd.Parameters.AddWithValue("@SkupajZDDV", skupaj_z_ddv);
                        cmd.Parameters.AddWithValue("@SkupajBrezDDV", skupaj_brez_ddv);
                        cmd.Parameters.AddWithValue("@Kupec", lbl_kupec_id.Content);
                        cmd.Parameters.AddWithValue("@DatumZapadlosti", dp_zapadlost.SelectedDate);
                        cmd.Parameters.AddWithValue("@BlagoId", st_dokumenta_blago);
                        cmd.Parameters.AddWithValue("@StNarocila", tb_st_narocila.Text);
                        cmd.Parameters.AddWithValue("@Jezik", jezik);

                        cmd.ExecuteNonQuery();
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
