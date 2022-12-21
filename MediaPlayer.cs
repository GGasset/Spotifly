using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using System.Text.RegularExpressions;

namespace Spotifly
{
    public partial class Form1
    {
        private Queue<string> queuedMedia;
        private Queue<string> folderQueue;
        private bool isPlaying = false;

        private void WindowsMediaPlayer_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            bool fScreen = axWindowsMediaPlayer.fullScreen;
            switch (e.newState)
            {
                // Paused
                case 2:
                    if (fScreen)
                        isPlaying = false;
                    break;

                // Playing
                case 3:
                    if (fScreen)
                        isPlaying = true;
                    break;

                // Media Ended
                case 8:
                    PlayFile(1);
                    if (fScreen)
                    {
                        axWindowsMediaPlayer.uiMode = "mini";
                        axWindowsMediaPlayer.fullScreen = true;
                    }
                    break;
                default:
                    break;
            }

        }

        private void WindowsMediaPlayer_MediaChange(object sender, AxWMPLib._WMPOCXEvents_MediaChangeEvent e)
        {
            if (ResizeForMediaCheckBox.Checked && panels[0].Visible)
                SetFormSizeForCurrentMedia();
        }

        private int AdvanceIndexesOnPlaylists(int currentIndex, int PositionsToAdvance, int PlaylistLength)
        {
            int output = currentIndex + PositionsToAdvance;

            //Check if output is inside the playlist
            if (output < PlaylistLength && output > 0)
                return output;

            if (output >= PlaylistLength)
            {
                return 0;
            }
            else
            {
                return PlaylistLength - 1;
            }
        }

        private void PlayFileInUnshuffled(string name, string mediaFolderPath, bool checkForMediaIndex = true)
        {
            try
            {
                GetFilteredFilesAndFolders(mediaFolderPath, out string[] unshuffled, out _);
                int index = -1;
                name = UrlToName(name);

                for (int i = 0; i < unshuffled.Length; i++)
                {
                    if (UrlToName(unshuffled[i]) == name)
                    {
                        index = i;
                        break;
                    }
                }

                if (index != -1)
                {
                    PlayFileInUnshuffled(index, mediaFolderPath, checkForMediaIndex);
                }
            }
            catch (Exception)
            {

            }
        }

        private void PlayFileInUnshuffled(int index, string folderPath = "", bool checkForMediaIndex = true)
        {
            try
            {
                string[] unshuffled;
                folderPath = folderPath == string.Empty? this.folderPath : folderPath;
                GetFilteredFilesAndFolders(folderPath, out unshuffled, out _);
                PlayFile(unshuffled[index], false, checkForMediaIndex);
            }
            catch (Exception)
            {

            }
        }

        private void PlayFile(string URL, bool isName = false, bool checkPlaylistIndex = true)
        {
            //if is a name parse url to be a url
            if (isName)
            {
                for (int i = 0; i < urlPlaylist.Length; i++)
                {
                    if (UrlToName(urlPlaylist[i]) == URL)
                        URL = urlPlaylist[i];
                }
            }

            Task.Run(() => SetURL(URL));

            if (checkPlaylistIndex)
                playlistIndex = CheckPlaylistIndex();
        }

        bool isQueued = false;
        private void PlayFile(int positionsToAdvance)
        {
            if (queuedMedia.Count > 0 && positionsToAdvance == 1)
            {
                isQueued = true;
                Task.Run(() => PlayFileInUnshuffled(queuedMedia.Dequeue(), folderQueue.Dequeue(), CheckMediaIndexWithSongQueueCheckBox.Checked));
            }
            else
            {
                if (playlistIndex != -1)
                    Task.Run(() => SetURL(urlPlaylist[playlistIndex = AdvanceIndexesOnPlaylists(playlistIndex, positionsToAdvance, urlPlaylist.Length)]));
                else
                    Task.Run(() => SetURL(urlPlaylist[playlistIndex = 0]));
                isQueued = false;
            }
            UpdateQueuedMediaListView();
        }

        private void SetURL(string URL)
        {
            try
            {
                if (URL != axWindowsMediaPlayer.URL)
                {
                    currentUrlFolder = URL.Remove(URL.LastIndexOf(@"\"));
                    axWindowsMediaPlayer.URL = URL;
                    axWindowsMediaPlayer.Ctlcontrols.currentPosition = 0;
                    CurrentMediaTxtBox.Text = UrlToName(URL);
                    do
                    {
                        Thread.Sleep(10);
                    } while (axWindowsMediaPlayer.playState == WMPLib.WMPPlayState.wmppsTransitioning);
                    axWindowsMediaPlayer.Ctlcontrols.play();
                    /*Thread.Sleep(50);
                    axWindowsMediaPlayer.Ctlcontrols.pause();*/
                    /*if (startPlaying)
                        try
                        {
                            while (axWindowsMediaPlayer.playState != WMPLib.WMPPlayState.wmppsPlaying)
                            {
                                System.Threading.Thread.Sleep(500);
                                if (axWindowsMediaPlayer.playState != WMPLib.WMPPlayState.wmppsPlaying)
                                    axWindowsMediaPlayer.Ctlcontrols.play();
                            }
                        }
                        catch { }
                    else
                    {
                        axWindowsMediaPlayer.Ctlcontrols.pause();
                        while (axWindowsMediaPlayer.playState != WMPLib.WMPPlayState.wmppsPaused)
                        {
                            System.Threading.Thread.Sleep(500);
                            axWindowsMediaPlayer.Ctlcontrols.pause();
                        }
                    }*/
                }
            }
            catch { }
        }

        internal string UrlToName(string url)// => url.Remove(url.LastIndexOf('.')).Remove(0, url.LastIndexOf(initialFolderPath, StringComparison.InvariantCulture) + initialFolderPath.Length + 1);
        {
            if (string.IsNullOrEmpty(url))
                return "";
            bool hasExtension = false;
            foreach (string supportedExtension in SupportedExtensions)
                hasExtension = hasExtension || url.Contains(supportedExtension);

            if (hasExtension)
                url = url.Remove(url.LastIndexOf(".", StringComparison.InvariantCulture));
            if (url.Contains(@"\"))
                url = url.Remove(0, url.LastIndexOf(@"\", StringComparison.InvariantCulture) + 1);
            return url;
        }

        private int CheckPlaylistIndex()
        {
            try
            {
                for (int i = 0; i < urlPlaylist.Length; i++)
                    if (urlPlaylist[i] == axWindowsMediaPlayer.URL)
                    {
                        playlistIndex = i;
                        return i;
                    }
                return playlistIndex;
            }
            catch (Exception)
            {
                return playlistIndex;
            }
        }

        private void AxWindowsMediaPlayer_DoubleClickEvent(object sender, AxWMPLib._WMPOCXEvents_DoubleClickEvent e)
        {
            FullScreenBttn_Click(null, null);
        }

        private void axWindowsMediaPlayer_KeyDownEvent(object sender, AxWMPLib._WMPOCXEvents_KeyDownEvent e)
        {
            if (e.nKeyCode == 81)
                axWindowsMediaPlayer.Ctlcontrols.currentPosition = Math.Max(0, Math.Min(axWindowsMediaPlayer.currentMedia.duration, axWindowsMediaPlayer.Ctlcontrols.currentPosition - 10));
            else if (e.nKeyCode == 69)
                axWindowsMediaPlayer.Ctlcontrols.currentPosition = Math.Max(0, Math.Min(axWindowsMediaPlayer.currentMedia.duration, axWindowsMediaPlayer.Ctlcontrols.currentPosition + 10));
        }
    }
}