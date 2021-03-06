using GraphPAD.Data.JSON;
using GraphPAD.Data.User;
using Newtonsoft.Json;
using RestSharp;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Media;

namespace GraphPAD
{
    public partial class AuthPage : Window
    {
        public AuthPage()
        {
            InitializeComponent();
            Closing += OnClosing; //Делегат для отлова закрытия окна
            
            if (File.Exists("Token.json"))
            {
                //string responseData = response.Content.ToString();
                var jsonString = File.ReadAllText("Token.json");
                JSONauth tempUser = JsonConvert.DeserializeObject<JSONauth>(jsonString);

                var client = new RestClient($"http://testingwebrtc.herokuapp.com/user/{tempUser.Data.userId}");
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddHeader("x-access-token", $"{tempUser.Token}");
                IRestResponse response = client.Execute(request);
                HttpStatusCode statusCode = response.StatusCode; 
                if (response.IsSuccessful)
                {
                    JSONauth user = JsonConvert.DeserializeObject<JSONauth>(response.Content.ToString());
                    
                    UserInfo.Email = user.Data.Email;
                    UserInfo.Name = user.Data.Name;
                    UserInfo.ID = tempUser.Data.userId;
                    UserInfo.Token = tempUser.Token;
                    UserInfo.Role = tempUser.Data.Role;
                    MessageBox.Show("Здравствуйте, " + tempUser.Data.Email, "Успешный вход", MessageBoxButton.OK);
                    MainPage mainPage = new MainPage();
                    this.Visibility = Visibility.Hidden;
                    mainPage.Show();
                }
            }
        }
        private void OpenRegPage(object sender, RoutedEventArgs e)
        {
            RegPage regPage = new RegPage();
            this.Visibility = Visibility.Hidden; //Скрывает текущее окно
            regPage.Show();
        }
        private void AuthButton_Clicked(object sender, RoutedEventArgs e)
        {
            string _Email = textboxLogin.Text.Trim().ToLower(); //ToLower() - Перевод всех символов строки в нижний регистр
            string _password = passwordboxPassword.Password.Trim(); //Trim() - Удаление лишних символов
            bool loginCorrect = false;
            bool passCorrect = false;
            //Логика авторизации
            if (_Email.Length < 5)
            {
                textboxLogin.ToolTip = "Логин слишком короткий.\n(Минимальная длина - 5 символов)"; //ToolTip - Выдаёт подсказку при наведении курсора мыши на объект
                textboxLogin.BorderBrush = Brushes.Red;
            } 
            else if(!_Email.Contains("@") || (!_Email.Contains(".")))
            {
                textboxLogin.ToolTip = "Введены некорректные данные.\n(Возможно отсутствует символ \"@\" или символ \".\")";
                textboxLogin.BorderBrush = Brushes.Red;
            } 
            else //Логин верен
            {
                textboxLogin.ToolTip = _Email;
                textboxLogin.BorderBrush = Brushes.Gray;
                loginCorrect = true;
            }
            if (_password.Length < 8)
            {
                passwordboxPassword.ToolTip = "Пароль слишком короткий.\nМинимальная длина пароля - 8 символов.";
                passwordboxPassword.BorderBrush = Brushes.Red;
            }
            else //Пароль верен
            {
                passwordboxPassword.ToolTip = "";
                passwordboxPassword.BorderBrush = Brushes.Gray;
                passCorrect = true;
            }
            if (loginCorrect && passCorrect)
            {
                try
                {
                    var client = new RestClient("http://testingwebrtc.herokuapp.com/login");
                    client.Timeout = -1;
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                    request.AddParameter("email", _Email.ToString());
                    request.AddParameter("password", _password.ToString());
                    IRestResponse response = client.Execute(request);

                    string responseData = response.Content.ToString();

                    JSONauth tempUser = JsonConvert.DeserializeObject<JSONauth>(responseData);

                    UserInfo.Email = tempUser.Data.Email;
                    //UserInfo.ID = tempUser.ID;
                    UserInfo.Role = tempUser.Data.Role;
                    UserInfo.Token = tempUser.Token;
                    GuestInfo.Name = "test";
                    UserInfo.Name = tempUser.Data.Name;
                    MessageBox.Show("Здравствуйте, " + textboxLogin.Text, "Успешный вход", MessageBoxButton.OK);
                    MainPage mainPage = new MainPage();
                    this.Visibility = Visibility.Hidden;
                    mainPage.Show();
                    if (checkboxRemember.IsChecked == true)
                    {
                        UserInfo JSONUser = new UserInfo();
                        
                        File.WriteAllText(@"Token.json", JsonConvert.SerializeObject(tempUser));

                        // serialize JSON directly to a file
                        using (StreamWriter file = File.CreateText(@"Token.json"))
                        {
                            JsonSerializer serializer = new JsonSerializer();
                            serializer.Serialize(file, tempUser);
                        }
                    }


                }
                catch
                {
                    passwordboxPassword.ToolTip = "Неверный пароль.";
                    passwordboxPassword.BorderBrush = Brushes.Red;
                    //MessageBox.Show("Введен неверный логин или пароль", "Ошибка", MessageBoxButton.OK);
                }
            }
        }
        private void OpenMainPageGuest(object sender, RoutedEventArgs e) //Открытие главного окна без входа в аккаунт ("Гостевой Профиль")
        {
            //NameEnterPage nameEnterPage = new NameEnterPage();
            //nameEnterPage.ShowDialog(); //ShowDialog открывает окно поверх, блокируя основное
            NameEnterPage nameEnterPage = new NameEnterPage();
            this.Visibility = Visibility.Hidden;
            nameEnterPage.Show();
        }
        private void OnClosing(object sender, CancelEventArgs cancelEventArgs) //Подтверждения выхода из программы
        {
            if (MessageBox.Show(this, "Вы действительно хотите выйти ? ", "Подтверждение", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                cancelEventArgs.Cancel = true;
            }
            else
            {
                Process.GetCurrentProcess().Kill(); //Полное выключение программы
            }

        }
        private void checkboxRemember_Click(object sender, RoutedEventArgs e)
        {
            if (checkboxRemember.IsChecked == true) //Запоминает информацию для последующего входа в аккаунт без необходимости вводить данные
            {

            }
            else
            {
                //pass
            }
        }
    }
}
