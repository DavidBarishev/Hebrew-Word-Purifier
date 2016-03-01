using System.Windows;
using System.Windows.Controls;
using LASHON;
using System.Windows.Data;
using System.Diagnostics;

namespace ModernUINavigationApp1.Pages
{
    /// <summary>
    /// Interaction logic for BasicPage3.xaml
    /// </summary>
    public partial class BasicPage3 : UserControl
    {
        public BasicPage3()
        {
            InitializeComponent();
        }

        private void onClick(object sender, RoutedEventArgs e)
        {
            this.button.IsEnabled = false;

            this.listView.Items.Clear();


            var data = GetData.getByLetter(char.Parse((string)(((ComboBoxItem)this.comboBox.SelectedItem).Content)));

            foreach(var word in data)
            {
                this.listView.Items.Add(new { word = word[0], Hebrew = word[1] });
                Debug.WriteLine("{0} {1}", word[0], word[1]);
            }
                

            this.button.IsEnabled = true;
        }
    }
}
