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
    /// Interaction logic for DobavniceView.xaml
    /// </summary>
    public partial class DobavniceView : Window
    {
        public DobavniceView()
        {
            InitializeComponent();
            lb_dobavnice.DataContext = new DobavniceViewModel();
        }

        private void btn_preklici_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_natisni_dobavnico_Click(object sender, RoutedEventArgs e)
        {
            if (lb_dobavnice.SelectedIndex != -1)
            {
                NatisniDobavnico dobavnica = new NatisniDobavnico(int.Parse(lbl_id_dobavnice.Content.ToString()));
            }
        }
    }
}
