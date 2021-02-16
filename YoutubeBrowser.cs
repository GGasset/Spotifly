using System;
using CefSharp;

namespace Spotifly
{
    public partial class Form1
    {
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
                    Invoke(new Action(() => SetDownloadGroupBox(false)));
            }
            catch (Exception)
            {  }
            
            void SetDownloadGroupBox(bool value)
            {
                if (value && !downloading)
                    WebDwnldSttsLabel.Text = string.Empty;
                DownloadGroupBox.Visible = value;
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
    }
}
