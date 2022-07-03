using System;
using CefSharp;

namespace Spotifly
{
    public partial class Form1
    {
        private readonly string initialBrowserUrl = "https://www.youtube.com/";
        private string currentLink;

        private void WebBrowser_AddressChanged(object sender, AddressChangedEventArgs e)
        {
            try
            {
                if (e.Address.Contains(initialBrowserUrl) && e.Address != initialBrowserUrl && e.Address.Contains("/watch"))//address is a video
                {
                    GetVideo(e.Address);
                    System.Threading.Thread.Sleep(2000);
                    Invoke(new Action(() => SetDownloadGroupBox(true)));
                }
                else
                {
                    Invoke(new Action(() => SetDownloadGroupBox(false)));
                }
                BrowserBackBttn.Visible = !IsBrowserOnHomePage();
                DownloadGroupBox.Visible = IsBrowserOnVideo();
            }
            catch (Exception)
            {  }
            
            void SetDownloadGroupBox(bool value)
            {
                if (value && !downloading)
                {
                    StaticWebDwnldSttsLabel.Text = (string)table["status"];
                    WebDownloadStatusLabel.Text = (string)table["prepared"];
                }
                DownloadGroupBox.Enabled = value;
                DownloadGroupBox.BringToFront();
            }
        }

        private void WebVideoDwnldBtnn_Click(object sender, EventArgs e)
        {
            DownloadVideo(WebBrowser.Address);
        }

        private void WebAudioDwnldBttn_Click(object sender, EventArgs e)
        {
            DownloadVideo(WebBrowser.Address, true);
        }
        
        private void BrowserBackBttn_Click(object sender, EventArgs e)
        {
            if (WebBrowser.CanGoBack)
                WebBrowser.Back();
        }

        private bool IsBrowserOnVideo() => WebBrowser.Address.Contains("/watch");
        private bool IsBrowserOnHomePage() => WebBrowser.Address == initialBrowserUrl;
    }
}
