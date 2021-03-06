using System.IO;
using System.Windows;

namespace GraphPAD
{
    public partial class SettingsPage : Window
    {
        public SettingsPage()
        {
            InitializeComponent();
        }
        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            File.Delete(@"Token.json");
            this.Close();
            MainPage.CloseForm();
        }
    }

}
