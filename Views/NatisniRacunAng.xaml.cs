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
    /// Interaction logic for NatisniRacunAng.xaml
    /// </summary>
    public partial class NatisniRacunAng : Window
    {
            private int id_racuna;
            string document_name;

            public class LVI
            {
                public string Opis { get; set; }
                public int Kol { get; set; }
                public string Cena_brez_ddv { get; set; }
                public string Vrednost { get; set; }
                public decimal _Cena_brez_ddv { get; set; }
                public decimal _Vrednost { get; set; }
            }

            ObservableCollection<LVI> lvi_list = new ObservableCollection<LVI>();

            public NatisniRacunAng(int racun_id)
            {
                InitializeComponent();
                id_racuna = racun_id;

                NapolniPodatke();
                NapolniListView();
                Izracun();
                lv_blago.ItemsSource = lvi_list;

                //PDF print
                //try
                //{
                //    PrintDialog printDlg = new PrintDialog();
                //    printDlg.PrintVisual(PrintGrid, document_name);
                //}
                //catch (Exception ex)
                //{
                //    MessageBox.Show(ex.Message);
                //}

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
                    var query = "select b.opis, b.cena_brez_ddv, b.kolicina, b.vrednost from blago b " +
                                "join racuni dn on dn.blago_id = b.st_dokumenta " +
                                "where dn.id = " + id_racuna + ";";

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
                            {
                                list_items.Cena_brez_ddv = (Convert.ToDecimal(reader["cena_brez_ddv"])).ToString("#,###.00");
                                list_items._Cena_brez_ddv = (Convert.ToDecimal(reader["cena_brez_ddv"]));
                            }
                            if (reader["kolicina"] != DBNull.Value)
                                list_items.Kol = (Convert.ToInt32(reader["kolicina"]));
                            if (reader["vrednost"] != DBNull.Value)
                            {
                                list_items.Vrednost = (Convert.ToDecimal(reader["vrednost"])).ToString("#,###.00");
                                list_items._Vrednost = (Convert.ToDecimal(reader["vrednost"]));
                            }

                            lvi_list.Add(list_items);
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
                    var query = "select k.podjetje, k.naslov, k.posta, k.davcna_st, r.st_racuna, r.ustvarjeno, r.datum_zap, r.st_narocila " +
                                "from racuni r " +
                                "join kupci k on r.kupec = k.id where r.id =  " + id_racuna + ";";

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
                                tb_kupec4.Text = "VAT number: " + (Convert.ToString(reader["davcna_st"]));
                            if (reader["st_racuna"] != DBNull.Value)
                            {
                                DateTime temp = Convert.ToDateTime(reader["ustvarjeno"]);
                                tb_racun_st.Text = "00" + Convert.ToString(reader["st_racuna"]) + "-" + temp.ToString("yyyy");
                                tb_sklic.Text = tb_racun_st.Text;
                                document_name = "racun_" + Convert.ToString(reader["st_racuna"]) + "-" + temp.ToString("yyyy");
                            }
                            if (reader["ustvarjeno"] != DBNull.Value)
                            {
                                DateTime temp = Convert.ToDateTime(reader["ustvarjeno"]);
                                tb_datum.Text = temp.ToString("dd.MM.yyyy");
                                tb_datum_ops.Text = temp.ToString("dd.MM.yyyy");
                            }
                            if (reader["datum_zap"] != DBNull.Value)
                            {
                                DateTime temp = Convert.ToDateTime(reader["datum_zap"]);
                                tb_datup_zap.Text = temp.ToString("dd.MM.yyyy");
                            }
                            if (reader["st_narocila"] != DBNull.Value)
                                tb_st_narocila.Text = (Convert.ToString(reader["st_narocila"]));


                        }
                        connection.Close();
                    }
                }
            }

            private void Izracun()
            {
                decimal izracun_skupaj_brez_ddv = 0;

                for (int i = 0; i < lvi_list.Count; i++)
                {
                    izracun_skupaj_brez_ddv = izracun_skupaj_brez_ddv + lvi_list[i]._Vrednost;
                }
                tb_skupaj.Text = izracun_skupaj_brez_ddv.ToString("#,###.00 €");
            }

        }
    
}
