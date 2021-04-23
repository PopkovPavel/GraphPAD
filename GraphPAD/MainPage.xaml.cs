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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Resources;

namespace GraphPAD
{
    public partial class MainPage : Window
    {
        #region Global Variables
        public bool isMicOn;
        public bool isHeadPhonesOn;
        public bool isVideoOn;
        public bool isAddVetexOn;
        public bool isRemoveVertexOn;
        public bool isConnectVertexOn;
        public bool isDisconnectVertexOn;
        public bool isGraphGeneratorOn;
        public bool isAlgorithmsOn;
        public bool isFreeModeOn;
        private bool _flag; //Флаг для логики микрофона (Проверка на то, был ли выключен микрофон до выключения звука)
        public int lobbyCount;
        public int lobbyButtonsMargin = -70;
        public int chatCount;
        public int chatTextblockMargin;

        delegate void Function(object sender, MouseButtonEventArgs e);
        Function function;

        public SolidColorBrush greenbrush = new SolidColorBrush(Color.FromRgb(19, 199, 19));
        public SolidColorBrush lightpurple = new SolidColorBrush(Color.FromRgb(85, 85, 147));
        public delegate void Method();
        private static Method close;
        private string ConferensionName;
        #endregion
        #region Initialize
        public MainPage()
        {
            InitializeComponent();
            close = new Method(Close);
            this.KeyDown += new KeyEventHandler(MainWindow_KeyDown);
            Closing += OnClosing;  //Делегат для отлова закрытия окна
            isMicOn = true;
            isHeadPhonesOn = true;
            isVideoOn = false;
            _flag = false;
            lobbyCount = 0;
            chatCount = 0;
            lobbyButtonsMargin = 10;
            chatTextblockMargin = 5;
            voiceChatTextBlock.Text = "Голосовой чат подключен";
            videoTextBlock.Text = "Видео отключено";
            videoTextBlock.Foreground = Brushes.DarkGray;
            function = null;
            isAddVetexOn = false;
            isRemoveVertexOn = false;
            isConnectVertexOn = false;
            isDisconnectVertexOn = false;
            isGraphGeneratorOn = false;
            isAlgorithmsOn = false;
            isFreeModeOn = false;
            FreeModeCanvas.Visibility = Visibility.Hidden;
            TextChatCanvas.Visibility = Visibility.Visible;
            conferenssionString.Text = "Конференция № ...";
            if (GuestInfo.Name == "exist")
            {
                nameString.Text = UserInfo.Name;
                userRoleString.Text = "Пользователь";
            }
            else
            {
                nameString.Text = GuestInfo.Name;
                userRoleString.Text = "Гость";
                
            }
            nameString.Text = UserInfo.Name;
            userRoleString.Text = "Пользователь";
            GraphControlCanvas.Visibility = Visibility.Hidden;
            infoTextBlock.Visibility = Visibility.Visible;
            leaveButton.Visibility = Visibility.Hidden;
            CancelEnterLobbyButton.Visibility = Visibility.Hidden;
            ConfirmEnterLobbyButton.Visibility = Visibility.Hidden;
            ConferensionIDTextBox.Visibility = Visibility.Hidden;
            NewConferensionNameTextBox.Visibility = Visibility.Hidden;
            CancelCreateLobbyButton.Visibility = Visibility.Hidden;
            ConfirmCreateLobbyButton.Visibility = Visibility.Hidden;
            chatGrid.Visibility = Visibility.Hidden;
            Chromium.settings.CefCommandLineArgs.Add("enable-media-stream", "1");
            Cef.Initialize(Chromium.settings);
            leaveButton.Click += (s, ea) =>
            {
                LobbyLeave_ClickAsync(s,ea);
                ChatBox.Clear();
                chatTextBox.Clear();
                foreach(UIElement temp in VideoChatCanvas.Children)
                {
                    try
                    {
                        ((ChromiumWebBrowser)temp).Dispose();
                    }
                    catch { }

                }
                VideoChatCanvas.Children.Clear();
                //Кнопка "Покинуть"
                BGgrid.Background = lightpurple;
                freeModeBtn.Visibility = Visibility.Hidden;
                PaintControlCanvas.Visibility = Visibility.Hidden;
                ButtonsFix();

            };
            RefreshRooms();
        }
        #endregion
        #region Functions
        public void ButtonsFix()
        {
            isAddVetexOn = false;
            isRemoveVertexOn = false;
            isConnectVertexOn = false;
            isDisconnectVertexOn = false;
            isGraphGeneratorOn = false;
            isAlgorithmsOn = false;
            isFreeModeOn = false;
            addVertexBtn.Background = Brushes.Transparent;
            deleteVertexBtn.Background = Brushes.Transparent;
            connectVertexBtn.Background = Brushes.Transparent;
            disconnectVertexBtn.Background = Brushes.Transparent;
            graphGeneratorBtn.Background = Brushes.Transparent;
            algorithmsBtn.Background = Brushes.Transparent;
            addVertexBtn.IsEnabled = true;
            deleteVertexBtn.IsEnabled = true;
            connectVertexBtn.IsEnabled = true;
            disconnectVertexBtn.IsEnabled = true;
            graphGeneratorBtn.IsEnabled = true;
            algorithmsBtn.IsEnabled = true;
            addVertexBtn.Visibility = Visibility.Visible;
            deleteVertexBtn.Visibility = Visibility.Visible;
            connectVertexBtn.Visibility = Visibility.Visible;
            disconnectVertexBtn.Visibility = Visibility.Visible;
            graphGeneratorBtn.Visibility = Visibility.Visible;
            algorithmsBtn.Visibility = Visibility.Visible;
            currentMode.Text = "Текущий режим: Курсор";

            CancelCreateLobbyButton.Visibility = Visibility.Hidden;
            ConfirmCreateLobbyButton.Visibility = Visibility.Hidden;
            NewConferensionNameTextBox.Visibility = Visibility.Hidden;
            EnterLobbyButton.Visibility = Visibility.Visible;
            ConferensionIDTextBox.Text = "";
            ConferensionIDTextBox.BorderBrush = Brushes.Transparent;
            ConferensionIDTextBox.ToolTip = null;
            NewConferensionNameTextBox.Text = "";
            NewConferensionNameTextBox.BorderBrush = Brushes.Transparent;
            NewConferensionNameTextBox.ToolTip = null;
        }
        public void RefreshRooms()
        {
            try
            {
                lobbyCount = 0;
                lobbyButtonsMargin = -70;
                var client = new RestClient("https://testingwebrtc.herokuapp.com/room/myrooms");
                client.Timeout = -1;
                var request = new RestRequest(RestSharp.Method.GET);
                request.AddHeader("x-access-token", UserInfo.Token);
                IRestResponse response = client.Execute(request);
                if (response.IsSuccessful)
                {
                    LobbysCanvas.Children.Clear();
                    JSONrooms rooms = JsonConvert.DeserializeObject<JSONrooms>(response.Content.ToString());
                    foreach (JSONroomData room in rooms.Data)
                    {
                        lobbyCount += 1;
                        lobbyButtonsMargin += 70;
                        if (lobbyCount > 8)
                        {
                            LobbysCanvas.Height = LobbysCanvas.Height + 70;
                        }
                        ConferensionsCountTextBlock.Text = "Конференций: " + (lobbyCount);

                        var menuCopyItem = new MenuItem()
                        {
                            Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF181840"),
                            Foreground = Brushes.White,
                            FontFamily = new FontFamily("Segoe UI"),
                            FontSize = 14,
                            FontStyle = FontStyles.Normal,
                            FontWeight = FontWeights.Bold,
                            Cursor = Cursors.Hand,
                            Padding = new Thickness(10, 0, 0, 0),
                            Height = 40,
                            Width = 190,
                            Header = "Скопировать ID",
                            ToolTip = "Скопировать ID конференции в буфер"
                        };

                        menuCopyItem.Click += (sender1, e1) =>
                        {
                            //Копирование текста в буфер обмена
                            Clipboard.SetData(DataFormats.Text, (Object)room.RoomID);
                            MessageBox.Show("ID скопирован", "Сообщение");
                        };

                        //Второй элемент контекстного меню
                        var menuLeaveItem = new MenuItem()
                        {
                            Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF181840"),
                            Foreground = Brushes.White,
                            FontFamily = new FontFamily("Segoe UI"),
                            FontSize = 14,
                            FontStyle = FontStyles.Normal,
                            FontWeight = FontWeights.Bold,
                            Cursor = Cursors.Hand,
                            Padding = new Thickness(10, 0, 0, 0),
                            Height = 40,
                            Width = 190,
                            ToolTip = "Покинуть конференцию"
                        };
                        menuLeaveItem.Header = "Покинуть конференцию";

                        var contextMenu = new ContextMenu()
                        {
                            Background = Brushes.Transparent
                        };
                        contextMenu.Items.Add(menuLeaveItem);
                        contextMenu.Items.Add(menuCopyItem);

                        var tempButton = new Button()
                        {
                            Width = 64,
                            Height = 64,
                            Margin = new Thickness(15, lobbyButtonsMargin, 0, 0),
                            BorderBrush = null,
                            ToolTip = room.RoomName,
                            ContextMenu = contextMenu
                        };

                        tempButton.Click += Ez_Click;

                        menuLeaveItem.Click += (s, ea) => 
                        {
                            LeaveRoom($"{room.RoomID}",$"{room.RoomName}");
                        };

                        tempButton.Click += (senda, ev) =>
                        {
                            OpenRoomAsync($"{room.RoomID}",$"{room.RoomName}");
                            //Кнопка конференции слева
                            BGgrid.Background = Brushes.DarkGray;
                            isAddVetexOn = false;
                            isRemoveVertexOn = false;
                            isConnectVertexOn = false;
                            isDisconnectVertexOn = false;
                            isGraphGeneratorOn = false;
                            isAlgorithmsOn = false;
                            isFreeModeOn = false;
                            freeModeBtn.Visibility = Visibility.Visible;
                            freeModeBtn.Content = "Графы";

                        };

                        var path = "Resources/account.png";
                        Uri resourceUri = new Uri(path, UriKind.Relative);
                        StreamResourceInfo streamInfo = Application.GetResourceStream(resourceUri);
                        BitmapFrame temp = BitmapFrame.Create(streamInfo.Stream);
                        tempButton.Background = new ImageBrush(temp);                        
                        LobbysCanvas.Children.Add(tempButton);
                    }
                }
                else
                {
                    MessageBox.Show("Что-то пошло не так.", "Ошибка");
                }
            }
            catch
            {
                MessageBox.Show("Что-то пошло не так. Сообщите об этом администратору ribalko2006@mail.ru", "Ошибка");

            }
        }
        public void LeaveRoom(string roomId, string roomName)
        {
            try
            {
                var client = new RestClient($"https://testingwebrtc.herokuapp.com/room/{roomId}/delete");
                client.Timeout = -1;
                var request = new RestRequest(RestSharp.Method.DELETE);
                request.AddHeader("x-access-token", UserInfo.Token);
                IRestResponse response = client.Execute(request);
                if (response.IsSuccessful)
                {
                    MessageBox.Show($"Вы покинули конференцию \"{roomName}\"", "Сообщение");
                    RefreshRooms();
                }
                else
                {
                    MessageBox.Show("Вы не можете удалить собственную комнату\nСкоро сможете", "Ошибка");
                }
            }
            catch
            {
                MessageBox.Show("Что-то пошло не так. Сообщите об этом администратору ribalko2006@mail.ru", "Ошибка");

            }
        }
        public Tuple<JSONroomuser[], JSONroomuser> GetUsers(string roomId)
        {
            var client = new RestClient($"https://testingwebrtc.herokuapp.com/room/{roomId}");
            client.Timeout = -1;
            var request = new RestRequest(RestSharp.Method.GET);
            request.AddHeader("x-access-token", UserInfo.Token);
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                JSONroom room = JsonConvert.DeserializeObject<JSONroom>(response.Content.ToString());
                Console.WriteLine(room.Data.Users.ToString());
                return Tuple.Create(room.Data.Users, room.Data.RoomOwner);
            }
            return null;
        }
        public async System.Threading.Tasks.Task OpenRoomAsync(string roomId, string roomName)
        {
            Chromium.SetSettings(roomId);
            var jepa = Chromium.Connect();
            jepa.Height = 720;
            jepa.Width = 405;
            
            VideoChatCanvas.Children.Add(jepa);
            chatGrid.Visibility = Visibility.Visible;
            GraphCanvas.Visibility = Visibility.Visible;
            GraphControlCanvas.Visibility = Visibility.Visible;
            leaveButton.Visibility = Visibility.Visible;
            LobbysCanvas.Visibility = Visibility.Hidden;

            TextChatCanvas.Visibility = Visibility.Visible;
            VideoChatCanvas.Visibility = Visibility.Hidden;
            ParticipantsCanvas.Visibility = Visibility.Hidden;
            ChatBox.Visibility = Visibility.Visible;
            ParticipantsBox.Visibility = Visibility.Hidden;
            ParticipantsString.Visibility = Visibility.Hidden;
            ParticipantsScrollView.Visibility = Visibility.Hidden;

            conferenssionString.Text = $"Конференция \"{roomName}\"";
            ConferensionString.Text = $"Чат конференции \"{roomName}\"";
            //Participants
            int num = 1;
            var temp = GetUsers(roomId);
            ParticipantsBox.AppendText($"Owner: {temp.Item2.Name}\n\n");
            if (temp.Item1 != null)
            {
                foreach (JSONroomuser participant in temp.Item1)
                {
                    ParticipantsBox.AppendText($"#{num++}: {participant.Name}\n");
                    //Console.WriteLine(participant.ToString());
                }
                try
                {
                    await SocketConnector.InitializeClientAsync();
                    SocketConnector.SetSettings(roomId, UserInfo.Name);
                    SocketConnector.client.On("chat-message", async response =>
                    {
                        var text = JsonConvert.DeserializeObject<JSONmessage[]>(response.ToString());
                        await Dispatcher.BeginInvoke((Action)(() => ChatBox.AppendText($"{text[0].UserId}: {text[0].Message}\n\n")));
                        Console.WriteLine($"{text[0].UserId}: {text[0].Message}");
                    });
                    chatTextBox.IsReadOnly = (SocketConnector.IsConnected) ? false : true;
                }
                catch { }
            }        
        }
        #endregion
        #region Image Changer
        private void ChangeImage(string path, Button btn) //Функция для смены изображений в кнопке
        {
            Uri resourceUri = new Uri(path, UriKind.Relative);
            StreamResourceInfo streamInfo = Application.GetResourceStream(resourceUri);
            BitmapFrame temp = BitmapFrame.Create(streamInfo.Stream);
            var brush = new ImageBrush();
            brush.ImageSource = temp;
            btn.Background = brush;
        }
        #endregion
        #region Enter Lobby
        private void EnterLobby_Click(object sender, RoutedEventArgs e)
        {
            if (UserInfo.Email != null)
            {
                CreateLobbyButton.Visibility = Visibility.Hidden;
                ConferensionIDTextBox.Visibility = Visibility.Visible;
                CancelEnterLobbyButton.Visibility = Visibility.Visible;
                ConfirmEnterLobbyButton.Visibility = Visibility.Visible;
            }
        }
        private void CancelEnterLobby_Click(object sender, RoutedEventArgs e)
        {
            CreateLobbyButton.Visibility = Visibility.Visible;
            ConferensionIDTextBox.Visibility = Visibility.Hidden;
            CancelEnterLobbyButton.Visibility = Visibility.Hidden;
            ConfirmEnterLobbyButton.Visibility = Visibility.Hidden;
            ConferensionIDTextBox.Text = "";
            ConferensionIDTextBox.BorderBrush = Brushes.Transparent;
            ConferensionIDTextBox.ToolTip = null;
        }
        private void ConfirmEnterLobby_Click(object sender, RoutedEventArgs e)
        {
            string _conferensionID = ConferensionIDTextBox.Text.Trim().ToLower(); //Trim() - Удаление лишних пробелов
            if (_conferensionID == "")
            {
                ConferensionIDTextBox.ToolTip = "Введите ID конференции";
                ConferensionIDTextBox.BorderBrush = Brushes.Red;
            }
            else
            {
                try
                {
                    var client = new RestClient("https://testingwebrtc.herokuapp.com/room/" + _conferensionID + "/join");
                    client.Timeout = -1;
                    var request = new RestRequest(RestSharp.Method.POST);
                    request.AddHeader("x-access-token", UserInfo.Token);
                    IRestResponse response = client.Execute(request);
                    if (response.IsSuccessful)
                    {
                        MessageBox.Show("Вы успешно вошли в конференцию.\nЕё ID: " + _conferensionID + "\nЧтобы подкючиться к конференции выберите её в списке слева.", "Сообщение"); 
                        ConferensionIDTextBox.Text = "";
                        ConferensionIDTextBox.BorderBrush = Brushes.Transparent;
                        ConferensionIDTextBox.ToolTip = null;
                        RefreshRooms();
                    }
                    else
                    {
                        MessageBox.Show("Возможно такой конференции не существует, либо она уже была добавлена", "Ошибка");
                    }
                }
                catch
                {
                    MessageBox.Show("Что-то пошло не так. Сообщите об этом администратору ribalko2006@mail.ru", "Ошибка");

                }

            }
        }
        #endregion
        #region Create lobby
        private void CreateLobby_Click(object sender, RoutedEventArgs e)
        {
            EnterLobbyButton.Visibility = Visibility.Hidden;
            ConfirmCreateLobbyButton.Visibility = Visibility.Visible;
            CancelCreateLobbyButton.Visibility = Visibility.Visible;
            NewConferensionNameTextBox.Visibility = Visibility.Visible;
        }
        private void CancelCreateLobby_Click(object sender, RoutedEventArgs e)
        {
            CancelCreateLobbyButton.Visibility = Visibility.Hidden;
            ConfirmCreateLobbyButton.Visibility = Visibility.Hidden;
            NewConferensionNameTextBox.Visibility = Visibility.Hidden;
            EnterLobbyButton.Visibility = Visibility.Visible;
            NewConferensionNameTextBox.Text = "";
            NewConferensionNameTextBox.BorderBrush = Brushes.Transparent;
            NewConferensionNameTextBox.ToolTip = null;
        }
        private void ConfirmCreateLobby_Click(object sender, RoutedEventArgs e)
        {
            ConferensionName = NewConferensionNameTextBox.Text;
            string _newConferensionName = NewConferensionNameTextBox.Text.Trim().ToLower(); //Trim() - Удаление лишних пробелов
            if (_newConferensionName == "")
            {
                NewConferensionNameTextBox.ToolTip = "Введите название конференции";
                NewConferensionNameTextBox.BorderBrush = Brushes.Red;
            }
            else
            if (MessageBox.Show(this, $"Создать новую конференцию?\nНазвание конференции: {ConferensionName}", "Подтверждение", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                //pass
            }
            else
            {
                try
                {
                    var client = new RestClient("https://testingwebrtc.herokuapp.com/room/create");
                    client.Timeout = -1;
                    var request = new RestRequest(RestSharp.Method.POST);
                    request.AddHeader("x-access-token", UserInfo.Token);
                    request.AddParameter("roomName", ConferensionName);
                    IRestResponse response = client.Execute(request);
                    if (response.IsSuccessful)
                    {
                        JSONroom room = JsonConvert.DeserializeObject<JSONroom>(response.Content.ToString());
                        var newRoomID = room.Data.RoomID;
                        var newRoomName = room.Data.RoomName;
                        MessageBox.Show($"Вы успешно создали конференцию\nНазвание: {newRoomName}\nID: {newRoomID}", "Сообщение");
                        NewConferensionNameTextBox.Text = "";
                        NewConferensionNameTextBox.BorderBrush = Brushes.Transparent;
                        NewConferensionNameTextBox.ToolTip = null;
                        RefreshRooms();
                    }
                    else
                    {
                        MessageBox.Show("Что-то пошло не так.", "Ошибка");
                    }
                }
                catch
                {
                    MessageBox.Show("Что-то пошло не так. Сообщите об этом администратору ribalko2006@mail.ru", "Ошибка");
                }
            }
        }
        #endregion
        #region Lobby Leave
        private async System.Threading.Tasks.Task LobbyLeave_ClickAsync(object sender, RoutedEventArgs e)
        {          
            await SocketConnector.Disconnect();
            chatTextBox.IsReadOnly = (SocketConnector.IsConnected) ? false : true;
            chatGrid.Visibility = Visibility.Hidden;
            ConferensionIDTextBox.BorderBrush = Brushes.Transparent;
            NewConferensionNameTextBox.BorderBrush = Brushes.Transparent;
            GraphCanvas.Visibility = Visibility.Hidden;
            FreeModeCanvas.Visibility = Visibility.Hidden;
            GraphControlCanvas.Visibility = Visibility.Hidden;
            conferenssionString.Text = "Конференция № ...";
            leaveButton.Visibility = Visibility.Hidden;

            CreateLobbyButton.Visibility = Visibility.Visible;
            ConferensionIDTextBox.Visibility = Visibility.Hidden;
            CancelEnterLobbyButton.Visibility = Visibility.Hidden;
            ConfirmEnterLobbyButton.Visibility = Visibility.Hidden;
            CancelCreateLobbyButton.Visibility = Visibility.Hidden;
            ConfirmCreateLobbyButton.Visibility = Visibility.Hidden;
            LobbysCanvas.Visibility = Visibility.Visible;
            ParticipantsBox.Text = "";
        }
        #endregion
        #region Text, Video, Chat buttons
        private void TextChatButton_Clicked(object sender, RoutedEventArgs e)
        {
            TextChatCanvas.Visibility = Visibility.Visible;
            VideoChatCanvas.Visibility = Visibility.Hidden;
            ParticipantsCanvas.Visibility = Visibility.Hidden;
            ChatBox.Visibility = Visibility.Visible;
            ParticipantsBox.Visibility = Visibility.Hidden;
            ParticipantsString.Visibility = Visibility.Hidden;
            ParticipantsScrollView.Visibility = Visibility.Hidden;
        }
        private void VideoChatButton_Clicked(object sender, RoutedEventArgs e)
        {
            TextChatCanvas.Visibility = Visibility.Hidden;
            VideoChatCanvas.Visibility = Visibility.Visible;
            ParticipantsCanvas.Visibility = Visibility.Hidden;
            ChatBox.Visibility = Visibility.Hidden;
            ParticipantsBox.Visibility = Visibility.Hidden;
            ParticipantsString.Visibility = Visibility.Hidden;
            ParticipantsScrollView.Visibility = Visibility.Hidden;
        }
        private void ParticipantsButton_Clicked(object sender, RoutedEventArgs e)
        {
            TextChatCanvas.Visibility = Visibility.Hidden;
            VideoChatCanvas.Visibility = Visibility.Hidden;
            ParticipantsCanvas.Visibility = Visibility.Visible;
            ChatBox.Visibility = Visibility.Hidden;
            ParticipantsBox.Visibility = Visibility.Visible;
            ParticipantsString.Visibility = Visibility.Visible;
            ParticipantsScrollView.Visibility = Visibility.Visible;
        }
        #endregion
        #region Microphone
        private void MicButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (isMicOn)
            {
                var path = "Resources/microphone_off.png";
                ChangeImage(path, micButton);
                voiceChatTextBlock.Text = "Голосовой чат выключен";
                voiceChatTextBlock.Foreground = Brushes.Red;
                micButton.ToolTip = "Вкл. микрофон";
                isMicOn = false;
            }
            else
            {
                var path = "Resources/microphone_on.png";
                ChangeImage(path, micButton);
                voiceChatTextBlock.Text = "Голосовой чат подключен";
                voiceChatTextBlock.Foreground = greenbrush;
                micButton.ToolTip = "Выкл. микрофон";
                isMicOn = true;

                path = "Resources/headphones_on.png";
                ChangeImage(path, headphonesButton);
                headphonesButton.ToolTip = "Выкл. звук";
                isHeadPhonesOn = true;
            }
        }
        #endregion
        #region Headphones
        private void HeadPhonesButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (isHeadPhonesOn)
            {
                var path = "Resources/headphones_off.png";
                ChangeImage(path, headphonesButton);
                headphonesButton.ToolTip = "Вкл. звук";
                isHeadPhonesOn = false;

                if (isMicOn)
                {
                    isMicOn = false;
                }
                else
                {
                    _flag = true;
                }
                path = "Resources/microphone_off.png";
                ChangeImage(path, micButton);
                voiceChatTextBlock.Text = "Голосовой чат выключен";
                voiceChatTextBlock.Foreground = Brushes.Red;
                micButton.ToolTip = "Вкл. микрофон";
            }
            else
            {
                var path = "Resources/headphones_on.png";
                ChangeImage(path, headphonesButton);
                headphonesButton.ToolTip = "Выкл. звук";
                isHeadPhonesOn = true;
                if (!_flag)
                {
                    path = "Resources/microphone_on.png";
                    ChangeImage(path, micButton);
                    voiceChatTextBlock.Text = "Голосовой чат подключен";
                    voiceChatTextBlock.Foreground = greenbrush;
                    micButton.ToolTip = "Выкл. микрофон";
                    isMicOn = true;
                }
                else
                {
                    _flag = false;
                }
            }
        }
        #endregion
        #region Video
        private void VideoButton_Clicked(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            btn.Background = btn.Background == Brushes.DarkGreen ? (SolidColorBrush)(new BrushConverter().ConvertFrom("#00000000")) : Brushes.DarkGreen;
            
            if (isVideoOn)
            {
                //Веб-камера выключена
                videoTextBlock.Text = "Видео отключено";
                videoTextBlock.Foreground = Brushes.DarkGray;
                videoButton.ToolTip = "Вкл. камеру";
                isVideoOn = false;
            }
            else
            {
                //Веб-камера включена
                videoTextBlock.Text = "Видео подключено";
                videoTextBlock.Foreground = greenbrush;
                videoButton.ToolTip = "Выкл. камеру";
                isVideoOn = true;
            }
        }
        #endregion
        #region Settings
        private void SettingsButton_Clicked(object sender, RoutedEventArgs e)
        {
            SettingsPage settingsPage = new SettingsPage();
            settingsPage.ShowDialog(); //ShowDialog открывает окно поверх, блокируя основное
        }
        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            File.Delete(@"Token.json");
            AuthPage authPage = new AuthPage();
            this.Visibility = Visibility.Hidden; //Скрывает текущее окно
            authPage.Show();
        }
        #endregion
        #region FreeMode
        private void FreeMode_Click(object sender, RoutedEventArgs e)
        {
            if (!isFreeModeOn)
            {
                isFreeModeOn = true;

                GraphControlCanvas.Visibility = Visibility.Hidden;
                PaintControlCanvas.Visibility = Visibility.Visible;
                GraphCanvas.Visibility = Visibility.Hidden;
                FreeModeCanvas.Visibility = Visibility.Visible;
                BGgrid.Background = Brushes.DarkSlateGray;
                freeModeBtn.Content = "Рисование";
                freeModeBtn.ToolTip = "Включить Графы";
                
            }
            else
            {
                isFreeModeOn = false;
                GraphControlCanvas.Visibility = Visibility.Visible;
                PaintControlCanvas.Visibility = Visibility.Hidden;
                GraphCanvas.Visibility = Visibility.Visible;
                FreeModeCanvas.Visibility = Visibility.Hidden;
                BGgrid.Background = Brushes.DarkGray;
                freeModeBtn.Content = "Графы";
                freeModeBtn.ToolTip = "Включить Рисование";

                ButtonsFix();
            }
            //Button btn = sender as Button;
            //btn.Background = btn.Background == Brushes.DarkGray ? (SolidColorBrush)(new BrushConverter().ConvertFrom("#00000000")) : Brushes.DarkGray;
        }
        #endregion
        #region Graph panel buttons
        private void AddVertex_Click(object sender, RoutedEventArgs e)
        {
            if (!isAddVetexOn)
            {
                //function = AddVertex;
                //addVertexBtn.IsEnabled = false;
                deleteVertexBtn.IsEnabled = false;
                connectVertexBtn.IsEnabled = false;
                disconnectVertexBtn.IsEnabled = false;
                graphGeneratorBtn.IsEnabled = false;
                algorithmsBtn.IsEnabled = false;
                currentMode.Text = "Текущий режим: Добавление вершин";
                isAddVetexOn = true;
                addVertexBtn.ToolTip = "Выключить режим добавления вершин";
            }
            else
            {
                //function = null;
                //addVertexBtn.IsEnabled = true;
                deleteVertexBtn.IsEnabled = true;
                connectVertexBtn.IsEnabled = true;
                disconnectVertexBtn.IsEnabled = true;
                graphGeneratorBtn.IsEnabled = true;
                algorithmsBtn.IsEnabled = true;
                currentMode.Text = "Текущий режим: Курсор";
                isAddVetexOn = false;
                addVertexBtn.ToolTip = "Включить режим добавления вершин";
            }
            Button btn = sender as Button;
            btn.Background = btn.Background == Brushes.DarkGreen ? (SolidColorBrush)(new BrushConverter().ConvertFrom("#00000000")) : Brushes.DarkGreen;
        }
        private void RemoveVertex_Click(object sender, RoutedEventArgs e)
        {
            if (!isRemoveVertexOn)
            {
                //function = DeleteVertex;
                addVertexBtn.IsEnabled = false;
                //deleteVertexBtn.IsEnabled = false;
                connectVertexBtn.IsEnabled = false;
                disconnectVertexBtn.IsEnabled = false;
                graphGeneratorBtn.IsEnabled = false;
                algorithmsBtn.IsEnabled = false;
                currentMode.Text = "Текущий режим: Удаление вершин";
                isRemoveVertexOn = true;
                deleteVertexBtn.ToolTip = "Выключить режим удаления вершин";
            }
            else
            {
                //function = null;
                addVertexBtn.IsEnabled = true;
                //deleteVertexBtn.IsEnabled = true;
                connectVertexBtn.IsEnabled = true;
                disconnectVertexBtn.IsEnabled = true;
                graphGeneratorBtn.IsEnabled = true;
                algorithmsBtn.IsEnabled = true;
                currentMode.Text = "Текущий режим: Курсор";
                isRemoveVertexOn = false;
                deleteVertexBtn.ToolTip = "Включить режим удаления вершин";
            }
            Button btn = sender as Button;
            btn.Background = btn.Background == Brushes.DarkRed ? (SolidColorBrush)(new BrushConverter().ConvertFrom("#00000000")) : Brushes.DarkRed;
        }

        private void ConnectVertex_Click(object sender, RoutedEventArgs e)
        {
            if (!isConnectVertexOn)
            {
                //function = ConnectVertex;
                addVertexBtn.IsEnabled = false;
                deleteVertexBtn.IsEnabled = false;
                //connectVertexBtn.IsEnabled = false;
                disconnectVertexBtn.IsEnabled = false;
                graphGeneratorBtn.IsEnabled = false;
                algorithmsBtn.IsEnabled = false;
                currentMode.Text = "Текущий режим: Соединение вершин";
                isConnectVertexOn = true;
                connectVertexBtn.ToolTip = "Выключить режим соединения вершин";
            }
            else
            {
                //function = null;
                addVertexBtn.IsEnabled = true;
                deleteVertexBtn.IsEnabled = true;
                //connectVertexBtn.IsEnabled = true;
                disconnectVertexBtn.IsEnabled = true;
                graphGeneratorBtn.IsEnabled = true;
                algorithmsBtn.IsEnabled = true;
                currentMode.Text = "Текущий режим: Курсор";
                isConnectVertexOn = false;
                connectVertexBtn.ToolTip = "Включить режим соединения вершин";
            }
            Button btn = sender as Button;
            btn.Background = btn.Background == Brushes.DarkGreen ? (SolidColorBrush)(new BrushConverter().ConvertFrom("#00000000")) : Brushes.DarkGreen;
        }

        private void DisconnectVertex_Click(object sender, RoutedEventArgs e)
        {
            if (!isDisconnectVertexOn)
            {
                //function = DisconnectVertex;
                addVertexBtn.IsEnabled = false;
                deleteVertexBtn.IsEnabled = false;
                connectVertexBtn.IsEnabled = false;
                //disconnectVertexBtn.IsEnabled = false;
                graphGeneratorBtn.IsEnabled = false;
                algorithmsBtn.IsEnabled = false;
                currentMode.Text = "Текущий режим: Удаление связей";
                isDisconnectVertexOn = true;
                disconnectVertexBtn.ToolTip = "Выключить режим удаления связей";
            }
            else
            {
                //function = null;
                addVertexBtn.IsEnabled = true;
                deleteVertexBtn.IsEnabled = true;
                connectVertexBtn.IsEnabled = true;
                //disconnectVertexBtn.IsEnabled = true;
                graphGeneratorBtn.IsEnabled = true;
                algorithmsBtn.IsEnabled = true;
                currentMode.Text = "Текущий режим: Курсор";
                isDisconnectVertexOn = false;
                disconnectVertexBtn.ToolTip = "Включить режим удаления связей";
            }
            Button btn = sender as Button;
            btn.Background = btn.Background == Brushes.DarkRed ? (SolidColorBrush)(new BrushConverter().ConvertFrom("#00000000")) : Brushes.DarkRed;
        }

        private void GraphGenerator_Click(object sender, RoutedEventArgs e)
        {
            if (!isGraphGeneratorOn)
            {
                addVertexBtn.Visibility = Visibility.Hidden;
                deleteVertexBtn.Visibility = Visibility.Hidden;
                connectVertexBtn.Visibility = Visibility.Hidden;
                disconnectVertexBtn.Visibility = Visibility.Hidden;
                //graphGeneratorBtn.Visibility = Visibility.Hidden;
                algorithmsBtn.Visibility = Visibility.Hidden;
                currentMode.Text = "Текущий режим: Генерация графов";                
                isGraphGeneratorOn = true;
                graphGeneratorBtn.ToolTip = "Выключить режим удаления связей";
            }
            else
            {
                addVertexBtn.Visibility = Visibility.Visible;
                deleteVertexBtn.Visibility = Visibility.Visible;
                connectVertexBtn.Visibility = Visibility.Visible;
                disconnectVertexBtn.Visibility = Visibility.Visible;
                //graphGeneratorBtn.Visibility = Visibility.Visible;
                algorithmsBtn.Visibility = Visibility.Visible;
                currentMode.Text = "Текущий режим: Курсор";
                isGraphGeneratorOn = false;
                disconnectVertexBtn.ToolTip = "Включить режим удаления связей";
            }
            Button btn = sender as Button;
            btn.Background = btn.Background == Brushes.DarkGray ? (SolidColorBrush)(new BrushConverter().ConvertFrom("#00000000")) : Brushes.DarkGray;
        }

        private void Algorithms_Click(object sender, RoutedEventArgs e)
        {
            if (!isAlgorithmsOn)
            {
                addVertexBtn.Visibility = Visibility.Hidden;
                deleteVertexBtn.Visibility = Visibility.Hidden;
                connectVertexBtn.Visibility = Visibility.Hidden;
                disconnectVertexBtn.Visibility = Visibility.Hidden;
                graphGeneratorBtn.Visibility = Visibility.Hidden;
                //algorithmsBtn.Visibility = Visibility.Hidden;
                currentMode.Text = "Текущий режим: Алгоритмы";
                isAlgorithmsOn = true;
                graphGeneratorBtn.ToolTip = "Выключить режим удаления связей";
            }
            else
            {
                addVertexBtn.Visibility = Visibility.Visible;
                deleteVertexBtn.Visibility = Visibility.Visible;
                connectVertexBtn.Visibility = Visibility.Visible;
                disconnectVertexBtn.Visibility = Visibility.Visible;
                graphGeneratorBtn.Visibility = Visibility.Visible;
                //algorithmsBtn.Visibility = Visibility.Visible;
                currentMode.Text = "Текущий режим: Курсор";
                isAlgorithmsOn = false;
                disconnectVertexBtn.ToolTip = "Включить режим удаления связей";
            }
            Button btn = sender as Button;
            btn.Background = btn.Background == Brushes.DarkGray ? (SolidColorBrush)(new BrushConverter().ConvertFrom("#00000000")) : Brushes.DarkGray;
        }
        #endregion
        #region Paint panel buttons
        //LET'S GOOOOOOOOOOOOOOO
        #endregion
        #region Closing
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
        public static void CloseForm()
        {
            close.Invoke();
        }
        #endregion
        #region Symbol Counter
        private void chatTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string userInput = chatTextBox.Text;
            char charCount;
            if (userInput != "")
            {
                charCount = userInput[0];
            }
            
            CharCountTextBlock.Text = "Символов " + userInput.Length.ToString() + "/200";
        }
        #endregion
        #region Chat
        private void SendButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (chatTextBox.Text != "")
            {
                chatCount += 1;
                ChatsScrollView.ScrollToBottom();
                ChatBox.AppendText($"Вы: {chatTextBox.Text}\n\n");
                SocketConnector.SendMessage(chatTextBox.Text);
                chatTextBox.Text = "";
            }
            //ChatTextBlock.Text = chatTextBox.Text;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.sendButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }
        #endregion
        #region Trash
        private void Ez_Click(object sender, RoutedEventArgs e)
        {
        }
        private void RefreshRoomsList(object sender, RoutedEventArgs e)
        {
            RefreshRooms();
        }
        #endregion
    }
}

//ToDo
//1)доп контекстное меню "удалить комнату" и "покинуть комнату"
//2)Вернуть выход из конференции без её полного удаления (leave = !!!POST запрос!!!)
//3)Настройки -> мой аккаунт = добавить реальные данные