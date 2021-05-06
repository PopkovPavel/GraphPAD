using GraphPAD.Data.User;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GraphPAD
{
    public partial class SettingsPage : Window
    {
        #region Global Variables
        string avatarsFolder = Path.GetFullPath(@"Avatars\Avatar.png");
        string newAvatar;
        #endregion
        #region Initialize
        public SettingsPage()
        {
            InitializeComponent();
            SettingsAvatar.Source = NonBlockingLoad(UserInfo.Avatar);
            userNameTextBlock.Text = UserInfo.Name;
            userRoleTextBlock.Text = "Пользователь";
            nameStingTextBlock.Text = UserInfo.Name;
            IDStingTextBlock.Text = UserInfo.ID;
            emailStingTextBlock.Text = UserInfo.Email;
            if (UserInfo.Name != null)
            {
                userNameTextBlock.Text = UserInfo.Name;
                userRoleTextBlock.Text = "Пользователь";
                nameStingTextBlock.Text = UserInfo.Name;
                IDStingTextBlock.Text = UserInfo.ID;
                emailStingTextBlock.Text = UserInfo.Email;
            }
            else
            {
                userNameTextBlock.Text = GuestInfo.Name;
                userRoleTextBlock.Text = "Гость";
                nameStingTextBlock.Text = GuestInfo.Name;
                IDStingTextBlock.Text = "...";
                emailStingTextBlock.Text = "...";
            }
        }
        #endregion
        #region Left Buttons
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

        }
        #endregion
        #region Avatar Changer
        private void avatarButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            avatarButton.Content = "Изменить";
            avatarButton.Opacity = 0.8;
        }
        private void avatarButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
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
        private void avatarButton_Clicked(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.FileName = "Avatar"; // Default file name
            dlg.DefaultExt = ".jpg"; // Default file extension
            dlg.Filter = "Image files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png|All files (*.*)|*.*"; // Filter files by extension
            dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            dlg.Title = "Выберите новый аватар";
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
                    MessageBox.Show("Что-то пошло не так.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        #endregion
        #region Etc.
        private void OnClosing(object sender, CancelEventArgs cancelEventArgs)
        {

        }
        #endregion
    }
}
