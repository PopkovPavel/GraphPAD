using CefSharp;
using CefSharp.Wpf;
using System.Windows;

namespace GraphPAD
{

    public static class Chromium
    {
        private static string _roomId;
        private const string BaseUrl = "https://testingwebrtc.herokuapp.com/room/";
        public static CefSettings settings= new CefSettings();
        public static ChromiumWebBrowser Connect()
        {
            try
            {
                //var tempUrl = BaseUrl + _roomId + "/view";
                var tempUrl = "google.com";
                ChromiumWebBrowser browser = new ChromiumWebBrowser(tempUrl);
                browser.MenuHandler = new CustomMenuHandler();
                
                return browser;
            }
            catch
            {
                MessageBox.Show("something went wrong");
                return null;
            }
        }
        public static void Disconnect()
        {
          
        }
        public static void SetSettings(string roomId)
        {
            _roomId = roomId;

        }
    }
}
