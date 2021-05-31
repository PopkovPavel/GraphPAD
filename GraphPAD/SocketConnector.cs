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

            };

            client.EmitAsync("update-paint-canvas", jSONstroke);
        }
        public static void SendGraph(GraphData.Model.GraphZone graph)
        {
            var Graph = new QuickGraph.BidirectionalGraph<GraphData.Model.DataVertex, GraphData.Model.DataEdge>();
            var edges = graph.EdgesList;
            var vertices = graph.VertexList;
            foreach (var v in vertices.Keys)
            {
                Graph.AddVertex(v);
            }
            foreach (var e in edges.Keys)
            {
                Graph.AddEdge(e);
            }
            client.EmitAsync("send-graph", Graph);
        }
    }
}

