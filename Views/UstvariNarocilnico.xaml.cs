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
    /// Interaction logic for UstvariNarocilnico.xaml
    /// </summary>
    public partial class UstvariNarocilnico : Window
    {
        private string opis;
        private int kol;
        private decimal cena_brez_ddv;

        private int narocilnica_id;
        private int id_dobavitelja;
        private int st_dokumenta_blago;
        private string podjetje = "";

        private decimal izracun_skupaj_brez_ddv = 0;

        public class LVI
        {
            public string Opis { get; set; }
            public int Kol { get; set; }
            public decimal Cena_brez_ddv { get; set; }
        }

        ObservableCollection<LVI> lvi_list = new ObservableCollection<LVI>();

        public UstvariNarocilnico()
        {
            InitializeComponent();
            //tb_leto.Text = "-" + DateTime.Now.Year.ToString();
            lv_blago.ItemsSource = lvi_list;
        }

        private void btn_preklici_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_dodaj_dobavitelja_Click(object sender, RoutedEventArgs e)
        {
            var okno = new DobaviteljiView(1);
            if (okno.ShowDialog() == true)
            {
                id_dobavitelja = okno.id;
                lbl_dobavitelj_id.Content = id_dobavitelja;
                podjetje = okno.podjetje;
            }
            else MessageBox.Show("Napaka");

            tb_podjetje.Text = podjetje;
        }

        private void btn_dodaj_blago_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                opis = tb_vrsta_blaga.Text;
                kol = int.Parse(tb_kolicina.Text);
                cena_brez_ddv = Math.Round(decimal.Parse(tb_cena.Text), 2);

                lvi_list.Add(new LVI
                {
                    Opis = opis,
                    Kol = kol,
                    Cena_brez_ddv = cena_brez_ddv
                });

                OsveziIzracun();
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
            decimal temp;
            izracun_skupaj_brez_ddv = 0;

            for (int i = 0; i < lvi_list.Count; i++)
            {
                temp = 0;
                temp = Math.Round((lvi_list[i].Cena_brez_ddv * lvi_list[i].Kol), 2);
                izracun_skupaj_brez_ddv = Math.Round((izracun_skupaj_brez_ddv + temp), 2);
            }

            tb_skupaj_brez_ddv.Text = izracun_skupaj_brez_ddv.ToString();
        }

        private void btn_ustvari_narocilnico_Click(object sender, RoutedEventArgs e)
        {
            if (cb_skupaj_textbox.IsChecked != true)
            {
                MessageBoxResult result = MessageBox.Show("Ali želite ustvariti naročilnico?", "Ustvari Naročilnico", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    InsertBlago();
                    InsertNarocilnica(0);

                    NatisniNarocilnico racun = new NatisniNarocilnico(narocilnica_id);

                    this.Close();
                }
            }
            else
            {
                decimal skupaj_cena = decimal.Parse(tb_skupaj_textbox.Text);

                MessageBoxResult result = MessageBox.Show("Ali želite ustvariti naročilnico?", "Ustvari Naročilnico", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    InsertBlago();
                    InsertNarocilnica(skupaj_cena);

                    NatisniNarocilnicoSkupajCena racun = new NatisniNarocilnicoSkupajCena(narocilnica_id, skupaj_cena.ToString("#,###.00 €"));

                    this.Close();
                }
            }
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

                    var queryInsertBlago = "insert into blago (id, opis, kolicina, cena_brez_ddv, st_dokumenta) " +
                        "values (@Id, @Opis, @Kolicina, @CenaBrezDDV, @StDokumenta);";

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
                            cmd.Parameters.AddWithValue("@Kolicina", item.Kol);
                            cmd.Parameters.AddWithValue("@CenaBrezDDV", item.Cena_brez_ddv);
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

        private void InsertNarocilnica(decimal skupaj_cena)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["myDatabaseConnection"].ConnectionString;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.ConnectionString = connectionString;
                    var queryGetId = "select max(id)+1 from narocilnice;";

                    var queryGetStNarocilnice = "select max(st_narocilnice) from narocilnice where YEAR(ustvarjeno) = YEAR(CURDATE());";

                    var queryInsertBlago = "insert into narocilnice (id, ustvarjeno, st_narocilnice, dobavitelj, blago_id, dni_za_placilo, datum_dobave, skupaj_cena)" +
                        "values (@Id, @Ustvarjeno, @StNarocilnice, @Dobavitelj, @BlagoId, @DniZaPlacilo, @DatumDobave, @SkupajCena);";

                    connection.Open();

                    MySqlCommand commandgetid = new MySqlCommand(queryGetId, connection);
                    narocilnica_id = Convert.ToInt32(commandgetid.ExecuteScalar());

                    MySqlCommand commandgetstn = new MySqlCommand(queryGetStNarocilnice, connection);
                    int st_narocilnice;
                    if (commandgetstn.ExecuteScalar() != DBNull.Value)
                        st_narocilnice = Convert.ToInt32(commandgetstn.ExecuteScalar()) + 1;
                    else st_narocilnice = 1;

                    using (MySqlCommand cmd = new MySqlCommand(queryInsertBlago, connection))
                    {
                        cmd.Parameters.AddWithValue("@Id", narocilnica_id);
                        cmd.Parameters.AddWithValue("@Ustvarjeno", DateTime.Now);
                        cmd.Parameters.AddWithValue("@StNarocilnice", st_narocilnice);
                        cmd.Parameters.AddWithValue("@Dobavitelj", id_dobavitelja);
                        cmd.Parameters.AddWithValue("@BlagoId", st_dokumenta_blago);
                        cmd.Parameters.AddWithValue("@DniZaPlacilo", tb_dni_za_placilo.Text);
                        cmd.Parameters.AddWithValue("@DatumDobave", dp_datum_dobave.SelectedDate);
                        if (skupaj_cena != 0)
                            cmd.Parameters.AddWithValue("@SkupajCena", skupaj_cena);
                        else
                            cmd.Parameters.AddWithValue("@SkupajCena", DBNull.Value);

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

        private void cb_skupaj_textbox_Unchecked(object sender, RoutedEventArgs e)
        {
            tb_skupaj_textbox.IsEnabled = false;
        }

        private void cb_skupaj_textbox_Checked(object sender, RoutedEventArgs e)
        {
            tb_skupaj_textbox.IsEnabled = true;
        }
    }
}
