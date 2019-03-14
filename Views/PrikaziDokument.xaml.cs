using System;
using System.Collections.Generic;
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

namespace POSSavkovic.Views
{
    /// <summary>
    /// Interaction logic for PrikaziDokument.xaml
    /// </summary>
    public partial class PrikaziDokument : Window
    {
        public PrikaziDokument(string title)
        {
            InitializeComponent();

            Title = title;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DialogResult = true;
        }
    }
}

