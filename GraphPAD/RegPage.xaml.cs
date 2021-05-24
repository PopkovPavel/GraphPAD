using GraphPAD.Data.JSON;
using GraphPAD.Data.User;
using Newtonsoft.Json;
using RestSharp;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace GraphPAD
{
    public partial class RegPage : Window
    {
        public RegPage()
        {
            InitializeComponent();
            Closing += OnClosing; //Делегат для отлова закрытия окна
            this.KeyDown += new KeyEventHandler(RegWindow_KeyDown);
        }
        private void OpenAuthPage(object sender, RoutedEventArgs e)
        {
            AuthPage authPage = new AuthPage();
            this.Visibility = Visibility.Hidden; //Скрывает текущее окно
            authPage.Show();
        }
        private void RegButton_Clicked(object sender, RoutedEventArgs e)
        {
            string _name = textboxName.Text.Trim(); //Trim() - Удаление лишних символов
            string _Email = textboxEmail.Text.Trim().ToLower(); //ToLower() - Перевод всех символов строки в нижний регистр
            string _password1 = passwordbox_1.Password.Trim();
            string _password2 = passwordbox_2.Password.Trim();
            bool nameCorrect = false;
            bool emailCorrect = false;
            bool ispassEqual = false;
            //Логика регистрации
            if (_name.Length < 3)
            {
                textboxName.ToolTip = Properties.Language.TooShortNameTooltip;
                textboxName.BorderBrush = Brushes.Red;
            }
            else //Имя верно
            {
                textboxName.ToolTip = null;
                textboxName.BorderBrush = Brushes.Gray;
                nameCorrect = true;
            }
            if (_Email.Length < 5)
            {
                textboxEmail.ToolTip = Properties.Language.TooShortLoginTooltip;
                textboxEmail.BorderBrush = Brushes.Red;
            }
            else if (!_Email.Contains("@") || (!_Email.Contains(".")))
            {
                textboxEmail.ToolTip = Properties.Language.IncorrectEmailDataTooltip;
                textboxEmail.BorderBrush = Brushes.Red;
            }
            else //Почта верна
            {
                textboxEmail.ToolTip = null;
                textboxEmail.BorderBrush = Brushes.Gray;
                emailCorrect = true;
            }
            if (_password1 != _password2 || (_password1 == "" && _password2 == ""))
            {
                passwordbox_1.ToolTip = Properties.Language.PasswordsDontMatchTooltip;
                passwordbox_1.BorderBrush = Brushes.Red;
                passwordbox_2.ToolTip = Properties.Language.PasswordsDontMatchTooltip;
                passwordbox_2.BorderBrush = Brushes.Red;
            }
            else if (_password1.Length < 8 || _password2.Length < 8)
            {
                passwordbox_1.ToolTip = Properties.Language.TooShortPasswordTooltip;
                passwordbox_1.BorderBrush = Brushes.Red;
                passwordbox_2.ToolTip = Properties.Language.TooShortPasswordTooltip;
                passwordbox_2.BorderBrush = Brushes.Red;
            }
            else //Пароли совпадают
            {
                passwordbox_1.ToolTip = null;
                passwordbox_1.BorderBrush = Brushes.Gray;
                passwordbox_2.ToolTip = null;
                passwordbox_2.BorderBrush = Brushes.Gray;
                ispassEqual = true;
            }
            if (nameCorrect == true && emailCorrect == true && ispassEqual == true)
            {
                if (MessageBox.Show(this, Properties.Language.CreateAccWithThisDataMessage + textboxName.Text + "\nEmail - " + textboxEmail.Text, Properties.Language.Confirmation, MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                {
                    //pass
                }
                else
                {
                    try
                    {
                        var client = new RestClient("http://testingwebrtc.herokuapp.com/signup");
                        client.Timeout = -1;
                        var request = new RestRequest(Method.POST);
                        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                        request.AddParameter("nickname", _name.ToString());
                        request.AddParameter("email", _Email.ToString());
                        request.AddParameter("password", _password1.ToString());
                        IRestResponse response = client.Execute(request);

                        string responseData = response.Content.ToString();

                        JSONauth tempUser = JsonConvert.DeserializeObject<JSONauth>(responseData);

                        UserInfo.Email = tempUser.Data.Email;
                        //UserInfo.ID = tempUser.ID;
                        UserInfo.Role = tempUser.Data.Role;
                        UserInfo.Token = tempUser.Token;
                        MessageBox.Show(Properties.Language.SuccessReg, Properties.Language.Caption, MessageBoxButton.OK, MessageBoxImage.Information);
                        AuthPage authPage = new AuthPage();
                        this.Visibility = Visibility.Hidden; //Скрывает текущее окно
                        authPage.Show();
                    }
                    catch
                    {
                        textboxEmail.ToolTip = Properties.Language.ExistingEmail;
                        textboxEmail.BorderBrush = Brushes.Red;
                    }
                }
            }
        }
        private void OnClosing(object sender, CancelEventArgs cancelEventArgs) //Подтверждения выхода из программы
        {
            if (MessageBox.Show(this, Properties.Language.ExitAppMessage, Properties.Language.Confirmation, MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                cancelEventArgs.Cancel = true;
            }
            else
            {
                Process.GetCurrentProcess().Kill();  //Полное выключение программы
            }

        }

        private void RegWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.regButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }
    }
}
