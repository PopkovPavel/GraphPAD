using GraphPAD.Data.JSON;
using GraphPAD.Data.User;
using Microsoft.Win32;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GraphPAD
{
    public partial class SettingsPage : Window
    {
        #region Global Variables

        string avatarsFolder = Path.GetFullPath(@"Avatars\Avatar.png");
        string newAvatar;
        public ComboBoxItem selectedItem;
        #endregion
        #region Initialize
        public SettingsPage()
        {
            InitializeComponent();
            if (Properties.Language.Culture?.Name == "ru-RU" || Properties.Language.Culture == null)
            {
                countryFlag.Source = ImageSourceFromBitmap(Properties.Resources.russia);
            }
            else
            {
                countryFlag.Source = ImageSourceFromBitmap(Properties.Resources.england);
            }
            DataContext = this;
            AccountCanvasScrollView.Visibility = Visibility.Visible;
            VoiceCanvas.Visibility = Visibility.Hidden;
            InterfaceCanvas.Visibility = Visibility.Hidden;
            SettingsAvatar.Source = NonBlockingLoad(UserInfo.Avatar);
            userNameTextBlock.Text = UserInfo.Name;
            userRoleTextBlock.Text = Properties.Language.UserString;
            nameStingTextBlock.Text = UserInfo.Name;
            IDStingTextBlock.Text = UserInfo.ID;
            emailStingTextBlock.Text = UserInfo.Email;
            if (!MainPage.isGuestConnected)
            {
                userNameTextBlock.Text = UserInfo.Name;
                userRoleTextBlock.Text = Properties.Language.UserString;
                nameStingTextBlock.Text = UserInfo.Name;
                IDStingTextBlock.Text = UserInfo.ID;
                emailStingTextBlock.Text = UserInfo.Email;
            }
            else
            {
                userNameTextBlock.Text = UserInfo.Name;
                userRoleTextBlock.Text = Properties.Language.GuestString;
                nameStingTextBlock.Text = UserInfo.Name;
                IDStingTextBlock.Text = "...";
                emailStingTextBlock.Text = "...";
                ChangePasswordButton.IsEnabled = false;
                VoiceButton.IsEnabled = false;
            }
        }
        #endregion
        #region Left Buttons
        private void AccountButton_Click(object sender, RoutedEventArgs e)
        {
            AccountCanvasScrollView.Visibility = Visibility.Visible;//AccountCanvas.Visibility = Visibility.Visible;
            VoiceCanvas.Visibility = Visibility.Hidden;
            InterfaceCanvas.Visibility = Visibility.Hidden;
        }
        private void VoiceButton_Click(object sender, RoutedEventArgs e)
        {
            AccountCanvasScrollView.Visibility = Visibility.Hidden;//AccountCanvas.Visibility = Visibility.Hidden;
            VoiceCanvas.Visibility = Visibility.Visible;
            InterfaceCanvas.Visibility = Visibility.Hidden;
        }
        private void InterfaceButton_Click(object sender, RoutedEventArgs e)
        {
            AccountCanvasScrollView.Visibility = Visibility.Hidden;//AccountCanvas.Visibility = Visibility.Hidden;
            VoiceCanvas.Visibility = Visibility.Hidden;
            InterfaceCanvas.Visibility = Visibility.Visible;
        }
        #endregion
        #region Account Settings
        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            File.Delete(@"Token.json");
            this.Close();
            MainPage.CloseForm();
        }
        private void vkButton_Clicked(object sender, RoutedEventArgs e)
        {
            Process.Start("https://vk.com/graphpad");
        }

        private void ytButton_Clicked(object sender, RoutedEventArgs e)
        {
            Process.Start("https://www.youtube.com/channel/UChP-i2J5yW3yT4u04VbZjug");
        }

        private void webButton_Clicked(object sender, RoutedEventArgs e)
        {
            Process.Start("http://www.google.com");
        }
        private void ChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            AccountCanvas.Height = 470;
            newPasswordBox.Visibility = Visibility.Visible;
            CancelChangePassword.Visibility = Visibility.Visible;
            ConfirmChangePasswordButton.Visibility = Visibility.Visible;
            AccountCanvasScrollView.ScrollToBottom();
        }
        private void CancelChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            newPasswordBox.ToolTip = null;
            newPasswordBox.BorderBrush = Brushes.Black;

            AccountCanvas.Height = 390;
            newPasswordBox.Password = null;
            newPasswordBox.Visibility = Visibility.Hidden;
            CancelChangePassword.Visibility = Visibility.Hidden;
            ConfirmChangePasswordButton.Visibility = Visibility.Hidden;
        }
        private void ConfirmChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            string _newPassword = newPasswordBox.Password.Trim();
            if (_newPassword == null || _newPassword == "")
            {
                newPasswordBox.ToolTip = Properties.Language.EnterNewPasswordString;
                newPasswordBox.BorderBrush = Brushes.Red;
            }
            else if (_newPassword.Length < 8)
            {
                newPasswordBox.ToolTip = Properties.Language.TooShortPassword;
                newPasswordBox.BorderBrush = Brushes.Red;
            }
            if (_newPassword.Length >= 8)
            {
                newPasswordBox.ToolTip = null;
                newPasswordBox.BorderBrush = Brushes.Black;
                //Изменение пароля на новый
                var client = new RestClient($"http://testingwebrtc.herokuapp.com/user");
                client.Timeout = -1;
                var request = new RestRequest(Method.PUT);
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddHeader("x-access-token", $"{UserInfo.Token}");
                request.AddParameter("password", _newPassword.ToString());
                IRestResponse response = client.Execute(request);
                if (response.IsSuccessful)
                {
                    File.Delete(@"Token.json");
                    MessageBox.Show(Properties.Language.SuccessPasswordChange, Properties.Language.Caption, MessageBoxButton.OK);
                    AccountCanvas.Height = 390;
                    newPasswordBox.Password = null;
                    newPasswordBox.Visibility = Visibility.Hidden;
                    CancelChangePassword.Visibility = Visibility.Hidden;
                }
                else
                {
                    MessageBox.Show(Properties.Language.SomethingWentWrong, Properties.Language.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        #endregion
        #region Voice & Audio Settings
        //aeee
        #endregion
        #region Interface Settings
        private void languagesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            selectedItem = (ComboBoxItem)comboBox.SelectedItem;
        }

        private void ChangeLanguageButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedItem != null)
            {
                if (selectedItem.Tag.ToString() == "ru")
                {
                    MessageBox.Show(Properties.Language.MessageBoxLanguageText, Properties.Language.Caption, MessageBoxButton.OK, MessageBoxImage.Information);
                    //Properties.Language.Culture = new System.Globalization.CultureInfo("ru-RU");
                    File.WriteAllText(@"Language.txt", "ru-RU");
                }
                else
                {
                    MessageBox.Show(Properties.Language.MessageBoxLanguageText, Properties.Language.Caption, MessageBoxButton.OK, MessageBoxImage.Information);
                    //Properties.Language.Culture = new System.Globalization.CultureInfo("en-US");
                    File.WriteAllText(@"Language.txt", "en-US");
                }
            }
            else
            {
                MessageBox.Show(Properties.Language.LanguageHint, Properties.Language.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
        #region Avatar Changer
        private void AvatarButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            avatarButton.Content = Properties.Language.ChangeLangBtn;
            avatarButton.Opacity = 0.8;
        }
        private void AvatarButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            avatarButton.Content = null;
            avatarButton.Opacity = 0;
        }
        public static ImageSource NonBlockingLoad(string path)
        {
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = new Uri(path);
            image.EndInit();
            image.Freeze();
            return image;
        }
        private void AvatarButton_Clicked(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.FileName = "Avatar"; // Default file name
            dlg.DefaultExt = ".jpg"; // Default file extension
            dlg.Filter = "Image files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png|All files (*.*)|*.*"; // Filter files by extension
            dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            dlg.Title = Properties.Language.ChooseNewAvatar;
            bool? result = dlg.ShowDialog();
            if (result == true && result != null)
            {
                try
                {
                    newAvatar = dlg.FileName;
                    File.Copy(newAvatar, avatarsFolder, true);
                    SettingsAvatar.Source = NonBlockingLoad(newAvatar);
                    UserInfo.Avatar = newAvatar;
                }
                catch (IOException copyError)
                {
                    Console.WriteLine(copyError.Message);
                    MessageBox.Show(Properties.Language.SomethingWentWrong, Properties.Language.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        #endregion
        #region Etc.
        public ImageSource ImageSourceFromBitmap(System.Drawing.Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            catch { return null; }
        }
        private void OnClosing(object sender, CancelEventArgs cancelEventArgs)
        {

        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
