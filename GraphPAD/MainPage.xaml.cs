using CefSharp;
using CefSharp.Wpf;
using GraphPAD.Data.JSON;
using GraphPAD.Data.User;
using GraphPAD.GraphData.Model;
using GraphPAD.GraphData.Pattern;
using GraphX.Controls;
using GraphX.Controls.Models;
using GraphX.PCL.Common;
using GraphX.PCL.Common.Enums;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Resources;

namespace GraphPAD
{
    public partial class MainPage : Window
    {
        #region Global Variables
        private string algorithmResult;
        /// <summary>
        /// Список ребер, которые необходимо "покрасить"
        /// </summary>
        private List<DataEdge> algorithmEdgesList = new List<DataEdge>();
        CancellationToken source = new CancellationToken();
        /// <summary>
        /// Фабрика
        /// </summary>
        private Painter _creator;
        bool flagNegr = true;
        /// <summary>
        /// Строитель
        /// </summary>
        private Director _director;

        private EditorOperationMode _opMode = EditorOperationMode.Select;
        private VertexControl _ecFrom;
        private readonly EditorObjectManager _editorManager;
        public enum EditorOperationMode
        {
            Select = 0,
            Edit,
            Delete,
            Algorithm
        }

        /// <summary>
        /// Для управления командами
        /// </summary>
        private readonly List<Command> _commands = new List<Command>();

        private Command _currentCommand;
        private int _commandCounter = -1;
        //User Controls
        public bool isMicOn;
        public bool isHeadPhonesOn;
        public bool isVideoOn;
        private bool _flag;
        //Graph Controls
        public bool isAddVetexOn;
        public bool isRemoveVertexOn;
        public bool isConnectVertexOn;
        public bool isDisconnectVertexOn;
        public bool isGraphGeneratorOn;
        public bool isAlgorithmsOn;
        //Graph Controls (Generation)
        public bool isRandomTreeOn;
        public bool isRandomConnectedGraphOn;
        //Paint Controls
        public bool isFreeModeOn;
        public bool isBrushModeOn;
        public bool isEraserModeOn;
        public bool isEraser_SmartModeOn;
        public bool isSelectionModeOn;
        public string desktopPath;
        //Etc.
        public int lobbyCount;
        public int lobbyButtonsMargin = -70;
        public int chatCount;
        public int chatTextblockMargin;
        /// <summary>
        /// Имя конференции
        /// </summary>
        private string ConferensionName;
        public delegate void Method();
        private static Method close;
        //Путь к папке аватара
        string avatarsFolder = Path.GetFullPath(@"Avatars\Avatar.png");
        //Доп. Кастомные Цвета
        public SolidColorBrush greenbrush = new SolidColorBrush(Color.FromRgb(19, 199, 19));
        public SolidColorBrush lightpurple = new SolidColorBrush(Color.FromRgb(85, 85, 147));
        #endregion
        #region Initialize
        public MainPage()
        {
            InitializeComponent();
            #region grapharea + zoom ctrl
            // ZoomCtrl.Visibility = Visibility.Visible;
            //Элементы на поле отрисовки графа
            var dgLogic = new GraphLogic();

            GraphArea.LogicCore = dgLogic;
            GraphArea.VertexSelected += GraphArea_VertexSelected;
            GraphArea.EdgeSelected += GraphArea_EdgeSelected;
            GraphArea.SetVerticesMathShape(VertexShape.Ellipse);

            GraphArea.VertexLabelFactory = new DefaultVertexlabelFactory();
            GraphArea.EdgeLabelFactory = new DefaultEdgelabelFactory();
            GraphArea.ShowAllEdgesLabels(true);
            GraphArea.ShowAllEdgesArrows(true);
            GraphArea.SetEdgesHighlight(true, GraphControlType.VertexAndEdge);
            GraphArea.SetVerticesHighlight(true, GraphControlType.VertexAndEdge, EdgesType.All);
            dgLogic.EdgeCurvingEnabled = true;
            //dgLogic.DefaultLayoutAlgorithm = LayoutAlgorithmTypeEnum.Custom;
            //dgLogic.DefaultOverlapRemovalAlgorithm = OverlapRemovalAlgorithmTypeEnum.None;
            //dgLogic.DefaultEdgeRoutingAlgorithm = EdgeRoutingAlgorithmTypeEnum.None;

            _editorManager = new EditorObjectManager(GraphArea, ZoomCtrl);
            Painter.GraphZone = GraphArea;
            //  DeleteCliqueCommand.Sp = SpDeletedClicks;
            _opMode = EditorOperationMode.Select;

            GraphArea.SetVerticesDrag(true, true);
            ClearEditMode();

            ZoomControl.SetViewFinderVisibility(ZoomCtrl, Visibility.Visible);
            ZoomCtrl.IsAnimationEnabled = true;
            ZoomCtrl.MouseDown += ZoomCtrl_MouseDown;
            ZoomCtrl.MouseUp += ZoomCtrl_MouseUp;
            ZoomCtrl.Cursor = Cursors.Hand;

            ZoomControl.SetViewFinderVisibility(PaintCanvasScroll, Visibility.Visible);
            // PaintCanvasScroll.IsAnimationEnabled = true;
            //  PaintCanvasScroll.MouseDown += ZoomCtrl_MouseDown;
            PaintCanvas.EditingMode = InkCanvasEditingMode.Ink;


            #endregion
            //Создание папки "Avatars", если она не существует
            if (!Directory.Exists(Path.GetFullPath(@"Avatars")))
            {
                Directory.CreateDirectory(Path.GetFullPath(@"Avatars"));
                //MessageBox.Show(Path.GetFullPath("Avatars"));
            }
            //Создание файла "Avatar.png" в папке "Avatars", если его не существует
            if (!File.Exists(Path.GetFullPath(@"Avatars/Avatar.png")))
            {
                using (var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("GraphPAD.Resources.avatar_default.png"))
                {
                    using (var file = new FileStream(Path.GetFullPath(@"Avatars/Avatar.png"), FileMode.Create, FileAccess.Write))
                    {
                        resource.CopyTo(file);
                    }
                }
            }
            UserInfo.Avatar = avatarsFolder;
            Avatar.Source = NonBlockingLoad(UserInfo.Avatar);
            close = new Method(Close);
            this.KeyDown += new KeyEventHandler(MainWindow_KeyDown); //Отлов клавиши Enter для отправки сообщений
            Closing += OnClosing;  //Делегат для отлова закрытия окна
            isMicOn = true;
            isHeadPhonesOn = true;
            isVideoOn = false;
            _flag = false; //Флаг для логики микрофона (Проверка на то, был ли выключен микрофон до выключения звука)
            lobbyCount = 0;
            chatCount = 0;
            lobbyButtonsMargin = 10;
            chatTextblockMargin = 5;
            voiceChatTextBlock.Text = "Голосовой чат подключен";
            videoTextBlock.Text = "Видео отключено";
            videoTextBlock.Foreground = Brushes.DarkGray;
            desktopPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\image.jpg";
            //Выключение всех режимов работы с графами
            isAddVetexOn = false;
            isRemoveVertexOn = false;
            isConnectVertexOn = false;
            isDisconnectVertexOn = false;
            isGraphGeneratorOn = false;
            isAlgorithmsOn = false;
            isRandomTreeOn = false;
            isRandomConnectedGraphOn = false;

            //Выключение всех режимов работы с рисовалкой
            isBrushModeOn = false;
            isEraserModeOn = false;
            isEraser_SmartModeOn = false;
            isSelectionModeOn = false;

            isFreeModeOn = false;
            //Отключение Кисти при запуске
            PaintCanvas.EditingMode = InkCanvasEditingMode.None;
            if (UserInfo.Name != null)
            {
                nameString.Text = UserInfo.Name;
                userRoleString.Text = "Пользователь";
            }
            else
            {
                nameString.Text = GuestInfo.Name;
                userRoleString.Text = "Гость";
            }
            conferenssionString.Text = "Конференция ...";

            //debug
            ZoomCtrl.Visibility = Visibility.Hidden;
            PaintCanvasScroll.Visibility = Visibility.Hidden;
            GraphControlCanvas.Visibility = Visibility.Hidden;
            GraphModeChangerButton.Visibility = Visibility.Hidden;
            PaintControlCanvas.Visibility = Visibility.Hidden;
            PaintModeChangerButton.Visibility = Visibility.Hidden;
            infoTextBlock.Visibility = Visibility.Visible;
            leaveButton.Visibility = Visibility.Hidden;
            chatGrid.Visibility = Visibility.Hidden;
            EraserTextBlock.Visibility = Visibility.Hidden;
            EraserSlider.Visibility = Visibility.Hidden;
            edgesWeightTextBox.Visibility = Visibility.Hidden;
            orientedCheckbox.Visibility = Visibility.Hidden;

            Chromium.settings.CefCommandLineArgs.Add("enable-media-stream", "1");
            Cef.Initialize(Chromium.settings);
            leaveButton.Click += (s, ea) =>
            {
                LobbyLeave_ClickAsync(s, ea);
                ChatBox.Clear();
                chatTextBox.Clear();
                foreach (UIElement temp in VideoChatCanvas.Children)
                {
                    try
                    {
                        ((ChromiumWebBrowser)temp).Dispose();
                    }
                    catch { }

                }
            };
            RefreshRooms();
            ButtonsFix();
        }


