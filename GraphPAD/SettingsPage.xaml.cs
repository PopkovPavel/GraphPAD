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
        private void AccountButton_Click(object sender, RoutedEventArgs e)
        {
            AccountCanvas.Visibility = Visibility.Visible;
            VoiceCanvas.Visibility = Visibility.Hidden;
            VideoCanvas.Visibility = Visibility.Hidden;
            InterfaceCanvas.Visibility = Visibility.Hidden;
        }

        private void VoiceButton_Click(object sender, RoutedEventArgs e)
        {
            AccountCanvas.Visibility = Visibility.Hidden;
            VoiceCanvas.Visibility = Visibility.Visible;
            VideoCanvas.Visibility = Visibility.Hidden;
            InterfaceCanvas.Visibility = Visibility.Hidden;
        }

        private void VideoButton_Click(object sender, RoutedEventArgs e)
        {
            AccountCanvas.Visibility = Visibility.Hidden;
            VoiceCanvas.Visibility = Visibility.Hidden;
            VideoCanvas.Visibility = Visibility.Visible;
            InterfaceCanvas.Visibility = Visibility.Hidden;
        }

        private void InterfaceButton_Click(object sender, RoutedEventArgs e)
        {
            AccountCanvas.Visibility = Visibility.Hidden;
            VoiceCanvas.Visibility = Visibility.Hidden;
            VideoCanvas.Visibility = Visibility.Hidden;
            InterfaceCanvas.Visibility = Visibility.Visible;
        }
        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            File.Delete(@"Token.json");
            this.Close();
            MainPage.CloseForm();
        }

        private void vkButton_Clicked(object sender, RoutedEventArgs e)
        {

        }

        private void ytButton_Clicked(object sender, RoutedEventArgs e)
        {

        }

        private void webButton_Clicked(object sender, RoutedEventArgs e)
        {

        }
    }
}
