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
    /// Interaction logic for UstvariDokumentView.xaml
    /// </summary>
    public partial class UstvariDokumentView : Window
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

        public UstvariDokumentView(int id_dn)
        {
            InitializeComponent();
            id_delovnega_naloga = id_dn;
            lv_blago.ItemsSource = lvi_list;
        }

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

        private void btn_ustvari_dokument_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Ali želite ustvariti dokument", "Ustvari Dokument", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                InsertBlagoUpdateDN();
                DialogResult = true;
                this.Close();
            }
        }

        private void btn_preklici_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private void InsertBlagoUpdateDN()
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

                    var queryUpdateDelovniNalog = "update delovni_nalogi set status = 1, blago_id = " + st_dokumenta_blago + ", st_narocila = '" + tb_st_narocila.Text + "', prihodek = " + Convert.ToString(izracun_skupaj_brez_ddv, new NumberFormatInfo() { NumberDecimalSeparator = "." }) + " where id = " + id_delovnega_naloga + ";";

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
    }
}
