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
    /// Interaction logic for NatisniNarocilnicoSkupajCena.xaml
    /// </summary>
    public partial class NatisniNarocilnicoSkupajCena : Window
    {
        private int id_narocilnice;
        string document_name;

        public class LVI
        {
            public string Opis { get; set; }
            public int Kol { get; set; }
        }

        ObservableCollection<LVI> lvi_list_narocilnice = new ObservableCollection<LVI>();

        public NatisniNarocilnicoSkupajCena(int narocilnica_id, string skupaj_cena)
        {
            InitializeComponent();
            id_narocilnice = narocilnica_id;
            tb_skupaj.Text = skupaj_cena;

            NapolniPodatke();
            NapolniListView();
            lv_blago_narocilnice.ItemsSource = lvi_list_narocilnice;

            //PDF Print
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
                            "join narocilnice n on n.blago_id = b.st_dokumenta " +
                            "where n.id = " + id_narocilnice + ";";

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

                        lvi_list_narocilnice.Add(list_items);
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
                var query = "select d.podjetje, d.naslov, d.posta, d.davcna_st, n.st_narocilnice, n.ustvarjeno, n.datum_dobave, n.dni_za_placilo " +
                            "from narocilnice n " +
                            "join dobavitelji d on n.dobavitelj = d.id where n.id = " + id_narocilnice + ";";

                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["podjetje"] != DBNull.Value)
                            tb_dobavitelj1.Text = (Convert.ToString(reader["podjetje"]));
                        if (reader["naslov"] != DBNull.Value)
                            tb_dobavitelj2.Text = (Convert.ToString(reader["naslov"]));
                        if (reader["posta"] != DBNull.Value)
                            tb_dobavitelj3.Text = (Convert.ToString(reader["posta"]));
                        if (reader["davcna_st"] != DBNull.Value)
                            tb_dobavitelj4.Text = "ID številka kupca za DDV: " + (Convert.ToString(reader["davcna_st"]));
                        if (reader["st_narocilnice"] != DBNull.Value)
                        {
                            DateTime temp = Convert.ToDateTime(reader["ustvarjeno"]);
                            tb_narocilnica_st.Text = "00" + Convert.ToString(reader["st_narocilnice"]) + "-" + temp.ToString("yyyy");
                            document_name = "narocilnica_" + Convert.ToString(reader["st_narocilnice"]) + "-" + temp.ToString("yyyy");
                        }
                        if (reader["ustvarjeno"] != DBNull.Value)
                        {
                            DateTime temp = Convert.ToDateTime(reader["ustvarjeno"]);
                            tb_datum.Text = temp.ToString("dd.MM.yyyy");
                        }
                        if (reader["datum_dobave"] != DBNull.Value)
                        {
                            DateTime temp = Convert.ToDateTime(reader["datum_dobave"]);
                            tb_datum_dobave.Text = temp.ToString("dd.MM.yyyy");
                        }
                        if (reader["dni_za_placilo"] != DBNull.Value)
                            tb_dni_za_placilo.Text = (Convert.ToString(reader["dni_za_placilo"]));
                    }
                    connection.Close();
                }
            }
        }
    }
}
