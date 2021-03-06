using GraphPAD.Data.User;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
            GuestInfo.Name = _Name;
            if (_Name.Length < 3)
            {
                textboxName.ToolTip = "Имя слишком короткое.\n(Минимальная длина - 3 символа)"; //ToolTip - Выдаёт подсказку при наведении курсора мыши на объект
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
            if (MessageBox.Show(this, "Вы действительно хотите выйти ? ", "Подтверждение", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                cancelEventArgs.Cancel = true;
            }
            else
            {
                Process.GetCurrentProcess().Kill();
            }

        }
    }
}
