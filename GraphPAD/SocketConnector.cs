using GraphPAD.Data.JSON;
using Newtonsoft.Json;
using SocketIOClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Ink;

namespace GraphPAD
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public static class SocketConnector
    {
        public static SocketIO client { get; set; }
        public static bool IsConnected { get; set; }
        private static string _roomId { get; set; }
        private static string _name { get; set; }
        

        public static async Task InitializeClientAsync()
        {
            client = new SocketIO("https://testingwebrtc.herokuapp.com/", new SocketIOOptions
            {
                EIO = 4
            });
            client.OnConnected += async (sender, e) =>
            {
                
                Console.WriteLine("connected");
                await client.EmitAsync("join-room", _roomId, _name);
            };
            client.OnDisconnected += async (sender, e) =>
            {

                Console.WriteLine("disconnected");
      
            };
            await Connect();

        } 
        public static async Task Connect()
        {
            await client.ConnectAsync();
            IsConnected = true;
        }
        public static async Task Disconnect()
        {
            await client.DisconnectAsync();
            IsConnected = false;
            
        }
        public static void SetSettings(string roomId, string name)
        {
            _roomId = roomId;
            _name = name;
        }
        public static void SendMessage(string message)
        {
            client.EmitAsync("send-chat-message", message);
        }
        public static void SendStroke(Stroke stroke)
        {
            var stylusPoints = stroke.StylusPoints;
            var color = stroke.DrawingAttributes.Color;
            var width = stroke.DrawingAttributes.Width;
            JSONstroke jSONstroke = new JSONstroke()
            {
                StrokeArray = stylusPoints,
                Color = color,
                Width = width
                //Color = color.ToString(),
                //Width = width.ToString()
            };
                
            var strokeJSON = JsonConvert.SerializeObject(jSONstroke);
            client.EmitAsync("update-paint-canvas", jSONstroke);
        }
        public static void SendGraph(string graph)
        {
            client.EmitAsync("send-graph", graph);
        }
    }
}

