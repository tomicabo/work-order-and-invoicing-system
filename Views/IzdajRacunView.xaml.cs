using System;
using System.Collections.Generic;
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
    /// Interaction logic for IzdajRacunView.xaml
    /// </summary>
    public partial class IzdajRacunView : Window
    {
        public DateTime? DatumZapadlosti { get; set; }
        public DateTime? DatumIzdaje { get; set; }
        public int Jezik { get; set; }

        public IzdajRacunView()
        {
            InitializeComponent();
            dp_izdano.SelectedDate = DateTime.Now;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DatumZapadlosti = dp_zapadlost.SelectedDate;
            DatumIzdaje = dp_izdano.SelectedDate;
            if (rb_slo.IsChecked == true)
                Jezik = 0;
            else if (rb_ang.IsChecked == true)
                Jezik = 1;

            MessageBoxResult result = MessageBox.Show("Ali želite izdati račun?", "Izdaj Račun", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                this.DialogResult = true;
            }
        }
    }
}
