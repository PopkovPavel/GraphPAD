using GraphPAD.Data.User;
using RestSharp;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace GraphPAD
{
    /// <summary>
    /// Логика взаимодействия для PasswordRestorePage.xaml
    /// </summary>
    public partial class PasswordRestorePage : Window
    {
        public PasswordRestorePage()
        {
            InitializeComponent();
            Closing += OnClosing; //Делегат для отлова закрытия окна
        }

        private void OnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            if (MessageBox.Show(this, Properties.Language.ExitAppMessage, Properties.Language.Confirmation, MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                cancelEventArgs.Cancel = true;
            }
            else
            {
                Process.GetCurrentProcess().Kill();
            }
        }
        private void SendMailButton_Clicked(object sender, RoutedEventArgs e)
        {
            string _Mail = mailTextbox.Text.Trim(); //ToLower() - Перевод всех символов строки в нижний регистр
            MainPage.isGuestConnected = false;
            if (!_Mail.Contains("@") || (!_Mail.Contains(".")) || _Mail.Length <= 6)
            {
                mailTextbox.ToolTip = Properties.Language.IncorrectEmailDataTooltip;
                mailTextbox.BorderBrush = Brushes.Red;
            }
            else
            {
                mailTextbox.ToolTip = _Mail;
                mailTextbox.BorderBrush = Brushes.Gray;
                var client = new RestClient("http://testingwebrtc.herokuapp.com/user/forgotPassword");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddParameter("email", _Mail.ToString());
                IRestResponse response = client.Execute(request);

                MessageBox.Show(Properties.Language.PasswordRestoreMessage, Properties.Language.Caption);
                AuthPage authPage = new AuthPage();
                this.Visibility = Visibility.Hidden;
                authPage.Show();
            }
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.sendMailButton.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
            }
        }
    }

}
