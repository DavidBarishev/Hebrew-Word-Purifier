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
using System.Windows.Navigation;
using System.Windows.Shapes;
using LASHON;

namespace ModernUINavigationApp1
{
    /// <summary>
    /// Interaction logic for BasicPage2.xaml
    /// </summary>
    public partial class BasicPage2 : UserControl
    {
        public BasicPage2()
        {
            InitializeComponent();
        }

        private void onClick(object sender, RoutedEventArgs e)
        {
            this.button.IsEnabled = false;

            var returnRnd = GetData.random();
            this.textBlock.Text = returnRnd[0];
            this.textBlock1.Text = returnRnd[1];

            this.button.IsEnabled = true;
        }
    }
}
