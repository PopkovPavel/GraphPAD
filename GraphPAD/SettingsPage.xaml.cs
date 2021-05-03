using CefSharp;
using CefSharp.Wpf;
using GraphPAD.Data.JSON;
using GraphPAD.Data.User;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace GraphPAD
{
    public partial class SettingsPage : Window
    {
        public SettingsPage()
        {
            InitializeComponent();
            
            //Avatar.UpdateLayout();

            userNameTextBlock.Text = UserInfo.Name;
            userRoleTextBlock.Text = "Пользователь";
            nameStingTextBlock.Text = UserInfo.Name;
            IDStingTextBlock.Text = UserInfo.ID;
            emailStingTextBlock.Text = UserInfo.Email;
            //if (GuestInfo.Name == "exist")
            //{
            //    userNameTextBlock.Text = UserInfo.Name;
            //    userRoleTextBlock.Text = UserInfo.Role;
            //    nameStingTextBlock.Text = UserInfo.Name;
            //    IDStingTextBlock.Text = UserInfo.ID;
            //    emailStingTextBlock.Text = UserInfo.Email;
            //}
            //else
            //{
            //    userNameTextBlock.Text = GuestInfo.Name;
            //    userRoleTextBlock.Text = UserInfo.Role;
            //    nameStingTextBlock.Text = GuestInfo.Name;
            //    IDStingTextBlock.Text = "...";
            //    emailStingTextBlock.Text = "...";
            //}
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

        private void Button_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            avatarButton.Content = "Изменить";
            avatarButton.Opacity = 0.8;
        }

        private void avatarButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            avatarButton.Content = null;
            avatarButton.Opacity = 0;
        }

        private void avatarButton_Clicked(object sender, RoutedEventArgs e)
        {
            //fix this cringe! :(

            //Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            //dlg.FileName = ""; // Default file name
            //dlg.DefaultExt = ".png"; // Default file extension
            //dlg.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*"; // Filter files by extension
            //dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //Nullable<bool> result = dlg.ShowDialog();
            //if (result == true)
            //{
            //    try
            //    {
            //        if (!Directory.Exists(Path.GetFullPath(@"Avatars")))
            //        {
            //            Directory.CreateDirectory(Path.GetFullPath(@"Avatars"));
            //        }
            //        string filepath = dlg.FileName;
            //        string fileToReplace = Path.GetFullPath(@"Avatars\Avatar.png");
            //        //Console.WriteLine("New Avatar - " + filepath);
            //        //Console.WriteLine("Avatar path - " + fileToReplace);
            //        File.Copy(filepath, fileToReplace, true);

            //        //MessageBox.Show("Чтобы изменения вступили в силу, небходимо перезапустить приложение.", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);

            //        //Обновление аватарки
            //        Avatar.Source = null;
            //        var converter = new ImageSourceConverter();
            //        Avatar.Source = (ImageSource)converter.ConvertFromString(@"Avatars\Avatar.png");
            //        //Console.WriteLine(Avatar.Source);
            //    }
            //    catch (IOException copyError)
            //    {
            //        Console.WriteLine(copyError.Message);
            //        MessageBox.Show("Что-то пошло не так.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            //    }
            //}
        }

        private void ChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void OnClosing(object sender, CancelEventArgs cancelEventArgs)
        {

        }
    }
}
