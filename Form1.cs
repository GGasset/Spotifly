using CefSharp.WinForms;
using Spotifly.Properties;
using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Spotifly
{
    public partial class Form1 : Form
    {
        internal Form MainForm { get => this; }

        private static string AppName { get => "Spotifly"; }

        private readonly ChromiumWebBrowser WebBrowser;
        private readonly Size panelSize = new Size(740, 418), mediaPanelSize;
        private readonly Point panelLocation = new Point(141, 12), mediaPanelLocation;
        private Panel[] panels;
        private string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) + $@"\{AppName}", currentUrlFolder;
        private bool shuffle, loading = true, showRemainingTimeInElapsed = Settings.Default.ShowRemainingTimeInElapsed;
        private int playlistIndex = 0, activePanelIndex, verticalModeMinWidth = 710, normalMinWidth, verticalModeStart = 750;
        private readonly int initialMediaLengthLabelDistanceToFormEnd;
        private readonly Hashtable table = new Hashtable();
        private readonly Random random = new Random(DateTime.Now.Millisecond);


        public Form1()
        {
            InitializeComponent();
            Directory.CreateDirectory(initialFolderPath);

            normalMinWidth = MinimumSize.Width;
            initialMediaLengthLabelDistanceToFormEnd = Width - (MediaLengthLabel.Location.X + MediaLengthLabel.Width);
            mediaPanelLocation = new Point(PanelGroupBox.Width + PanelGroupBox.Location.X, 0);
            mediaPanelSize = new Size(Width - mediaPanelLocation.X, ControlPanel.Location.Y + 5);

            WebBrowser = new ChromiumWebBrowser(initialBrowserUrl)
            {
                Parent = YoutubeBrowserPanel,
                Dock = DockStyle.Fill,
            };
            WebBrowser.AddressChanged += WebBrowser_AddressChanged;
            BrowseBttn.Parent = PanelGroupBox;
            ChangeTheme(currentTheme = Settings.Default.LastSessionTheme);
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            //Settings.Default.Reset();
            panels = new Panel[5];
            panels[0] = MediaPlayerPanel;
            panels[1] = DownloadedMediaPanel;
            //panels[2] = BrowserPanel;
            panels[2] = YoutubeBrowserPanel;
            panels[3] = SettingsPanel;
            panels[4] = LoadingPanel;

            table.Add("er", "ERROR");
            table.Add("wait", "Please wait until the download finishes to download another video");
            table.Add("preparing", "Download Status: Preparing...");
            table.Add("download", "Download Status: Downloading...");
            table.Add("finished", "Download Status: Prepared");
            table.Add("getEr", "There has been a problem getting the video.");
            table.Add("path", "Couldn't reach the download path. Please restart the app");
            table.Add("internet", "There has been an internet error. Please check your internet conection.");

            #region prepareForm

            var watch = System.Diagnostics.Stopwatch.StartNew();

            SetActivePanel(4);

            LoadingPanel.Size = panelSize;
            LoadingPanel.Location = panelLocation;
            YoutubeBrowserPanel.Size = mediaPanelSize;
            YoutubeBrowserPanel.Location = mediaPanelLocation;
            BrowserPanel.Size = panelSize;
            BrowserPanel.Location = panelLocation;
            DownloadedMediaPanel.Size = mediaPanelSize;
            DownloadedMediaPanel.Location = mediaPanelLocation;
            SettingsPanel.Size = panelSize;
            SettingsPanel.Location = panelLocation;
            MediaPlayerPanel.Size = mediaPanelSize;
            MediaPlayerPanel.Location = mediaPanelLocation;
            CurrentMediaTxtBox.Size = new Size(PrevMediaBttn.Location.X - CurrentMediaTxtBox.Location.X, 20);

            CurrentMediaTxtBox.Text = "";
            ElapsedTimeLabel.Text = "";
            MediaLengthLabel.Text = "";
            folderLabel.Text = $"Looking to media files in {folderPath}";
            TitleTextBox.Text = "";
            ViewsLabel.Text = "";
            UploadDateLabel.Text = "";
            DescriptionTextBox.Text = "";
            DwnldSttsLabel.Text = "";
            WebDwnldSttsLabel.Text = "";


            if (Settings.Default.LastSessionFolder.Length != 0 && Directory.Exists(Settings.Default.LastSessionFolder))
                folderPath = Settings.Default.LastSessionFolder;

            BackBttn.Visible = folderPath != initialFolderPath;

            string lastSessionUrl = Settings.Default.lastSessionMediaURL;
            if (File.Exists(lastSessionUrl))
                currentUrlFolder = lastSessionUrl.Remove(lastSessionUrl.LastIndexOf('\\'));

            VolumeTrackBar.Value = Settings.Default.LastSessionVolume;

            if (ResizeForMediaCheckBox.Checked = Settings.Default.ResizeForMedia)
                FormBorderStyle = FormBorderStyle.FixedSingle;
            else
                FormBorderStyle = FormBorderStyle.Sizable;
            axWindowsMediaPlayer.stretchToFit = ResizeForMediaCheckBox.Checked;

            MediaListView_DrawMedia("");

            SetShuffleBttn(Settings.Default.Shuffle);
            if (!string.IsNullOrEmpty(Settings.Default.lastSessionMediaURL))
                try
                {
                    string URL;
                    if (File.Exists(URL = Settings.Default.lastSessionMediaURL))
                    {
                        axWindowsMediaPlayer.settings.volume = 0;
                        for (int i = 0; i < urlPlaylist.Length; i++)
                            if (URL == urlPlaylist[i])
                            {
                                playlistIndex = i;
                                break;
                            }
                        PlayFile(urlPlaylist[playlistIndex]);
                    }
                }
                catch { }

            axWindowsMediaPlayer.uiMode = "none";
            axWindowsMediaPlayer.stretchToFit = true;
            CurrentMediaTxtBox.Size = new Size(PrevMediaBttn.Location.X - CurrentMediaTxtBox.Location.X, 20);
            ThemeSelectionComboBox.Items.AddRange(GetThemes());
            priorityQueue = new System.Collections.Generic.Queue<string>();

            watch.Stop();
            Settings.Default.AverageLoadingTime = Convert.ToInt32((watch.Elapsed.Milliseconds + Settings.Default.AverageLoadingTime) / 2);
            table.Add("about", $"Developed by Germán Gasset Martí\nYoutubeExplode was used to download youtube videos\naverageLoadingTime: {Settings.Default.AverageLoadingTime}ms, currentLoadingTime: {watch.Elapsed.Milliseconds}ms" +
                    "\n\n\n\t\t\t\tCopyright ©  2020 Germán Gasset About");
            int elapsedMS = (int)watch.ElapsedMilliseconds, minWait = 600;
            await Task.Delay(Math.Max(0, minWait - elapsedMS)).ConfigureAwait(true);
            loading = false;

            if (string.IsNullOrWhiteSpace(Settings.Default.lastSessionMediaURL) && MediaListView.Items.Count > 0)
                SetActivePanel(1);
            else if (MediaListView.Items.Count == 0)
            {
                SetActivePanel(2);
            }
            else
            {
                if (ResizeForMediaCheckBox.Checked)
                {
                    SetFormSizeForCurrentMedia();
                    FormBorderStyle = FormBorderStyle.FixedSingle;
                }
                axWindowsMediaPlayer.Ctlcontrols.currentPosition = Settings.Default.LastSessionTime;

                CurrentMediaTxtBox.Text = GetCurrentMediaName();
                SetActivePanel(0);
            }

            UpdatesStart();

            ElapsedTimeBarPictureBox.Visible = true;

            #endregion prepareForm
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            CurrentMediaTxtBox.Size = new Size(PrevMediaBttn.Location.X - CurrentMediaTxtBox.Location.X, 20);

            if (ResizeForMediaCheckBox.Checked && panels[0].Visible)
                return;
            MinimumSize = new Size(Convert.ToInt32(Height > verticalModeStart) * verticalModeMinWidth +
                       Convert.ToInt32(Height < verticalModeStart) * normalMinWidth, MinimumSize.Height);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer.Dispose();
            progressBarBrush.Dispose();
            WebBrowser.Dispose();
            if (!loading && e.CloseReason != CloseReason.TaskManagerClosing && e.CloseReason != CloseReason.None)
            {
                Settings.Default.lastSessionMediaURL = urlPlaylist[playlistIndex];
                Settings.Default.LastSessionTime = axWindowsMediaPlayer.Ctlcontrols.currentPosition;
                Settings.Default.LastSessionVolume = VolumeTrackBar.Value;
                Settings.Default.LastSessionFolder = folderPath;
                Settings.Default.Shuffle = shuffle;
                Settings.Default.ResizeForMedia = ResizeForMediaCheckBox.Checked;
                Settings.Default.ShowRemainingTimeInElapsed = showRemainingTimeInElapsed;
                Settings.Default.LastSessionTheme = currentTheme;
                //Settings.Default.Reset();
                Settings.Default.Save();
                //Settings.Default.Reload();
                //Settings.Default.Upgrade();
            }
        }

        #region panelControl

        private void SetActivePanel(int panelIndex)
        {
            activePanelIndex = panelIndex;
            foreach (Panel panel in panels)
                panel.Visible = ReferenceEquals(panel, panels[panelIndex]);
        }

        private void SetFormSizeForCurrentMedia()
        {
            try
            {
                if (Math.Min(axWindowsMediaPlayer.currentMedia.imageSourceWidth, axWindowsMediaPlayer.currentMedia.imageSourceHeight) <= 0)
                {
                    Size = new Size(normalMinWidth, MinimumSize.Height);
                    return;
                }
            }
            catch
            { return; }
            Size minSize = mediaPanelSize, maxSize = new Size(1307, 1307), initialSizeDifference, imageSourceSize, mediaPlayerSize = new Size(), formSize;
            initialSizeDifference = new Size(MainForm.MinimumSize.Width - mediaPanelSize.Width, MainForm.MinimumSize.Height - mediaPanelSize.Height);
            imageSourceSize = new Size(axWindowsMediaPlayer.currentMedia.imageSourceWidth, axWindowsMediaPlayer.currentMedia.imageSourceHeight);

            double videoAspectRatio;
            if (imageSourceSize.Width > imageSourceSize.Height)
            {
                videoAspectRatio = imageSourceSize.Width / Convert.ToDouble(imageSourceSize.Height);
                mediaPlayerSize.Height = SetBetwenBounds(new Size(0, axWindowsMediaPlayer.currentMedia.imageSourceHeight)).Height;
                mediaPlayerSize.Width = Convert.ToInt32(mediaPlayerSize.Height * videoAspectRatio);
            }
            else
            {
                videoAspectRatio = imageSourceSize.Height / Convert.ToDouble(imageSourceSize.Width);
                mediaPlayerSize.Width = SetBetwenBounds(new Size(axWindowsMediaPlayer.currentMedia.imageSourceWidth, 0)).Width;
                mediaPlayerSize.Height = Convert.ToInt32(mediaPlayerSize.Width * videoAspectRatio);
            }
            //mediaPlayerSize = new Size(mediaPanelSize.Width, (int)(mediaPlayerSize.Width * videoAspectRatio));
            formSize = new Size(mediaPlayerSize.Width + initialSizeDifference.Width, mediaPlayerSize.Height + initialSizeDifference.Height);
            Size = formSize;

            Size SetBetwenBounds(Size size) //=> new Size(Math.Max(minSize.Width, Math.Min(maxSize.Width, size.Width)), Math.Max(Math.Min(maxSize.Height, size.Height), minSize.Height));
            {
                Size output = new Size
                {
                    Width = Math.Max(minSize.Width, Math.Min(maxSize.Width, size.Width)),
                    Height = Math.Max(minSize.Height, Math.Min(maxSize.Height, size.Height))
                };
                return output;
            }
        }

        private void MediaPlayerBttn_Click(object sender, EventArgs e)
        {
            if (!loading)
            {
                if (ResizeForMediaCheckBox.Checked)
                {
                    SetFormSizeForCurrentMedia();
                    FormBorderStyle = FormBorderStyle.FixedSingle;
                }
                else
                {
                    FormBorderStyle = FormBorderStyle.Sizable;
                }
                SetActivePanel(0);
            }
        }

        private void SongsBttn_Click(object sender, EventArgs e)
        {
            if (!loading)
            {
                if (FormBorderStyle == FormBorderStyle.FixedSingle)
                    ActiveForm.FormBorderStyle = FormBorderStyle.Sizable;
                BackBttn.Visible = folderPath != initialFolderPath;
                MediaListView_DrawMedia(null);
                SetActivePanel(1);
            }
        }

        private void BrowseBttn_Click(object sender, EventArgs e)
        {
            if (!loading)
            {
                if (FormBorderStyle == FormBorderStyle.FixedSingle)
                    ActiveForm.FormBorderStyle = FormBorderStyle.Sizable;
                SetActivePanel(2);
            }
        }

        private void SettingsBttn_Click(object sender, EventArgs e)
        {
            if (!loading)
            {
                if (FormBorderStyle == FormBorderStyle.FixedSingle)
                    ActiveForm.FormBorderStyle = FormBorderStyle.Sizable;
                SetActivePanel(3);
            }
        }

        #endregion panelControl

        #region changeButtonColours

        private void MediaPlayerBttn_MouseEnter(object sender, EventArgs e)
        {
            GetColorsForTheme(currentTheme, out _, out _, out Color color, out _, out _);
            MediaPlayerBttn.ForeColor = color;
        }

        private void SongsBttn_MouseEnter(object sender, EventArgs e)
        {
            GetColorsForTheme(currentTheme, out _, out _, out Color color, out _, out _);
            SongsBttn.ForeColor = color;
        }

        private void Button2_MouseEnter(object sender, EventArgs e)
        {
            GetColorsForTheme(currentTheme, out _, out _, out Color color, out _, out _);
            BrowseBttn.ForeColor = color;
        }

        private void SettingsBttn_MouseEnter(object sender, EventArgs e)
        {
            GetColorsForTheme(currentTheme, out _, out _, out Color color, out _, out _);
            SettingsBttn.ForeColor = color;
        }

        private void BackBttn_MouseEnter(object sender, EventArgs e)
        {
            GetColorsForTheme(currentTheme, out _, out _, out Color color, out _, out _);
            BackBttn.ForeColor = color;
        }

        private void OpenCurrentFldrBttn_MouseEnter(object sender, EventArgs e)
        {
            GetColorsForTheme(currentTheme, out _, out _, out Color color, out _, out _);
            OpenCurrentFldrBttn.ForeColor = color;
        }

        private void MediaPlayerBttn_MouseLeave(object sender, EventArgs e)
        {
            GetColorsForTheme(currentTheme, out _, out Color color, out _, out _, out _);
            MediaPlayerBttn.ForeColor = color;
        }

        private void SongsBttn_MouseLeave(object sender, EventArgs e)
        {
            GetColorsForTheme(currentTheme, out _, out Color color, out _, out _, out _);
            SongsBttn.ForeColor = color;
        }

        private void Button2_MouseLeave(object sender, EventArgs e)
        {
            GetColorsForTheme(currentTheme, out _, out Color color, out _, out _, out _);
            BrowseBttn.ForeColor = color;
        }

        private void SettingsBttn_MouseLeave(object sender, EventArgs e)
        {
            GetColorsForTheme(currentTheme, out _, out Color color, out _, out _, out _);
            SettingsBttn.ForeColor = color;
        }

        private void BackBttn_MouseLeave(object sender, EventArgs e)
        {
            GetColorsForTheme(currentTheme, out _, out Color color, out _, out _, out _);
            BackBttn.ForeColor = color;
        }

        private void OpenCurrentFldrBttn_MouseLeave(object sender, EventArgs e)
        {
            GetColorsForTheme(currentTheme, out _, out Color color, out _, out _, out _);
            OpenCurrentFldrBttn.ForeColor = color;
        }

        #endregion changeButtonColours
    }
}