        #endregion
        #region GraphArea Functions
        private void ZoomCtrl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ClearEditMode();
        }
        private void ZoomCtrl_MouseDown(object sender, MouseButtonEventArgs e)
        {

            //create vertices and edges only in Edit mode
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (_opMode == EditorOperationMode.Edit)
                {
                    var pos = ZoomCtrl.TranslatePoint(e.GetPosition(ZoomCtrl), GraphArea);
                    pos.Offset(-22.5, -22.5);
                    var vc = CreateVertexControl(pos);
                    if (_ecFrom != null)
                        CreateEdgeControl(vc);
                }
                else if (_opMode == EditorOperationMode.Select)
                {
                    ClearSelectMode(true);
                } 
            }
        }
        /// <summary>
        /// Выбор вершины
        /// </summary>
        /// <param name="vc"></param>
        private static void SelectVertex(DependencyObject vc)
        {
            if (DragBehaviour.GetIsTagged(vc))
            {
                HighlightBehaviour.SetHighlighted(vc, false);
                DragBehaviour.SetIsTagged(vc, false);
            }
            else
            {
                HighlightBehaviour.SetHighlighted(vc, true);
                DragBehaviour.SetIsTagged(vc, true);
            }
        }
        /// <summary>
        /// Сбросить режим перемещения вершин
        /// </summary>
        /// <param name="soft"></param>
        private void ClearSelectMode(bool soft = false)
        {
            GraphArea.VertexList.Values
                .Where(DragBehaviour.GetIsTagged)
                .ToList()
                .ForEach(a =>
                {
                    HighlightBehaviour.SetHighlighted(a, false);
                    DragBehaviour.SetIsTagged(a, false);
                });

            if (!soft)
                GraphArea.SetVerticesDrag(false);
        }
        private void ClearEditMode()
        {
            if (_ecFrom != null) HighlightBehaviour.SetHighlighted(_ecFrom, false);
            _editorManager.DestroyVirtualEdge();
            _ecFrom = null;
        }
        private VertexControl CreateVertexControl(Point position)
        {
            Random rnd = new Random();
            byte c1 = (byte)rnd.Next(0, 160);
            byte c2 = (byte)rnd.Next(0, 160);
            byte c3 = (byte)rnd.Next(0, 160);

            var vertexColor = new SolidColorBrush(Color.FromRgb(c1, c2, c3));
            var data = new DataVertex((GraphArea.VertexList.Count + 1).ToString(), vertexColor) { };
            var vc = new VertexControl(data);
            vc.SetPosition(position);
            GraphArea.AddVertexAndData(data, vc, true);
            return vc;
        }
        private void CreateEdgeControl(VertexControl vc)
        {
            if (_ecFrom == null)
            {
                _editorManager.CreateVirtualEdge(vc, vc.GetPosition());
                _ecFrom = vc;
                HighlightBehaviour.SetHighlighted(_ecFrom, true);
                return;
            }
             
            if (_ecFrom == vc) return;
            var weightText = edgesWeightTextBox.Text != "";
            int weight = weightText ? int.Parse(edgesWeightTextBox.Text) : 1;
            Brush color;
            List<DataEdge> edgesToDelete = new List<DataEdge>();
            foreach(var edge in GraphArea.EdgesList)
            {
                if (edge.Key.Source == vc.Vertex && _ecFrom.Vertex == edge.Key.Target
                    || edge.Key.Target == vc.Vertex && _ecFrom.Vertex == edge.Key.Source)
                {
                    edgesToDelete.Add(edge.Key);
                }
            }

            foreach(var temp in edgesToDelete)
            {
                GraphArea.RemoveEdge(temp, true);
            }               

            if (orientedCheckbox.IsChecked == false)
            {
                color = Brushes.Transparent;
            }
            else
            {
                color = Brushes.Black;
                
            }
            var data = new DataEdge((DataVertex)_ecFrom.Vertex, (DataVertex)vc.Vertex, weight, color);
            var ec = new EdgeControl(_ecFrom, vc, data);
            if (orientedCheckbox.IsChecked == false)
            {
                var data2 = new DataEdge((DataVertex)vc.Vertex, (DataVertex)_ecFrom.Vertex, weight, Brushes.Transparent);
                var ec2 = new EdgeControl(vc, _ecFrom, data2);
                if (weight == 1)
                {
                    GraphArea.InsertEdgeAndData(data2, ec2, 0, false);
                }
                else
                {
                    GraphArea.InsertEdgeAndData(data2, ec2, 0, true);
                }
            }
            if (weight == 1)
            {
                GraphArea.InsertEdgeAndData(data, ec, 0, false);
            }
            else
            {
                GraphArea.InsertEdgeAndData(data, ec, 0, true);
            }

            HighlightBehaviour.SetHighlighted(_ecFrom, false);
            _ecFrom = null;
            _editorManager.DestroyVirtualEdge();
        }
        private void SafeRemoveVertex(VertexControl vc)
        {
            //remove vertex and all adjacent edges from layout and data graph
            GraphArea.RemoveVertexAndEdges(vc.Vertex as DataVertex);
        }
        void GraphArea_VertexSelected(object sender, VertexSelectedEventArgs args)
        {
            if (args.MouseArgs.LeftButton == MouseButtonState.Pressed)
            {
                switch (_opMode)
                {
                    case EditorOperationMode.Edit:
                        CreateEdgeControl(args.VertexControl);
                        break;
                    case EditorOperationMode.Delete:
                        SafeRemoveVertex(args.VertexControl);
                        break;
                    case EditorOperationMode.Algorithm:
                        StartAlgorithm(args.VertexControl);
                        break;
                    default:
                        if (_opMode == EditorOperationMode.Select && args.Modifiers == ModifierKeys.Control)
                            SelectVertex(args.VertexControl);
                        break;
                }
            }            
        }

        private void StartAlgorithm(VertexControl vc)
        {
            FixLabelsAndArrows();
            if (isAlgorithmsOn)
            {
                string ChoosedAlgorithm = "test";
                switch (ChoosedAlgorithm)
                {
                    case "test":
                        CalculateDFS((DataVertex)vc.Vertex);
                        break;
                    default: break;
                }
            }
        }

        void GraphArea_EdgeSelected(object sender, EdgeSelectedEventArgs args)
        {
            if (args.MouseArgs.LeftButton == MouseButtonState.Pressed && _opMode == EditorOperationMode.Delete)
            {
                List<DataEdge> edgesToDelete = new List<DataEdge>();
                foreach (var edge in GraphArea.EdgesList)
                {
                    if (edge.Key.Source == (args.EdgeControl.Edge as DataEdge).Source 
                        && (args.EdgeControl.Edge as DataEdge).Target == edge.Key.Target
                        || edge.Key.Target == (args.EdgeControl.Edge as DataEdge).Source
                        && (args.EdgeControl.Edge as DataEdge).Target == edge.Key.Source)
                    {
                        edgesToDelete.Add(edge.Key);
                    }
                }
                foreach (var temp in edgesToDelete)
                {
                    GraphArea.RemoveEdge(temp, true);
                }

            }
        }

        #endregion
        #region Functions
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
        public void ButtonsFix()
        {
            //GraphButtons fix
            isAddVetexOn = false;
            isRemoveVertexOn = false;
            isConnectVertexOn = false;
            isDisconnectVertexOn = false;
            isGraphGeneratorOn = false;
            isAlgorithmsOn = false;
            isFreeModeOn = false;
            isRandomTreeOn = false;
            isRandomConnectedGraphOn = false;
            addVertexBtn.Background = Brushes.Transparent;
            deleteVertexBtn.Background = Brushes.Transparent;
            edgesWeightTextBox.Background = Brushes.Transparent;

            graphGeneratorBtn.Background = Brushes.Transparent;
            algorithmsBtn.Background = Brushes.Transparent;
            randomTreeButton.Background = Brushes.Transparent;
            randomConnectedGraphButton.Background = Brushes.Transparent;
            addVertexBtn.IsEnabled = true;
            deleteVertexBtn.IsEnabled = true;
            edgesWeightTextBox.IsEnabled = true;
            orientedCheckbox.IsEnabled = true;
            graphGeneratorBtn.IsEnabled = true;
            algorithmsBtn.IsEnabled = true;
            reorderGraph.IsEnabled = true;
            clearGraph.IsEnabled = true;
            randomTreeButton.IsEnabled = true;
            randomConnectedGraphButton.IsEnabled = true;
            addVertexBtn.Visibility = Visibility.Visible;
            deleteVertexBtn.Visibility = Visibility.Visible;
            clearGraph.Visibility = Visibility.Visible;
            reorderGraph.Visibility = Visibility.Visible;
            edgesWeightTextBox.Visibility = Visibility.Hidden;
            orientedCheckbox.Visibility = Visibility.Hidden;
            orientedCheckbox.IsChecked = false;
            graphGeneratorBtn.Visibility = Visibility.Visible;
            algorithmsBtn.Visibility = Visibility.Visible;
            randomTreeButton.Visibility = Visibility.Hidden;
            randomConnectedGraphButton.Visibility = Visibility.Hidden;
            vertexAmountTextBox.Visibility = Visibility.Hidden;
            edgesAmountTextBox.Visibility = Visibility.Hidden;
            downloadGraphButton.Visibility = Visibility.Hidden;
            createGraphButton.Visibility = Visibility.Hidden;
            animationSpeed.Visibility = Visibility.Hidden;
            showAnimaton.Visibility = Visibility.Hidden;
            algorithmsComboBox.Visibility = Visibility.Hidden;
            currentGraphMode.Text = "Текущий режим: Перемещение";
            //PaintButtons Fix
            isBrushModeOn = false;
            isEraserModeOn = false;
            isEraser_SmartModeOn = false;
            isSelectionModeOn = false;
            BrushButton.Background = Brushes.Transparent;
            EraserButton.Background = Brushes.Transparent;
            Eraser_SmartButton.Background = Brushes.Transparent;
            SelectionButton.Background = Brushes.Transparent;
            BrushButton.IsEnabled = true;
            EraserButton.IsEnabled = true;
            Eraser_SmartButton.IsEnabled = true;
            SelectionButton.IsEnabled = true;
            ClearCanvasButton.IsEnabled = true;
            SaveToFileButton.IsEnabled = true;
            PaintCanvas.EditingMode = InkCanvasEditingMode.None;
            ColorPickerTextBlock.Visibility = Visibility.Hidden;
            ColorPicker.Visibility = Visibility.Hidden;
            EraserButton.Visibility = Visibility.Visible;
            Eraser_SmartButton.Visibility = Visibility.Visible;
            SaveToFileButton.Visibility = Visibility.Visible;
            EraserSlider.Visibility = Visibility.Hidden;
            EraserTextBlock.Visibility = Visibility.Hidden;
            BrushSlider.Visibility = Visibility.Hidden;
            BrushTextBlock.Visibility = Visibility.Hidden;
            currentPaintMode.Text = "Текущий режим: Курсор";
            //Menu Fix
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
                lobbyButtonsMargin = -60;
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
                        LobbysCanvas.Height = lobbyButtonsMargin;
                        if (lobbyCount > 8)
                        {
                            LobbysCanvas.Height = LobbysCanvas.Height + 70;
                            //LobbysScrollView.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                        }
                        ConferensionsCountTextBlock.Text = "Конференций: " + (lobbyCount);

                        //Первый элемент контекстного меню
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
                        var contextMenu = new ContextMenu()
                        {
                            Background = Brushes.Transparent
                        };
                        contextMenu.Items.Add(menuCopyItem);

                        menuCopyItem.Click += (sender1, e1) =>
                        {
                            //Копирование текста в буфер обмена
                            Clipboard.SetData(DataFormats.Text, (Object)room.RoomID);
                            MessageBox.Show("ID скопирован", "Сообщение");
                        };
                        if (room.RoomOwner.Id != UserInfo.ID)
                        {
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
                                Header = "Покинуть конференцию",
                                ToolTip = "Покинуть конференцию"
                            };
                            menuLeaveItem.Click += (s, ea) =>
                            {
                                LeaveRoom($"{room.RoomID}", $"{room.RoomName}");
                            };
                            contextMenu.Items.Add(menuLeaveItem);
                        }
                        else
                        {
                            //Третий элемент контекстного меню
                            var menuDeleteItem = new MenuItem()
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
                                Header = "Удалить конференцию",
                                ToolTip = "Удалить конференцию"
                            };
                            menuDeleteItem.Click += (s, ea) =>
                            {
                                DeleteRoom($"{room.RoomID}", $"{room.RoomName}");
                            };
                            contextMenu.Items.Add(menuDeleteItem);
                        }

                        var ConfButton = new Button()
                        {
                            Width = 64,
                            Height = 64,
                            Margin = new Thickness(15, lobbyButtonsMargin, 0, 0),
                            BorderBrush = null,
                            ToolTip = room.RoomName,
                            ContextMenu = contextMenu
                        };

                        ConfButton.Click += Ez_Click;

                        ConfButton.Click += (senda, ev) =>
                        {
                            LobbyEnter_ClickAsync($"{room.RoomID}", $"{room.RoomName}");
                            isAddVetexOn = false;
                            isRemoveVertexOn = false;
                            isConnectVertexOn = false;
                            isDisconnectVertexOn = false;
                            isGraphGeneratorOn = false;
                            isAlgorithmsOn = false;
                            isFreeModeOn = false;
                            isBrushModeOn = false;
                            isEraserModeOn = false;
                            isEraser_SmartModeOn = false;
                            isSelectionModeOn = false;
                        };

                        var path = "Resources/conferension.png";
                        Uri resourceUri = new Uri(path, UriKind.Relative);
                        StreamResourceInfo streamInfo = Application.GetResourceStream(resourceUri);
                        BitmapFrame temp = BitmapFrame.Create(streamInfo.Stream);
                        ConfButton.Background = new ImageBrush(temp);
                        LobbysCanvas.Children.Add(ConfButton);
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
                var client = new RestClient($"https://testingwebrtc.herokuapp.com/room/{roomId}/leave");
                client.Timeout = -1;
                var request = new RestRequest(RestSharp.Method.POST);
                request.AddHeader("x-access-token", UserInfo.Token);
                IRestResponse response = client.Execute(request);
                if (response.IsSuccessful)
                {
                    MessageBox.Show($"Вы покинули конференцию \"{roomName}\"", "Сообщение");
                    RefreshRooms();
                }
                else
                {
                    MessageBox.Show("Вы не можете покинуть собственную комнату", "Ошибка");
                }
            }
            catch
            {
                MessageBox.Show("Что-то пошло не так. Сообщите об этом администратору ribalko2006@mail.ru", "Ошибка");

            }
        }
        public void DeleteRoom(string roomId, string roomName)
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
                    MessageBox.Show($"Вы удалили конференцию \"{roomName}\"", "Сообщение");
                    RefreshRooms();
                }
                else
                {
                    MessageBox.Show("Вы не можете удалить чужую комнату", "Ошибка");
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
        #region Menu Enter Lobby
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
        #region Menu Create lobby
        private void CreateLobby_Click(object sender, RoutedEventArgs e)
        {
            EnterLobbyButton.IsEnabled = false;
            ConfirmCreateLobbyButton.Visibility = Visibility.Visible;
            CancelCreateLobbyButton.Visibility = Visibility.Visible;
            NewConferensionNameTextBox.Visibility = Visibility.Visible;
        }
        private void CancelCreateLobby_Click(object sender, RoutedEventArgs e)
        {
            CancelCreateLobbyButton.Visibility = Visibility.Hidden;
            ConfirmCreateLobbyButton.Visibility = Visibility.Hidden;
            NewConferensionNameTextBox.Visibility = Visibility.Hidden;
            EnterLobbyButton.IsEnabled = true;
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
                        CancelCreateLobbyButton.Visibility = Visibility.Hidden;
                        ConfirmCreateLobbyButton.Visibility = Visibility.Hidden;
                        NewConferensionNameTextBox.Visibility = Visibility.Hidden;
                        EnterLobbyButton.IsEnabled = true;
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
        #region Lobby Enter
        public async System.Threading.Tasks.Task LobbyEnter_ClickAsync(string roomId, string roomName)
        {
            //Отображение веб-камер
            Chromium.SetSettings(roomId);
            var camera = Chromium.Connect();
            camera.Height = 720;
            camera.Width = 405;

            //Список конференций в левой части окна
            LobbysCanvas.Visibility = Visibility.Hidden;
            //Нижнее поле управления
            ZoomCtrl.Visibility = Visibility.Visible;
            GraphControlCanvas.Visibility = Visibility.Visible;
            GraphModeChangerButton.Visibility = Visibility.Visible;
            PaintCanvasScroll.Visibility = Visibility.Hidden;
            PaintControlCanvas.Visibility = Visibility.Hidden;
            PaintModeChangerButton.Visibility = Visibility.Hidden;
            infoTextBlock.Visibility = Visibility.Hidden;
            //Нижнее-левое поле управления
            leaveButton.Visibility = Visibility.Visible;
            conferenssionString.Text = $"Конференция \"{roomName}\"";
            //Правая часть окна
            chatGrid.Visibility = Visibility.Visible;
            VideoChatCanvas.Children.Add(camera);
            ConferensionString.Visibility = Visibility.Visible;
            ConferensionString.Text = $"Чат конференции \"{roomName}\"";
            VideoString.Visibility = Visibility.Hidden;
            ParticipantsString.Visibility = Visibility.Hidden;
            //Главное меню
            BGgrid.Background = Brushes.DarkGray;
            menuCanvas.Visibility = Visibility.Hidden;
            //Отображение элементов чата
            chatTextBox.Visibility = Visibility.Visible;
            charCounterTextBlock.Visibility = Visibility.Visible;
            sendButton.Visibility = Visibility.Visible;
            ChatBox.Visibility = Visibility.Visible;
            ParticipantsBox.Visibility = Visibility.Hidden;
            ParticipantsString.Visibility = Visibility.Hidden;
            ParticipantsScrollView.Visibility = Visibility.Hidden;

            //Отображение участников конференции
            int participants = 0;
            var temp = GetUsers(roomId);
            ParticipantsBox.AppendText($"Владелец: {temp.Item2.Name}\n\nУчастники:\n");
            if (temp.Item1 != null)
            {
                foreach (JSONroomuser participant in temp.Item1)
                {
                    ParticipantsBox.AppendText($"#{++participants}: {participant.Name}\n");
                }
                try
                {
                    await SocketConnector.InitializeClientAsync();
                    SocketConnector.SetSettings(roomId, UserInfo.Name);
                    SocketConnector.client.On("chat-message", async response =>
                    {
                        var text = JsonConvert.DeserializeObject<JSONmessage[]>(response.ToString());
                        await Dispatcher.BeginInvoke((Action)(() => ChatBox.AppendText($"{text[0].UserId}: {text[0].Message}\n\n")));
                        chatCount += 1;
                        Console.WriteLine($"{text[0].UserId}: {text[0].Message}");
                    });
                    SocketConnector.client.On("stroke-data", async response =>
                    {

                        //StylusPointCollection
                        var stroke2 = JsonConvert.DeserializeObject<JSONstroke[]>(response.ToString());
                        var drawingAttributes = new DrawingAttributes()
                        {
                            Width = stroke2[0].Width,
                            Color = stroke2[0].Color,
                            FitToCurve = true
                        };
                        //MessageBox.Show(stroke2[0].Color.ToString() +"jopa", "");
                        //MessageBox.Show(stroke2[0].Width.ToString(), "");
                        //MessageBox.Show(stroke2[0].StrokeArray.ToString(), "");
                        var stroke = new Stroke(stroke2[0].StrokeArray, drawingAttributes);

                        await Dispatcher.BeginInvoke((Action)(() => PaintCanvas.Strokes.Add(stroke))); 
                    });
                    chatTextBox.IsReadOnly = (SocketConnector.IsConnected) ? false : true;
                }
                catch { }
            }
            ButtonsFix();
            GraphArea.ClearLayout();

        }
        #endregion
        #region Lobby Leave
        private async System.Threading.Tasks.Task LobbyLeave_ClickAsync(object sender, RoutedEventArgs e)
        {
            await SocketConnector.Disconnect();
            chatTextBox.IsReadOnly = !SocketConnector.IsConnected;
            //Изменения интерфейса до состояния в момент запуска

            //Список конференций в левой части окна
            LobbysCanvas.Visibility = Visibility.Visible;
            //Главные поля и панели для работы
            ZoomCtrl.Visibility = Visibility.Hidden;
            GraphControlCanvas.Visibility = Visibility.Hidden;
            PaintCanvasScroll.Visibility = Visibility.Hidden;
            PaintCanvas.Strokes.Clear();
            PaintControlCanvas.Visibility = Visibility.Hidden;
            infoTextBlock.Visibility = Visibility.Visible;
            //Нижнее поле управления
            GraphControlCanvas.Visibility = Visibility.Hidden;
            GraphModeChangerButton.Visibility = Visibility.Hidden;
            PaintControlCanvas.Visibility = Visibility.Hidden;
            PaintModeChangerButton.Visibility = Visibility.Hidden;
            //Нижнее-левое поле управления
            conferenssionString.Text = "Конференция ...";
            leaveButton.Visibility = Visibility.Hidden;
            //Очистить и скрыть правую часть окна
            chatGrid.Visibility = Visibility.Hidden;
            ParticipantsBox.Text = "";
            VideoChatCanvas.Children.Clear();
            ConferensionString.Visibility = Visibility.Visible;
            VideoString.Visibility = Visibility.Hidden;
            ParticipantsString.Visibility = Visibility.Hidden;
            //Главное меню
            BGgrid.Background = lightpurple;
            menuCanvas.Visibility = Visibility.Visible;
            EnterLobbyButton.IsEnabled = true;
            CancelEnterLobbyButton.Visibility = Visibility.Hidden;
            ConfirmEnterLobbyButton.Visibility = Visibility.Hidden;
            CreateLobbyButton.Visibility = Visibility.Visible;
            CancelCreateLobbyButton.Visibility = Visibility.Hidden;
            ConfirmCreateLobbyButton.Visibility = Visibility.Hidden;
            ConferensionIDTextBox.BorderBrush = Brushes.Transparent;
            ConferensionIDTextBox.Visibility = Visibility.Hidden;
            NewConferensionNameTextBox.BorderBrush = Brushes.Transparent;
            NewConferensionNameTextBox.Visibility = Visibility.Hidden;
            ButtonsFix();
        }
        #endregion
        #region Text, Video, Chat buttons
        private void TextChatButton_Clicked(object sender, RoutedEventArgs e)
        {
            //Отображение строки под кнопками
            ConferensionString.Visibility = Visibility.Visible;
            VideoString.Visibility = Visibility.Hidden;
            ParticipantsString.Visibility = Visibility.Hidden;
            //Отображение поля под строкой
            ChatBox.Visibility = Visibility.Visible;
            VideoChatCanvas.Visibility = Visibility.Hidden;
            ParticipantsBox.Visibility = Visibility.Hidden;
            //Отображение полосы прокрутки
            ChatsScrollView.Visibility = Visibility.Visible;
            VideosScrollView.Visibility = Visibility.Hidden;
            ParticipantsScrollView.Visibility = Visibility.Hidden;
            //Отображение элементов чата внизу
            chatTextBox.Visibility = Visibility.Visible;
            charCounterTextBlock.Visibility = Visibility.Visible;
            sendButton.Visibility = Visibility.Visible;
        }
        private void VideoChatButton_Clicked(object sender, RoutedEventArgs e)
        {
            //Отображение строки под кнопками
            ConferensionString.Visibility = Visibility.Hidden;
            VideoString.Visibility = Visibility.Visible;
            ParticipantsString.Visibility = Visibility.Hidden;
            //Отображение поля под строкой
            ChatBox.Visibility = Visibility.Hidden;
            VideoChatCanvas.Visibility = Visibility.Visible;
            ParticipantsBox.Visibility = Visibility.Hidden;
            //Отображение полосы прокрутки
            ChatsScrollView.Visibility = Visibility.Hidden;
            VideosScrollView.Visibility = Visibility.Visible;
            ParticipantsScrollView.Visibility = Visibility.Hidden;
            //Отображение элементов чата внизу
            chatTextBox.Visibility = Visibility.Hidden;
            charCounterTextBlock.Visibility = Visibility.Hidden;
            sendButton.Visibility = Visibility.Hidden;
        }
        private void ParticipantsButton_Clicked(object sender, RoutedEventArgs e)
        {
            //Отображение строки под кнопками
            ConferensionString.Visibility = Visibility.Hidden;
            VideoString.Visibility = Visibility.Hidden;
            ParticipantsString.Visibility = Visibility.Visible;
            //Отображение поля под строкой
            ChatBox.Visibility = Visibility.Hidden;
            VideoChatCanvas.Visibility = Visibility.Hidden;
            ParticipantsBox.Visibility = Visibility.Visible;
            //Отображение полосы прокрутки
            ChatsScrollView.Visibility = Visibility.Hidden;
            VideosScrollView.Visibility = Visibility.Hidden;
            ParticipantsScrollView.Visibility = Visibility.Visible;
            //Отображение элементов чата внизу
            chatTextBox.Visibility = Visibility.Hidden;
            charCounterTextBlock.Visibility = Visibility.Hidden;
            sendButton.Visibility = Visibility.Hidden;
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
            Avatar.Source = NonBlockingLoad(UserInfo.Avatar);
        }
        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            File.Delete(@"Token.json");
            AuthPage authPage = new AuthPage();
            this.Visibility = Visibility.Hidden; //Скрывает текущее окно
            authPage.Show();
        }
        #endregion
        #region FreeMode Changer
        private void FreeMode_Click(object sender, RoutedEventArgs e)
        {
            if (!isFreeModeOn)
            {
                isFreeModeOn = true;
                PaintCanvasScroll.ZoomToFill();
                GraphControlCanvas.Visibility = Visibility.Hidden;
                GraphModeChangerButton.Visibility = Visibility.Hidden;
                PaintControlCanvas.Visibility = Visibility.Visible;
                PaintModeChangerButton.Visibility = Visibility.Visible;
                ZoomCtrl.Visibility = Visibility.Hidden;
                PaintCanvasScroll.Visibility = Visibility.Visible;
                BGgrid.Background = Brushes.DarkSlateGray;
            }
            else
            {
                isFreeModeOn = false;
                GraphControlCanvas.Visibility = Visibility.Visible;
                GraphModeChangerButton.Visibility = Visibility.Visible;
                PaintControlCanvas.Visibility = Visibility.Hidden;
                PaintModeChangerButton.Visibility = Visibility.Hidden;
                ZoomCtrl.Visibility = Visibility.Visible;
                PaintCanvasScroll.Visibility = Visibility.Hidden;
                BGgrid.Background = Brushes.DarkGray;
                ButtonsFix();
            }
        }
        #endregion
        #region Graph panel
        private void AddVertex_Click(object sender, RoutedEventArgs e)
        {
            if (!isAddVetexOn)
            {
                //function = AddVertex;
                deleteVertexBtn.IsEnabled = false;
                graphGeneratorBtn.IsEnabled = false;
                algorithmsBtn.IsEnabled = false;
                clearGraph.Visibility = Visibility.Hidden;
                reorderGraph.Visibility = Visibility.Hidden;
                edgesWeightTextBox.Visibility = Visibility.Visible;
                orientedCheckbox.Visibility = Visibility.Visible;
                currentGraphMode.Text = "Текущий режим: Создание";
                isAddVetexOn = true;
                addVertexBtn.ToolTip = "Выключить режим добавления вершин";

                ZoomCtrl.Cursor = Cursors.Pen;
                _opMode = EditorOperationMode.Edit;
                ClearSelectMode();
            }
            else
            {
                //function = null;
                deleteVertexBtn.IsEnabled = true;
                graphGeneratorBtn.IsEnabled = true;
                algorithmsBtn.IsEnabled = true;
                clearGraph.Visibility = Visibility.Visible;
                reorderGraph.Visibility = Visibility.Visible;
                edgesWeightTextBox.Visibility = Visibility.Hidden;
                orientedCheckbox.Visibility = Visibility.Hidden;
                currentGraphMode.Text = "Текущий режим: Перемещение";
                isAddVetexOn = false;
                addVertexBtn.ToolTip = "Включить режим добавления вершин";

                ZoomCtrl.Cursor = Cursors.Hand;
                _opMode = EditorOperationMode.Select;
                GraphArea.SetVerticesDrag(true, true);
                ClearEditMode();
            }
            Button btn = sender as Button;
            btn.Background = btn.Background == Brushes.DarkGreen ? (SolidColorBrush)(new BrushConverter().ConvertFrom("#00000000")) : Brushes.DarkGreen;
        }
        private void RemoveVertex_Click(object sender, RoutedEventArgs e)
        {
            if (!isRemoveVertexOn)
            {
                reorderGraph.IsEnabled = false;
                clearGraph.IsEnabled = false;
                addVertexBtn.IsEnabled = false;
                graphGeneratorBtn.IsEnabled = false;
                algorithmsBtn.IsEnabled = false;
                currentGraphMode.Text = "Текущий режим: Удаление";
                isRemoveVertexOn = true;
                deleteVertexBtn.ToolTip = "Выключить режим удаления вершин";

                ZoomCtrl.Cursor = Cursors.Help;
                _opMode = EditorOperationMode.Delete;
                ClearEditMode();
                ClearSelectMode();
            }
            else
            {
                reorderGraph.IsEnabled = true;
                clearGraph.IsEnabled = true;
                addVertexBtn.IsEnabled = true;
                graphGeneratorBtn.IsEnabled = true;
                algorithmsBtn.IsEnabled = true;
                currentGraphMode.Text = "Текущий режим: Перемещение";
                isRemoveVertexOn = false;
                deleteVertexBtn.ToolTip = "Включить режим удаления вершин";
                ZoomCtrl.Cursor = Cursors.Hand;
                _opMode = EditorOperationMode.Select;
                GraphArea.SetVerticesDrag(true, true);
                ClearEditMode();
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
                algorithmsBtn.Visibility = Visibility.Hidden;
                clearGraph.Visibility = Visibility.Hidden;
                reorderGraph.Visibility = Visibility.Hidden;
                randomTreeButton.Visibility = Visibility.Visible;
                randomConnectedGraphButton.Visibility = Visibility.Visible;
                downloadGraphButton.Visibility = Visibility.Visible;
                createGraphButton.Visibility = Visibility.Hidden;
                currentGraphMode.Text = "Текущий режим: Генерация графов";
                isGraphGeneratorOn = true;
                graphGeneratorBtn.ToolTip = "Вылючить режим генерации графов";
            }
            else
            {
                addVertexBtn.Visibility = Visibility.Visible;
                deleteVertexBtn.Visibility = Visibility.Visible;
                algorithmsBtn.Visibility = Visibility.Visible;
                clearGraph.Visibility = Visibility.Visible;
                reorderGraph.Visibility = Visibility.Visible;
                randomTreeButton.Visibility = Visibility.Hidden;
                randomConnectedGraphButton.Visibility = Visibility.Hidden;
                downloadGraphButton.Visibility = Visibility.Hidden;
                createGraphButton.Visibility = Visibility.Hidden;
                currentGraphMode.Text = "Текущий режим: Перемещение";
                isGraphGeneratorOn = false;
                graphGeneratorBtn.ToolTip = "Включить режим генерации графов";
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
                graphGeneratorBtn.Visibility = Visibility.Hidden;
                clearGraph.Visibility = Visibility.Hidden;
                reorderGraph.Visibility = Visibility.Hidden;
                animationSpeed.Visibility = Visibility.Visible;
                showAnimaton.Visibility = Visibility.Visible;
                algorithmsComboBox.Visibility = Visibility.Visible;
                currentGraphMode.Text = "Текущий режим: Алгоритмы";
                isAlgorithmsOn = true;
                algorithmsBtn.ToolTip = "Выключить режим алгоритмов";
                _opMode = EditorOperationMode.Algorithm;
                GraphArea.SetVerticesDrag(false);
                flagNegr = false;

            }
            else
            {
                flagNegr = true;
                addVertexBtn.Visibility = Visibility.Visible;
                deleteVertexBtn.Visibility = Visibility.Visible;
                graphGeneratorBtn.Visibility = Visibility.Visible;
                clearGraph.Visibility = Visibility.Visible;
                reorderGraph.Visibility = Visibility.Visible;
                animationSpeed.Visibility = Visibility.Hidden;
                showAnimaton.Visibility = Visibility.Hidden;
                algorithmsComboBox.Visibility = Visibility.Hidden;
                currentGraphMode.Text = "Текущий режим: Перемещение";
                isAlgorithmsOn = false;
                algorithmsBtn.ToolTip = "Включить режим алгоритмов";
                //DrawAlgorithm();
                GraphArea.SetVerticesDrag(true);
            }
            Button btn = sender as Button;
            btn.Background = btn.Background == Brushes.DarkGray ? (SolidColorBrush)(new BrushConverter().ConvertFrom("#00000000")) : Brushes.DarkGray;
        }
        private void randomTreeButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isRandomTreeOn)
            {
                randomConnectedGraphButton.IsEnabled = false;
                vertexAmountTextBox.Visibility = Visibility.Visible;
                edgesAmountTextBox.Visibility = Visibility.Hidden;
                graphGeneratorBtn.IsEnabled = false;
                downloadGraphButton.Visibility = Visibility.Hidden;
                createGraphButton.Visibility = Visibility.Visible;
                currentGraphMode.Text = "Текущий режим: Случайное дерево";
                isRandomTreeOn = true;
                randomTreeButton.ToolTip = "Выключить генерацию случайного дерева";
            }
            else
            {
                randomConnectedGraphButton.IsEnabled = true;
                vertexAmountTextBox.Visibility = Visibility.Hidden;
                edgesAmountTextBox.Visibility = Visibility.Hidden;
                graphGeneratorBtn.IsEnabled = true;
                downloadGraphButton.Visibility = Visibility.Visible;
                createGraphButton.Visibility = Visibility.Hidden;
                currentGraphMode.Text = "Текущий режим: Генерация графов";
                isRandomTreeOn = false;
                randomTreeButton.ToolTip = "Включить генерацию случайного дерева";
            }
            Button btn = sender as Button;
            btn.Background = btn.Background == Brushes.DarkGreen ? (SolidColorBrush)(new BrushConverter().ConvertFrom("#00000000")) : Brushes.DarkGreen;
        }
        private void randomConnectedGraphButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isRandomConnectedGraphOn)
            {
                randomTreeButton.IsEnabled = false;
                vertexAmountTextBox.Visibility = Visibility.Visible;
                edgesAmountTextBox.Visibility = Visibility.Visible;
                graphGeneratorBtn.IsEnabled = false;
                downloadGraphButton.Visibility = Visibility.Hidden;
                createGraphButton.Visibility = Visibility.Visible;
                currentGraphMode.Text = "Текущий режим: Случайный сввязный граф";
                isRandomConnectedGraphOn = true;
                randomConnectedGraphButton.ToolTip = "Выключить генерацию случайного связного графа";
            }
            else
            {
                randomTreeButton.IsEnabled = true;
                vertexAmountTextBox.Visibility = Visibility.Hidden;
                edgesAmountTextBox.Visibility = Visibility.Hidden;
                graphGeneratorBtn.IsEnabled = true;
                downloadGraphButton.Visibility = Visibility.Visible;
                createGraphButton.Visibility = Visibility.Hidden;
                currentGraphMode.Text = "Текущий режим: Генерация графов";
                isRandomConnectedGraphOn = false;
                randomConnectedGraphButton.ToolTip = "Включить генерацию случайного связного графа";
            }
            Button btn = sender as Button;
            btn.Background = btn.Background == Brushes.DarkGreen ? (SolidColorBrush)(new BrushConverter().ConvertFrom("#00000000")) : Brushes.DarkGreen;
        }
        private void edgesVeightTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            edgesWeightTextBox.Text = edgesWeightTextBox.Text.Replace(" ", "");
            edgesWeightTextBox.SelectionStart = edgesWeightTextBox.Text.Length;
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        /// <summary>
        /// Путь к файлу с графом
        /// </summary>
        /// <returns></returns>
        public string GetNameOfFileWithGraph()
        {
            string filename = null;
            var dlg = new OpenFileDialog()
            {
                Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*"
            };
            bool? result = dlg.ShowDialog();
            if (result == true && result != null)
            {
                filename = dlg.FileName;
            }


            return filename;
        }
        /// <summary>
        /// Сконструировать таблицу связзности загруженного графа
        /// </summary>
        /// <param name="filename"></param>
        public int[,] ReadGraphFromFile(string filename)
        {
            List<string> input = File.ReadAllText(filename).Replace("\r\n", "\n").Replace(" \n", "\n").Split('\n').ToList();
            int size = Convert.ToInt32(input[0]);
            input.RemoveAt(0);
            int i = 0, j = 0;

            int[,] result = new int[size, size];
            foreach (var row in input)
            {
                j = 0;
                foreach (var col in row.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    result[i, j] = int.Parse(col.Trim());
                    j++;
                }
                i++;
            }
            return (int[,])result.Clone();
        }

        private void downloadGraphButton_Click(object sender, RoutedEventArgs e)
        {
            int[,] Data2D = new int[,] { };
            try
            {
                var filename = GetNameOfFileWithGraph();

                if (filename != "")
                {
                    Data2D = (int[,])(ReadGraphFromFile(filename)).Clone();
                }
                else
                    throw new Exception("Не выбран файл для загрузки графа!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!");
            }
            if (Data2D.Length > 1)
            {
                _commands.Clear();
                _commandCounter = -1;
                //сборка графа из таблицы связности
                Builder builder = new GraphBuilder();
                _director = new Director(builder);
                _director.Construct(Data2D);
                Command.Graph = builder.GetResult();

                //отрисовка графа
                _creator = new GraphPainter(Command.Graph);
                _creator.Draw();
            }
        }
        private void vertexAmountTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            vertexAmountTextBox.Text = vertexAmountTextBox.Text.Replace(" ", "");
            vertexAmountTextBox.SelectionStart = vertexAmountTextBox.Text.Length;
        }
        private void edgesAmountTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            edgesAmountTextBox.Text = edgesAmountTextBox.Text.Replace(" ", "");
            edgesAmountTextBox.SelectionStart = edgesAmountTextBox.Text.Length;
        }
        #endregion
        #region random graph
        private void createGraphButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Random rnd = new Random();
                var condEdges = edgesAmountTextBox.Text != "";
                var condVertices = vertexAmountTextBox.Text != "";
                int vertices = condVertices ? int.Parse(vertexAmountTextBox.Text) : 0;
                int edges = condEdges ? int.Parse(edgesAmountTextBox.Text) : 0;
                int[,] Data2D = new int[vertices, vertices];
                var flag = false;
                switch (isRandomConnectedGraphOn)
                {
                    case true:
                        {
                            if (!(vertices < 1 || vertices > 997 || edges < vertices - 1 || edges > vertices * (vertices - 1)))
                            {
                                flag = true;
                                for (int i = vertices - 1; i > 0; i--)
                                {

                                    //если нажата кнопка "без веса"
                                    if (true)
                                    {
                                        Data2D[rnd.Next(0, i), i] = 1;

                                    }
                                    else
                                    {
                                        Data2D[rnd.Next(0, i), i] = rnd.Next(0, 30);
                                    }
                                }
                                int temp = 0;
                                int count = edges - vertices + 1;
                                while (count != 0) {
                                    int i = rnd.Next(0, vertices);
                                    int j = rnd.Next(0, vertices);
                                    if (Data2D[i, j] == 0 && (i != j))
                                    {
                                        //если нажата кнопка "без веса"
                                        if (true)
                                        {
                                            Data2D[i, j] = 1;

                                        }
                                        else
                                        {
                                            Data2D[i, j] = rnd.Next(0, 30);
                                        }
                                        count--;
                                    } else
                                    {
                                        if (temp == 3)
                                        {
                                            bool cringeFlag = false;
                                            for (int ik = 0; ik < vertices - 1; ik++)
                                            {
                                                if (cringeFlag)
                                                {
                                                    break;
                                                }
                                                for (int jk = 0; jk < vertices - 1; jk++)
                                                {
                                                    if (Data2D[ik, jk] == 0 && (ik != jk))
                                                    {

                                                        if (true)
                                                        {
                                                            Data2D[i, j] = 1;
                                                        }
                                                        else
                                                        {
                                                            Data2D[i, j] = rnd.Next(0, 30);
                                                        }
                                                        cringeFlag = true;
                                                        break;
                                                    }
                                                }
                                            }
                                        } else
                                        {
                                            temp += 1;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                
                                MessageBox.Show("Количество ребер не должно быть меньше N-1, количество ребер не должно быть больше N(N-1)/2", "Введено некорректное количество вершин или ребер");
                            }
                            break;
                        }
                    case false:
                        {
                            if (!(vertices < 1 || vertices > 999))
                            {
                                flag = true;
                                for (int i = vertices - 1; i > 0; i--)
                                {
                                    /**
                                     * (rand() % i) - случайное число в множестве [0, i)
                                     * i-тый столбец
                                     */
                                    rnd.Next(1, i);
                                    //если нажата кнопка "без веса"
                                    if (true)
                                    {
                                        Data2D[rnd.Next(0, i), i] = 1;
                                    }
                                    else
                                    {
                                        Data2D[rnd.Next(0, i), i] = rnd.Next(0, 30);
                                    }
                                }

                            } else
                            {
                                MessageBox.Show("Введено некорректное количество вершин", "");
                            }
                            break;
                        }
                };
                if (flag)
                {
                    _commands.Clear();
                    _commandCounter = -1;

                    //сборка графа из таблицы связности
                    Builder builder = new GraphBuilder();
                    _director = new Director(builder);
                    _director.Construct(Data2D);
                    Command.Graph = builder.GetResult();

                    //отрисовка графа
                    _creator = new GraphPainter(Command.Graph);

                    _creator.Draw();
                    FixLabelsAndArrows();

                }
                ZoomCtrl.ZoomToFill();

            } catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "");
            }
        }
      

        
        public void FixLabelsAndArrows()
        {
            foreach (DataEdge edge in GraphArea.EdgesList.Keys)
            {
                if (edge.Weight == 1)
                {
                   
                }
                edge.ArrowBrush = Brushes.Black;
                edge.EdgeBrush = Brushes.Black;
                foreach (DataEdge edge2 in GraphArea.EdgesList.Keys)
                {
                    edge2.EdgeBrush = Brushes.Black;
                    if (edge.Source == edge2.Target && edge.Target == edge2.Source)
                    {

                        edge.ArrowBrush = Brushes.Transparent;
                        edge2.ArrowBrush = Brushes.Transparent;
                    }
                }
            }
        }
        #endregion
        #region Paint panel
        private void BrushButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isBrushModeOn)
            {
                //BrushButton.IsEnabled = false;
                EraserButton.IsEnabled = false;
                Eraser_SmartButton.IsEnabled = false;
                SelectionButton.IsEnabled = false;
                ClearCanvasButton.IsEnabled = false;
                SaveToFileButton.IsEnabled = false;
                if (MainWindow.ActualWidth >= 1400)
                {
                    EraserButton.Visibility = Visibility.Visible;
                    Eraser_SmartButton.Visibility = Visibility.Visible;
                    SaveToFileButton.Visibility = Visibility.Visible;
                }
                else
                {
                    EraserButton.Visibility = Visibility.Hidden;
                    Eraser_SmartButton.Visibility = Visibility.Hidden;
                    SaveToFileButton.Visibility = Visibility.Hidden;
                }
                ClearCanvasButton.Visibility = Visibility.Hidden;
                BrushSlider.Visibility = Visibility.Visible;
                BrushTextBlock.Visibility = Visibility.Visible;
                ColorPickerTextBlock.Visibility = Visibility.Visible;
                ColorPicker.Visibility = Visibility.Visible;
                PaintCanvas.EditingMode = InkCanvasEditingMode.Ink;
                currentPaintMode.Text = "Текущий режим: Кисть";
                BrushButton.ToolTip = "Выключить Кисть";
                //ColorPickerTextBlock.Visibility = Visibility.Visible;
                //ColorPicker.Visibility = Visibility.Visible;
                isBrushModeOn = true;
            }
            else
            {
                //BrushButton.IsEnabled = true;
                EraserButton.IsEnabled = true;
                Eraser_SmartButton.IsEnabled = true;
                SelectionButton.IsEnabled = true;
                ClearCanvasButton.IsEnabled = true;
                SaveToFileButton.IsEnabled = true;
                EraserButton.Visibility = Visibility.Visible;
                Eraser_SmartButton.Visibility = Visibility.Visible;
                SaveToFileButton.Visibility = Visibility.Visible;
                ClearCanvasButton.Visibility = Visibility.Visible;
                BrushSlider.Visibility = Visibility.Hidden;
                BrushTextBlock.Visibility = Visibility.Hidden;
                ColorPickerTextBlock.Visibility = Visibility.Hidden;
                ColorPicker.Visibility = Visibility.Hidden;
                PaintCanvas.EditingMode = InkCanvasEditingMode.None;
                currentPaintMode.Text = "Текущий режим: Курсор";
                BrushButton.ToolTip = "Включить Кисть";
                //ColorPickerTextBlock.Visibility = Visibility.Hidden;
                //ColorPicker.Visibility = Visibility.Hidden;
                isBrushModeOn = false;

            }
            Button btn = sender as Button;
            btn.Background = btn.Background == Brushes.DarkGreen ? (SolidColorBrush)(new BrushConverter().ConvertFrom("#00000000")) : Brushes.DarkGreen;
        }
        private void EraserButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isEraserModeOn)
            {
                BrushButton.IsEnabled = false;
                //EraserButton.IsEnabled = false;
                Eraser_SmartButton.IsEnabled = false;
                SelectionButton.IsEnabled = false;
                ClearCanvasButton.IsEnabled = false;
                SaveToFileButton.IsEnabled = false;
                SaveToFileButton.Visibility = Visibility.Hidden;
                PaintCanvas.EditingMode = InkCanvasEditingMode.EraseByPoint;
                currentPaintMode.Text = "Текущий режим: Ластик";
                EraserButton.ToolTip = "Выключить Ластик";
                EraserTextBlock.Visibility = Visibility.Visible;
                EraserSlider.Visibility = Visibility.Visible;
                isEraserModeOn = true;
            }
            else
            {
                BrushButton.IsEnabled = true;
                //EraserButton.IsEnabled = true;
                Eraser_SmartButton.IsEnabled = true;
                SelectionButton.IsEnabled = true;
                ClearCanvasButton.IsEnabled = true;
                SaveToFileButton.IsEnabled = true;
                SaveToFileButton.Visibility = Visibility.Visible;
                PaintCanvas.EditingMode = InkCanvasEditingMode.None;
                currentPaintMode.Text = "Текущий режим: Курсор";
                EraserButton.ToolTip = "Включить Ластик";
                EraserTextBlock.Visibility = Visibility.Hidden;
                EraserSlider.Visibility = Visibility.Hidden;
                isEraserModeOn = false;
            }
            Button btn = sender as Button;
            btn.Background = btn.Background == Brushes.DarkRed ? (SolidColorBrush)(new BrushConverter().ConvertFrom("#00000000")) : Brushes.DarkRed;
        }
        private void Eraser_SmartButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isEraser_SmartModeOn)
            {
                BrushButton.IsEnabled = false;
                EraserButton.IsEnabled = false;
                //Eraser_SmartButton.IsEnabled = false;
                SelectionButton.IsEnabled = false;
                ClearCanvasButton.IsEnabled = false;
                SaveToFileButton.IsEnabled = false;
                PaintCanvas.EditingMode = InkCanvasEditingMode.EraseByStroke;
                currentPaintMode.Text = "Текущий режим: Умный Ластик";
                Eraser_SmartButton.ToolTip = "Выключить Умный Ластик";
                isEraser_SmartModeOn = true;
            }
            else
            {
                BrushButton.IsEnabled = true;
                EraserButton.IsEnabled = true;
                //Eraser_SmartButton.IsEnabled = true;
                SelectionButton.IsEnabled = true;
                ClearCanvasButton.IsEnabled = true;
                SaveToFileButton.IsEnabled = true;
                PaintCanvas.EditingMode = InkCanvasEditingMode.None;
                currentPaintMode.Text = "Текущий режим: Курсор";
                Eraser_SmartButton.ToolTip = "Включить Умный Ластик";
                isEraser_SmartModeOn = false;
            }
            Button btn = sender as Button;
            btn.Background = btn.Background == Brushes.DarkRed ? (SolidColorBrush)(new BrushConverter().ConvertFrom("#00000000")) : Brushes.DarkRed;
        }
        private void SelectionButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isSelectionModeOn)
            {
                BrushButton.IsEnabled = false;
                EraserButton.IsEnabled = false;
                Eraser_SmartButton.IsEnabled = false;
                //SelectionButton.IsEnabled = false;
                ClearCanvasButton.IsEnabled = false;
                SaveToFileButton.IsEnabled = false;
                PaintCanvas.EditingMode = InkCanvasEditingMode.Select;
                currentPaintMode.Text = "Текущий режим: Выделение";
                SelectionButton.ToolTip = "Выключить Режим Выделения";
                isSelectionModeOn = true;
            }
            else
            {
                BrushButton.IsEnabled = true;
                EraserButton.IsEnabled = true;
                Eraser_SmartButton.IsEnabled = true;
                //SelectionButton.IsEnabled = true;
                ClearCanvasButton.IsEnabled = true;
                SaveToFileButton.IsEnabled = true;
                PaintCanvas.EditingMode = InkCanvasEditingMode.None;
                currentPaintMode.Text = "Текущий режим: Курсор";
                SelectionButton.ToolTip = "Включить Режим Выделения";
                isSelectionModeOn = false;
            }
            Button btn = sender as Button;
            btn.Background = btn.Background == Brushes.DarkOrange ? (SolidColorBrush)(new BrushConverter().ConvertFrom("#00000000")) : Brushes.DarkOrange;
        } 
        private void ClearCanvasButton_Click(object sender, RoutedEventArgs e)
        {
          //  MessageBox.Show(PaintCanvas.Strokes[0].StylusPoints.ToString(), "");
            PaintCanvas.Strokes.Clear();
        }
        private void SaveToFileButton_Click(object sender, RoutedEventArgs e)
        {
            //   PaintCanvasScroll.ScrollToTop();
            //   PaintCanvasScroll.ScrollToLeftEnd();
            
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "untitled"; // Default file name
            dlg.DefaultExt = "jpg"; // Default file extension
            dlg.AddExtension = true;
            dlg.Filter = "jpg (*.jpg)|*.jpg|jpeg (*.jpeg)|*.jpeg|png (*.png)|*.png"; // Filter files by extension
            dlg.InitialDirectory = desktopPath;
            dlg.Title = "Сохранить изображение";
            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                try
                {

                    string filepath = dlg.FileName;
                    RenderTargetBitmap rtb = new RenderTargetBitmap((int)PaintCanvas.ActualWidth, (int)PaintCanvas.ActualHeight, 96d, 96d, PixelFormats.Default);
                    rtb.Render(PaintCanvas);
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(rtb));
                    FileStream fs = File.Open(filepath, FileMode.Create);
                    encoder.Save(fs);
                    fs.Close();
                }
                catch (IOException copyError)
                {
                    Console.WriteLine(copyError.Message);
                    MessageBox.Show("Что-то пошло не так.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void ColorPicker_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var CurrentColor = ColorPicker.Color;
            PaintCanvas.DefaultDrawingAttributes.Color = CurrentColor;
        }
        private void ColorPicker_ColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            var CurrentColor = ColorPicker.Color;
            PaintCanvas.DefaultDrawingAttributes.Color = CurrentColor;
        }
        private void ColorPicker_LayoutUpdated(object sender, EventArgs e)
        {
            var CurrentColor = ColorPicker.Color;
            PaintCanvas.DefaultDrawingAttributes.Color = CurrentColor;
        }
        private void EraserSlider_LayoutUpdated(object sender, EventArgs e)
        {
            var eraserSize = EraserSlider.Value;
            PaintCanvas.EraserShape = new EllipseStylusShape(eraserSize, eraserSize);
            EraserTextBlock.Text = "Размер Ластика: " + (eraserSize).ToString();
        }
        private void EraserSlider_MouseEnter(object sender, MouseEventArgs e)
        {
            if (isEraserModeOn)
            {
                var eraserSize = EraserSlider.Value;
                PaintCanvas.EditingMode = InkCanvasEditingMode.None;
                PaintCanvas.EraserShape = new EllipseStylusShape(eraserSize, eraserSize);
            }
        }
        private void EraserSlider_MouseLeave(object sender, MouseEventArgs e)
        {
            if (isEraserModeOn)
            {
                var eraserSize = EraserSlider.Value;
                PaintCanvas.EditingMode = InkCanvasEditingMode.EraseByPoint;
                PaintCanvas.EraserShape = new EllipseStylusShape(eraserSize, eraserSize);
            }
        }
        private void BrushButton_LayoutUpdated(object sender, EventArgs e)
        {
            if (MainWindow.ActualWidth >= 1400 && isBrushModeOn == true)
            {
                EraserButton.Visibility = Visibility.Visible;
                Eraser_SmartButton.Visibility = Visibility.Visible;
                SaveToFileButton.Visibility = Visibility.Visible;
                ColorPickerTextBlock.SetValue(Canvas.LeftProperty, 350.0);
                ColorPicker.SetValue(Canvas.LeftProperty, 350.0);
            }
            else if (MainWindow.ActualWidth < 1400 && isBrushModeOn == true)
            {
                EraserButton.Visibility = Visibility.Hidden;
                Eraser_SmartButton.Visibility = Visibility.Hidden;
                SaveToFileButton.Visibility = Visibility.Hidden;
                ColorPickerTextBlock.SetValue(Canvas.LeftProperty, 180.0);
                ColorPicker.SetValue(Canvas.LeftProperty, 180.0);
            }
        }
        private void PaintCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            //Координаты мыши на paint-канвасе
            //Point p = Mouse.GetPosition(PaintCanvas);
            //currentPaintMode.Text = p.ToString();
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
        }
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.sendButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }
        private void ChatTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string userInput = chatTextBox.Text;
            char charCount;
            if (userInput != "")
            {
                charCount = userInput[0];
            }
            charCounterTextBlock.Text = "Символов " + userInput.Length.ToString() + "/200";
        }
        private void Ez_Click(object sender, RoutedEventArgs e)
        {
        }
        private void RefreshRoomsList(object sender, RoutedEventArgs e)
        {
            RefreshRooms();
        }
        #endregion

        private void BrushSlider_LayoutUpdated(object sender, EventArgs e)
        {
            var brushSize = BrushSlider.Value;
            PaintCanvas.DefaultDrawingAttributes.Width = brushSize;
            PaintCanvas.DefaultDrawingAttributes.Height = brushSize;
            BrushTextBlock.Text = "Размер Кисти: " + (brushSize).ToString();
        }

        private void BrushSlider_MouseEnter(object sender, MouseEventArgs e)
        {
            var brushSize = BrushSlider.Value;
            PaintCanvas.EditingMode = InkCanvasEditingMode.None;
            PaintCanvas.DefaultDrawingAttributes.Width = brushSize;
            PaintCanvas.DefaultDrawingAttributes.Height = brushSize;
        }

        private void BrushSlider_MouseLeave(object sender, MouseEventArgs e)
        {
            var brushSize = BrushSlider.Value;
            PaintCanvas.EditingMode = InkCanvasEditingMode.Ink;
            PaintCanvas.DefaultDrawingAttributes.Width = brushSize;
            PaintCanvas.DefaultDrawingAttributes.Height = brushSize;
        }
        
        private void PaintCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //var strokeJSON = JsonConvert.SerializeObject(PaintCanvas.Strokes.Last().StylusPoints.ToList());
            //var stroke2 = JsonConvert.DeserializeObject<StylusPointCollection>(strokeJSON);
            //var drawingAttributes = new DrawingAttributes() { Color = Colors.Red,
            //FitToCurve = true};
            //var stroke = new Stroke(stroke2,drawingAttributes);
            //PaintCanvas.Strokes.Add(stroke);
            if (PaintCanvas.EditingMode == InkCanvasEditingMode.Ink)
            {
                if(!(PaintCanvas.Strokes.Count == 0))
                SocketConnector.SendStroke(PaintCanvas.Strokes.Last());
            }
        }
        private void ClearGraphs_Click(object sender, RoutedEventArgs e)
        {
            GraphArea.ClearLayout();
        }

        private void ReorderGraph_Click(object sender, RoutedEventArgs e)
        {
            GraphArea.RelayoutGraph();
        }

        private void SaveGraph_Click(object sender, RoutedEventArgs e)
        {
            //save to JSON
        }

        private void animationSpeed_Click(object sender, RoutedEventArgs e)
        {
            //Switch case chi sho?
            //case 1
            //case 2
            //case 3
        }
        private void showAlgorithm_Click(object sender, RoutedEventArgs e)
        {
            //Animation
        }
        private void algorithmsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //ComboBox comboBox = (ComboBox)sender;
            //ComboBoxItem selectedItem = (ComboBoxItem)comboBox.SelectedItem;
            //MessageBox.Show(selectedItem.Content.ToString());
        }
    }
}

//ToDo
//1)suck koke