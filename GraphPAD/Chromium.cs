
namespace GraphPAD
{

    public static class Chromium
    {
        private static string _roomId;
        private const string BaseUrl = "https://testingwebrtc.herokuapp.com/room/";
        public static CefSharp.Wpf.CefSettings settings = new CefSharp.Wpf.CefSettings();
        public static CefSharp.Wpf.ChromiumWebBrowser Connect()
        {
            try
            {
                var tempUrl = BaseUrl + _roomId + "/view";
                //var tempUrl = "google.com";
                CefSharp.Wpf.ChromiumWebBrowser browser = new CefSharp.Wpf.ChromiumWebBrowser(tempUrl);
                browser.MenuHandler = new CustomMenuHandler();
                
                return browser;
            }
            catch
            {
                System.Windows.MessageBox.Show("something went wrong");
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
