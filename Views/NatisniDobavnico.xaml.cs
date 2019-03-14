using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
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
using System.Windows.Xps;
using System.Windows.Xps.Packaging;

namespace POSSavkovic.Views
{
    /// <summary>
    /// Interaction logic for NatisniDobavnico.xaml
    /// </summary>
    public partial class NatisniDobavnico : Window
    {
        private int id_dobavnice;
        string document_name;

        public class LVI
        {
            public string Opis { get; set; }
            public int Kol { get; set; }
        }

        ObservableCollection<LVI> lvi_list_dobavnice = new ObservableCollection<LVI>();

        public NatisniDobavnico(int dobavnica_id)
        {
            InitializeComponent();
            id_dobavnice = dobavnica_id;

            NapolniPodatke();
            NapolniListView();
            lv_blago_dobavnice.ItemsSource = lvi_list_dobavnice;

            //PDF print
            //PrintDialog printDlg = new PrintDialog();
            //printDlg.PrintVisual(PrintGrid, document_name);

            Print();
        }

        private void Print()
        {
            //XPS print
            string filename = Directory.GetCurrentDirectory() + document_name + ".xps";
            try
            {
                XpsDocument doc = new XpsDocument(filename, FileAccess.ReadWrite);
                XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(doc);
                writer.Write(PrintGrid);
                doc.Close();

                //odpri dokument
                DocumentViewer viewer = new DocumentViewer();
                XpsDocument xdoc = new XpsDocument(filename, FileAccess.Read);
                viewer.Document = xdoc.GetFixedDocumentSequence();
                //prikaži
                PrikaziDokument pd = new PrikaziDokument(filename);
                pd.Content = viewer;
                pd.WindowState = WindowState.Maximized;
                pd.ShowDialog();

                //Delete file
                if (pd.DialogResult == true)
                {
                    var myXpsUri = xdoc.Uri;
                    var theXpsPackage = System.IO.Packaging.PackageStore.GetPackage(myXpsUri);
                    theXpsPackage.Close();
                    System.IO.Packaging.PackageStore.RemovePackage(myXpsUri);
                    File.Delete(filename);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Podrobnosti:" + System.Environment.NewLine + System.Environment.NewLine + ex.Message, "Težave pri ustvarjanju dokumenta", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NapolniListView()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["myDatabaseConnection"].ConnectionString;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                var query = "select b.opis, b.kolicina from blago b " +
                            "join dobavnice dn on dn.blago_id = b.st_dokumenta " +
                            "where dn.id = " + id_dobavnice + ";";

                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        LVI list_items = new LVI();

                        if (reader["opis"] != DBNull.Value)
                            list_items.Opis = (Convert.ToString(reader["opis"]));
                        if (reader["kolicina"] != DBNull.Value)
                            list_items.Kol = (Convert.ToInt32(reader["kolicina"]));

                        lvi_list_dobavnice.Add(list_items);
                    }
                    connection.Close();
                }
            }
        }

        private void NapolniPodatke()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["myDatabaseConnection"].ConnectionString;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                var query = "select k.podjetje, k.naslov, k.posta, k.davcna_st, d.st_dobavnice, d.ustvarjeno, d.st_narocila " +
                            "from dobavnice d " +
                            "join kupci k on d.kupec = k.id where d.id =  " + id_dobavnice + ";";

                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["podjetje"] != DBNull.Value)
                            tb_kupec1.Text = (Convert.ToString(reader["podjetje"]));
                        if (reader["naslov"] != DBNull.Value)
                            tb_kupec2.Text = (Convert.ToString(reader["naslov"]));
                        if (reader["posta"] != DBNull.Value)
                            tb_kupec3.Text = (Convert.ToString(reader["posta"]));
                        if (reader["davcna_st"] != DBNull.Value)
                            tb_kupec4.Text = "ID številka kupca za DDV: " + (Convert.ToString(reader["davcna_st"]));
                        if (reader["st_dobavnice"] != DBNull.Value)
                        {
                            DateTime temp = Convert.ToDateTime(reader["ustvarjeno"]);
                            tb_dobavnica_st.Text = "00" + Convert.ToString(reader["st_dobavnice"]) + "-" + temp.ToString("yyyy");
                            document_name = "dobavnica_" + Convert.ToString(reader["st_dobavnice"]) + "-" + temp.ToString("yyyy");
                        }
                        if (reader["ustvarjeno"] != DBNull.Value)
                        {
                            DateTime temp = Convert.ToDateTime(reader["ustvarjeno"]);
                            tb_datum.Text = temp.ToString("dd.MM.yyyy");
                        }
                        if (reader["st_narocila"] != DBNull.Value)
                            tb_st_narocila.Text = (Convert.ToString(reader["st_narocila"]));


                    }
                    connection.Close();
                }
            }
        }
    }
}
