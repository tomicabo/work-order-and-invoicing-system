﻿using System;
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
    /// Interaction logic for IzdajDobavnico.xaml
    /// </summary>
    public partial class IzdajDobavnico : Window
    {
        public IzdajDobavnico()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //MessageBoxResult result = MessageBox.Show("Ali želite izdati dobavnico?", "Izdaj Dobavnico", MessageBoxButton.YesNo);
            //if (result == MessageBoxResult.Yes)
            //{
            //    this.DialogResult = true;
            //}
        }
    }
}
