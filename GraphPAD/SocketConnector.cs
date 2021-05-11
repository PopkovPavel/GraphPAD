using SocketIOClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
        public static void UpdateGraph(string message)
        {
            
        }
        public static void UpdatePaint(string message)
        {
            
        }
    }
}

