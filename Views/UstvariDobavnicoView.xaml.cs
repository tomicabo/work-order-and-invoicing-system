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
    /// Interaction logic for UstvariDobavnicoView.xaml
    /// </summary>
    public partial class UstvariDobavnicoView : Window
    {
        private string opis;
        private int kol;

        private int dobavnica_id;
        private int id_kupca;
        private int st_dokumenta_blago;
        private string podjetje = "";

        public class LVI
        {
            public string Opis { get; set; }
            public int Kol { get; set; }
        }

        ObservableCollection<LVI> lvi_list = new ObservableCollection<LVI>();

        public UstvariDobavnicoView()
        {
            InitializeComponent();
            //tb_leto.Text = "-" + DateTime.Now.Year.ToString();
            lv_blago.ItemsSource = lvi_list;
        }

        private void btn_preklici_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_dodaj_blago_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                opis = tb_vrsta_blaga.Text;
                kol = int.Parse(tb_kolicina.Text);

                lvi_list.Add(new LVI
                {
                    Opis = opis,
                    Kol = kol,
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btn_odstrani_blago_Click(object sender, RoutedEventArgs e)
        {
            lvi_list.Remove((LVI)lv_blago.SelectedItem);
        }

        private void btn_dodaj_kupca_Click(object sender, RoutedEventArgs e)
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

        private void btn_ustvari_dobavnico_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Ali želite ustvariti dobavnico?", "Ustvari Dobavnico", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                InsertBlago();
                InsertDobavnica();

                NatisniDobavnico racun = new NatisniDobavnico(dobavnica_id);

                this.Close();
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

                    var queryInsertBlago = "insert into blago (id, opis, kolicina, st_dokumenta)" +
                        "values (@Id, @Opis, @Kolicina, @StDokumenta);";

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
    }
}
