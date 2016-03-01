using LASHON;
using System.Windows;
using System.Windows.Controls;

namespace ModernUINavigationApp1
{
    /// <summary>
    /// Interaction logic for BasicPage1.xaml
    /// </summary>
    public partial class BasicPage1 : UserControl
    {
        public BasicPage1()
        {
            InitializeComponent();
        }

        private void onClick(object sender, RoutedEventArgs e)
        {
            button.IsEnabled = false;
            textBlock.Text = GetData.getEnglishToHebrew(textBox.Text);
            button.IsEnabled = true;
        }
    }
}
