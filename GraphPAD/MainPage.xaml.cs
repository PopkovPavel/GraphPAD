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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;

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
        public bool isFreeModeOn;
        public float paintsize;
        private bool _flag; //Флаг для логики микрофона (Проверка на то, был ли выключен микрофон до выключения звука)
        public int lobbyCount;
        public int lobbyButtonsMargin = -70;
        public int chatCount;
        public int chatTextblockMargin;

        Brush _customBrush;
        Random _rand = new Random();
        delegate void Function(object sender, MouseButtonEventArgs e);
        Function function;
        private int _inc = 1;
        private bool ispaint;

        public SolidColorBrush greenbrush = new SolidColorBrush(Color.FromRgb(19, 199, 19));

        public delegate void Method();
        private static Method close;
        #endregion

        public MainPage()
        {
            InitializeComponent();

            close = new Method(Close);

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
            //hellob
            function = null;
            isAddVetexOn = false;
            isRemoveVertexOn = false;
            isFreeModeOn = false;
            ispaint = false;
            paintsize = 20;
            FreeModeCanvas.Visibility = Visibility.Hidden;

            Camera3.Visibility = Visibility.Hidden;

            conferenssionString.Text = "Конференция № ...";
            if (GuestInfo.Name == "test")
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
            ControlCanvas.Visibility = Visibility.Hidden;
            infoTextBlock.Visibility = Visibility.Visible;
            leaveButton.Visibility = Visibility.Hidden;
            CancelLobbyButton.Visibility = Visibility.Hidden;
            ConferensionIDTextBox.Visibility = Visibility.Hidden;
            ConnectToLobbyButton.Visibility = Visibility.Hidden;
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
            };
            RefreshRooms();

        }

        #region ServerFunctions
        public void RefreshRooms()
        {
            try
            {
                lobbyCount = 0;
                lobbyButtonsMargin = -40;
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
                            MessageBox.Show("ID скопирован");
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
                            ToolTip = "Покинуть коференцию"
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
                            ToolTip = "Конференция №" + room.RoomID.Substring(0,8),
                            ContextMenu = contextMenu
                        };

                        tempButton.Click += Ez_Click;

                        menuLeaveItem.Click += (s, ea) => 
                        {
                            LeaveRoom($"{room.RoomID}");
                        };

                        tempButton.Click += (senda, ev) =>
                        {
                            OpenRoomAsync($"{room.RoomID}");
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
                    MessageBox.Show("возможно вы даун", "Ошибка");
                }

                //создание конференции с уникальным ID

            }
            catch
            {
                MessageBox.Show("Что-то пошло не так. Сообщите об этом администратору ribalko2006@mail.ru");

            }
        }
        public void LeaveRoom(string roomId)
        {
            try
            {
                var client = new RestClient($"https://testingwebrtc.herokuapp.com/room/{roomId}/leave");
                client.Timeout = -1;
                var request = new RestRequest(RestSharp.Method.POST);
                request.AddHeader("x-access-token", UserInfo.Token);
                IRestResponse response = client.Execute(request);
                if (response.IsSuccessful)
                {
                    MessageBox.Show($"Вы покинули конференцию {roomId}");
                    RefreshRooms();
                }
                else
                {
                    MessageBox.Show("Скорее всего эта ваша конференция\n(если нет, то со всеми бывает)", "да.");
                }

                //создание конференции с уникальным ID

            }
            catch
            {
                MessageBox.Show("Что-то пошло не так. Сообщите об этом администратору ribalko2006@mail.ru");

            }
        }
        public async System.Threading.Tasks.Task OpenRoomAsync(string roomId)
        {
            Chromium.SetSettings(roomId);
            var jepa = Chromium.Connect();
            jepa.Height = 720;
            jepa.Width = 405;
            
            VideoChatCanvas.Children.Add(jepa);
            chatGrid.Visibility = Visibility.Visible;
            GraphCanvas.Visibility = Visibility.Visible;
            ControlCanvas.Visibility = Visibility.Visible;
            leaveButton.Visibility = Visibility.Visible;
            LobbysCanvas.Visibility = Visibility.Hidden;
            conferenssionString.Text = $"Конференция №{roomId.Substring(0, 8)}";
            ConferensionString.Text = $"Чат конференции №{roomId.Substring(0, 8)}";
            ///
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
        private void CreateLobby_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(this, "Создать новую конференцию?", "Подтверждение", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
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
                    IRestResponse response = client.Execute(request);
                    if (response.IsSuccessful)
                    {                        
                        JSONroom room = JsonConvert.DeserializeObject<JSONroom>(response.Content.ToString());
                        var newRoomID = room.Data.RoomID;
                        MessageBox.Show($"Вы успешно создали конференцию с ID:\n {newRoomID}");
                        RefreshRooms();
                    }
                    else
                    {
                        MessageBox.Show("возможно вы даун", "Ошибка");
                    }

                    //создание конференции с уникальным ID

                }
                catch
                {
                    MessageBox.Show("Что-то пошло не так. Сообщите об этом администратору ribalko2006@mail.ru");

                }
            }
        }
        private void ConnectToLobby_Click(object sender, RoutedEventArgs e)
        {
            string _conferensionID = ConferensionIDTextBox.Text.Trim().ToLower(); //Trim() - Удаление лишних символов
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
                        MessageBox.Show("Вы успешно вошли в конференцию с ID:\n" + _conferensionID);
                        ConferensionIDTextBox.ToolTip = _conferensionID.ToString();
                        ConferensionIDTextBox.BorderBrush = Brushes.Gray;
                        GraphCanvas.Visibility = Visibility.Visible;
                        ControlCanvas.Visibility = Visibility.Visible;
                        conferenssionString.Text = "Конференция №" + _conferensionID;
                        leaveButton.Visibility = Visibility.Visible;
                        LobbysCanvas.Visibility = Visibility.Hidden;
                        RefreshRooms();
                    } 
                    else 
                    {
                        MessageBox.Show("Возможно такой конференции не существует, либо вы уже состоите в ней","Ошибка");
                    }

                    
                }
                catch
                {
                    MessageBox.Show("Что-то пошло не так. Сообщите об этом администратору ribalko2006@mail.ru");
                    
                }
                
            }
        }
        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            File.Delete(@"Token.json");
            AuthPage authPage = new AuthPage();
            this.Visibility = Visibility.Hidden; //Скрывает текущее окно
            authPage.Show();
        }
        #endregion

        #region CringeButtons
        private async System.Threading.Tasks.Task LobbyLeave_ClickAsync(object sender, RoutedEventArgs e)
        {          
            await SocketConnector.Disconnect();
            chatTextBox.IsReadOnly = (SocketConnector.IsConnected) ? false : true;
            chatGrid.Visibility = Visibility.Hidden;
            ConferensionIDTextBox.BorderBrush = Brushes.Gray;
            GraphCanvas.Visibility = Visibility.Hidden;
            FreeModeCanvas.Visibility = Visibility.Hidden;
            ControlCanvas.Visibility = Visibility.Hidden;
            conferenssionString.Text = "Конференция № ...";
            leaveButton.Visibility = Visibility.Hidden;

            CreateLobbyButton.Visibility = Visibility.Visible;
            ConferensionIDTextBox.Visibility = Visibility.Hidden;
            CancelLobbyButton.Visibility = Visibility.Hidden;
            ConnectToLobbyButton.Visibility = Visibility.Hidden;
            LobbysCanvas.Visibility = Visibility.Visible;
        }
        private void Ez_Click(object sender, RoutedEventArgs e)
        {           
        }
        private void CancelLobby_Click(object sender, RoutedEventArgs e)
        {
            CreateLobbyButton.Visibility = Visibility.Visible;
            ConferensionIDTextBox.Visibility = Visibility.Hidden;
            CancelLobbyButton.Visibility = Visibility.Hidden;
            ConnectToLobbyButton.Visibility = Visibility.Hidden;
        }
        private void EnterLobby_Click(object sender, RoutedEventArgs e)
        {
            if (UserInfo.Email != null)
            {
                CreateLobbyButton.Visibility = Visibility.Hidden;
                ConferensionIDTextBox.Visibility = Visibility.Visible;
                CancelLobbyButton.Visibility = Visibility.Visible;
                ConnectToLobbyButton.Visibility = Visibility.Visible;
            }
        }
        private void TextChatButton_Clicked(object sender, RoutedEventArgs e)
        {
            TextChatCanvas.Visibility = Visibility.Visible;
            VideoChatCanvas.Visibility = Visibility.Hidden;
            chatTextBox.Visibility = Visibility.Visible;
            sendButton.Visibility = Visibility.Visible;
            CharCountTextBlock.Visibility = Visibility.Visible;
            ChatsScrollView.Visibility = Visibility.Visible;

        }
        private void VideoChatButton_Clicked(object sender, RoutedEventArgs e)
        {
            TextChatCanvas.Visibility = Visibility.Hidden;
            VideoChatCanvas.Visibility = Visibility.Visible;
            ChatsScrollView.Visibility = Visibility.Hidden;
            chatTextBox.Visibility = Visibility.Hidden;
            sendButton.Visibility = Visibility.Hidden;
            CharCountTextBlock.Visibility = Visibility.Hidden;

        }
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
        private void VideoButton_Clicked(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            btn.Background = btn.Background == Brushes.DarkGreen ? (SolidColorBrush)(new BrushConverter().ConvertFrom("#00000000")) : Brushes.DarkGreen;
            
            if (isVideoOn)
            {
                videoTextBlock.Text = "Видео отключено";
                videoTextBlock.Foreground = Brushes.DarkGray;
                videoButton.ToolTip = "Вкл. камеру";
                isVideoOn = false;
                Camera3.Visibility = Visibility.Hidden;
            }
            else
            {
                Camera3.Visibility = Visibility.Visible;
                videoTextBlock.Text = "Видео подключено";
                videoTextBlock.Foreground = greenbrush;
                videoButton.ToolTip = "Выкл. камеру";
                isVideoOn = true;
            }
        }
        private void SettingsButton_Clicked(object sender, RoutedEventArgs e)
        {
            SettingsPage settingsPage = new SettingsPage();
            settingsPage.ShowDialog(); //ShowDialog открывает окно поверх, блокируя основное
        }
        #endregion

        #region FreeMode
        private void Paint(Brush brush, Point point)
        {
            Ellipse ellipse = new Ellipse();
            ellipse.Fill = brush;
            ellipse.Width = paintsize;
            ellipse.Height = paintsize;
            Canvas.SetLeft(ellipse, point.X - ellipse.Width / 2);
            Canvas.SetTop(ellipse, point.Y - ellipse.Height / 2);
            FreeModeCanvas.Children.Add(ellipse);
        }
        private void FreeModeMove(object sender, MouseEventArgs e)
        {
            if (ispaint)
            {
                Point point = e.GetPosition(FreeModeCanvas);
                Paint(Brushes.Black, point);
            }
        }
        private void FreeMode_Click(object sender, RoutedEventArgs e)
        {
            if (!isFreeModeOn)
            {
                function = null; //free mode
                addVertexBtn.IsEnabled = false; //canvas is better :/
                deleteVertexBtn.IsEnabled = false;
                isFreeModeOn = true;
                GraphCanvas.Visibility = Visibility.Hidden;
                FreeModeCanvas.Visibility = Visibility.Visible;
                var path = "Resources/graph_mode.png";
                Uri resourceUri = new Uri(path, UriKind.Relative);
                StreamResourceInfo streamInfo = Application.GetResourceStream(resourceUri);
                BitmapFrame temp = BitmapFrame.Create(streamInfo.Stream);
                freeModeImage.Source = temp;
                freeModeTextBlock.Text = "Режим графов";
                freeModeBtn.ToolTip = "Включить режим графов";
                
            }
            else
            {
                function = null;
                addVertexBtn.IsEnabled = true;
                deleteVertexBtn.IsEnabled = true;
                isFreeModeOn = false;
                GraphCanvas.Visibility = Visibility.Visible;
                FreeModeCanvas.Visibility = Visibility.Hidden;
                var path = "Resources/free_mode.png";
                Uri resourceUri = new Uri(path, UriKind.Relative);
                StreamResourceInfo streamInfo = Application.GetResourceStream(resourceUri);
                BitmapFrame temp = BitmapFrame.Create(streamInfo.Stream);
                freeModeImage.Source = temp;
                freeModeTextBlock.Text = "Свободный режим";
                freeModeBtn.ToolTip = "Включить свободный режим";
            }
            Button btn = sender as Button;
            btn.Background = btn.Background == Brushes.DarkGray ? (SolidColorBrush)(new BrushConverter().ConvertFrom("#00000000")) : Brushes.DarkGray;
        }
        private void FreeModeCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ispaint = false;
        }
        private void FreeModeCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ispaint = true;
            Point point = e.GetPosition(FreeModeCanvas);
            Paint(Brushes.Black, point);
        }
        private void ClearFreeCanvas(object sender, RoutedEventArgs e)
        {
            int count = FreeModeCanvas.Children.Count;
            if (count > 0)
            {
                FreeModeCanvas.Children.Clear();
            }
        }
        #endregion

        #region Vertexes
        public Canvas MakeVertex()
        {
            var temp = 40;
            Grid myGrid = new Grid
            {
                Width = temp,
                Height = temp,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            ColumnDefinition colDef1 = new ColumnDefinition();
            RowDefinition rowDef1 = new RowDefinition();
            myGrid.ColumnDefinitions.Add(colDef1);
            myGrid.RowDefinitions.Add(rowDef1);
            Ellipse ellipse1 = new Ellipse
            {
                Width = temp,
                Height = temp,
                StrokeThickness = 3,
                Fill = _customBrush,
                Stroke = Brushes.Black
            };
            TextBlock txt1 = new TextBlock
            {
                Text = _inc.ToString(),
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            _inc++;
            Grid.SetColumnSpan(ellipse1, 1);
            Grid.SetColumnSpan(txt1, 1);
            Grid.SetRow(ellipse1, 0);
            Grid.SetRow(txt1, 0);
            myGrid.Children.Add(ellipse1);
            myGrid.Children.Add(txt1);
            Viewbox newViewbox = new Viewbox
            {
                Height = temp,
                Width = temp,
                Child = myGrid
            };
            Canvas newCanvas = new Canvas
            {
                Height = temp,
                Width = temp,
            };
            Canvas.SetLeft(newCanvas, temp / 2);
            Canvas.SetTop(newCanvas, temp / 2);
            newCanvas.Children.Add(newViewbox);
            return newCanvas;
        }
        public void AddVertex(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Ellipse || e.OriginalSource is TextBlock)
            {
                try
                {
                    Ellipse activeRec = (Ellipse)e.OriginalSource; // create the link between the sender rectangle
                    activeRec.Fill = Brushes.Red;

                }
                catch
                {
                }
                try
                {
                    TextBlock activeRec = (TextBlock)e.OriginalSource; // create the link between the sender rectangle
                    var temp = ((Ellipse)((Grid)activeRec.Parent).Children[0]);
                    temp.Fill = Brushes.Red;
                }
                catch
                {

                }
            }
            else
            {
                _customBrush = new SolidColorBrush(Color.FromRgb((byte)_rand.Next(100, 255),
                    (byte)_rand.Next(100, 255), (byte)_rand.Next(100, 255)));

                var vertex = MakeVertex();
                var temp = 20;
                var tempX = -20;
                var tempY = -20;
                if (Mouse.GetPosition(GraphCanvas).X + temp >= GraphCanvas.Width) { tempX += -temp; }
                if (Mouse.GetPosition(GraphCanvas).Y + temp >= GraphCanvas.Height) { tempY += -temp; }
                if (Mouse.GetPosition(GraphCanvas).X - temp <= 0) { tempX += temp; }
                if (Mouse.GetPosition(GraphCanvas).Y - temp <= 0) { tempY += temp; }
                Canvas.SetLeft(vertex, Mouse.GetPosition(GraphCanvas).X + tempX); // set the left position of rectangle to mouse X
                Canvas.SetTop(vertex, Mouse.GetPosition(GraphCanvas).Y + tempY); // set the top position of rectangle to mouse Y

                GraphCanvas.Children.Add(vertex); // add the new rectangle to the canvas
            }
        }
        public void DeleteVertex(object sender, MouseButtonEventArgs e)
        {
            if ((e.OriginalSource is TextBlock) || (e.OriginalSource is Ellipse))
            {
                try
                {
                    TextBlock activeRec = (TextBlock)e.OriginalSource;
                    var temp = (Viewbox)(((Grid)activeRec.Parent).Parent);
                    ((Canvas)temp.Parent).Children.Remove(temp);
                }
                catch
                {

                }
                try
                {
                    Ellipse activeRec = (Ellipse)e.OriginalSource;
                    var temp = (Viewbox)(((Grid)activeRec.Parent).Parent);
                    ((Canvas)temp.Parent).Children.Remove(temp);
                }
                catch
                {

                }
            }
        }
        //-------------------------------------------------------------------------------------------
        private void AddVertex_Click(object sender, RoutedEventArgs e)
        {
            if (!isAddVetexOn)
            {
                function = AddVertex;
                //addVertexBtn.IsEnabled = false;
                deleteVertexBtn.IsEnabled = false;
                freeModeBtn.IsEnabled = false;
                isAddVetexOn = true;
                addVertexBtn.ToolTip = "Выключить режим добавления вершин";
            }
            else
            {
                function = null;
                deleteVertexBtn.IsEnabled = true;
                freeModeBtn.IsEnabled = true;
                isAddVetexOn = false;
                addVertexBtn.ToolTip = "Включить режим добавления вершин"; //Включить режим удаления вершин
            }
            Button btn = sender as Button;
            btn.Background = btn.Background == Brushes.DarkGreen ? (SolidColorBrush)(new BrushConverter().ConvertFrom("#00000000")) : Brushes.DarkGreen;
        }
        private void workWithCanvasLeftDown(object sender, MouseButtonEventArgs e)
        {
            function?.Invoke(sender, e);
        }
        private void RemoveVertex_Click(object sender, RoutedEventArgs e)
        {
            if (!isRemoveVertexOn)
            {
                function = DeleteVertex;
                addVertexBtn.IsEnabled = false;
                freeModeBtn.IsEnabled = false;
                isRemoveVertexOn = true;
                deleteVertexBtn.ToolTip = "Выключить режим удаления вершин";
            }
            else
            {
                function = null;
                addVertexBtn.IsEnabled = true;
                freeModeBtn.IsEnabled = true;
                isRemoveVertexOn = false;
                deleteVertexBtn.ToolTip = "Включить режим удаления вершин";
            }
            Button btn = sender as Button;
            btn.Background = btn.Background == Brushes.DarkRed ? (SolidColorBrush)(new BrushConverter().ConvertFrom("#00000000")) : Brushes.DarkRed;
        }
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
        private void ChangeImage(string path, Button btn) //Функция для смены изображений в кнопке
        {
            Uri resourceUri = new Uri(path, UriKind.Relative);
            StreamResourceInfo streamInfo = Application.GetResourceStream(resourceUri);
            BitmapFrame temp = BitmapFrame.Create(streamInfo.Stream);
            var brush = new ImageBrush();
            brush.ImageSource = temp;
            btn.Background = brush;
        }

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


        private void SendButton_Clicked(object sender, RoutedEventArgs e)
        {
            chatCount += 1;
            ChatsScrollView.ScrollToBottom();
            ChatBox.AppendText($"Вы: {chatTextBox.Text}\n\n");
            SocketConnector.SendMessage(chatTextBox.Text);
            chatTextBox.Text = "";

            //ChatTextBlock.Text = chatTextBox.Text;
        }
    }
}
