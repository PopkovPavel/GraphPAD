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
            AuthPage authPage = new AuthPage();
            MainPage mainPage = new MainPage();
            this.Close();
            MainPage.CloseForm();
            //mainPage.Hide();
            //mainPage.Visibility = Visibility.Hidden;
            //this.Visibility = Visibility.Hidden; //Скрывает текущее окно
            authPage.Show();
        }
    }

}
