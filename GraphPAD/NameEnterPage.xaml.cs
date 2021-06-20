using GraphPAD.Data.User;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace GraphPAD
{
    public partial class NameEnterPage : Window
    {
        public NameEnterPage()
        {
            InitializeComponent();
            Closing += OnClosing; //Делегат для отлова закрытия окна
        }
        private void AuthButtonNoAcc_Clicked(object sender, RoutedEventArgs e)
        {
            string _Name = textboxName.Text.Trim(); //ToLower() - Перевод всех символов строки в нижний регистр
            UserInfo.Name = _Name;
            MainPage.isGuestConnected = true;
            if (_Name.Length < 3)
            {
                textboxName.ToolTip = Properties.Language.TooShortNameTooltip; //ToolTip - Выдаёт подсказку при наведении курсора мыши на объект
                textboxName.BorderBrush = Brushes.Red;
            }
            else //Имя подходит
            {
                textboxName.ToolTip = _Name;
                textboxName.BorderBrush = Brushes.Gray;
                MainPage mainPage = new MainPage();
                UserInfo.Name = _Name;
                this.Visibility = Visibility.Hidden;
                mainPage.Show();
            }
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
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.AuthButtonNoAcc.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
            }
        }
    }
}
