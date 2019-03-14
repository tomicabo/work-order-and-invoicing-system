using POSSavkovic.ViewModels;
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
    /// Interaction logic for NarocilniceView.xaml
    /// </summary>
    public partial class NarocilniceView : Window
    {
        public NarocilniceView()
        {
            InitializeComponent();
            lb_narocilnice.DataContext = new NarocilniceViewModel();
        }

        private void btn_preklici_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_natisni_narocilnico_Click(object sender, RoutedEventArgs e)
        {
            if (lb_narocilnice.SelectedIndex != -1)
            {

                if (lbl_skupaj_cena.Content == "posamična cena")
                    NatisniNarocilnicoPosamicnaCena();
                else
                    NatisniNarocilnicoSkupaj();
            }
        }

        private void NatisniNarocilnicoSkupaj()
        {
            NatisniNarocilnicoSkupajCena narocilnica = new NatisniNarocilnicoSkupajCena(int.Parse(lbl_id_narocilnice.Content.ToString()), lbl_skupaj_cena.Content.ToString());
        }

        private void NatisniNarocilnicoPosamicnaCena()
        {
            NatisniNarocilnico narocilnica = new NatisniNarocilnico(int.Parse(lbl_id_narocilnice.Content.ToString()));
        }
    }
}
