using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Spotifly
{
    public partial class Form1
    {
        private Queue<string> priorityQueue;
        private bool isPlaying = false;

        private void WindowsMediaPlayer_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            //Media Ended
            if (e.newState == 8)
            {
                bool fScreen = axWindowsMediaPlayer.fullScreen;
                PlayFile(1);
                if (fScreen)
                {
                    axWindowsMediaPlayer.uiMode = "mini";
                    axWindowsMediaPlayer.fullScreen = true;
                }
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

        private void PlayFile(int positionsToAdvance)
        {
            PlayFile(positionsToAdvance, isPlaying);
        }

        private void PlayFile(int positionsToAdvance, bool startPlaying)
        {
            if (priorityQueue.Count > 0 && positionsToAdvance == 1)
            {
                Task.Run(() => PlayFileInUnshuffled(priorityQueue.Dequeue()));
            }
            else
            {
                if (playlistIndex != -1)
                    Task.Run(() => SetURL(urlPlaylist[playlistIndex = AdvanceIndexesOnPlaylists(playlistIndex, positionsToAdvance, urlPlaylist.Length)], startPlaying));
                else
                    Task.Run(() => SetURL(urlPlaylist[playlistIndex = positionsToAdvance], startPlaying));
                if (!loading)
                    CurrentMediaTxtBox.Text = GetCurrentMediaName();
            }
        }

        private void PlayFileInUnshuffled(string name)
        {
            try
            {
                string[] unshuffled;
                GetFilteredFilesAndFolders(folderPath, out unshuffled, out _);
                int index = -1;

                for (int i = 0; i < unshuffled.Length; i++)
                {
                    if (unshuffled[i].Contains(name))
                    {
                        index = i;
                        break;
                    }
                }

                if (index != -1)
                {
                    PlayFileInUnshuffled(index);
                }
            }
            catch (Exception)
            {

            }
        }

        private void PlayFileInUnshuffled(int index)
        {
            try
            {
                string[] unshuffled;
                GetFilteredFilesAndFolders(folderPath, out unshuffled, out _);
                PlayFile(unshuffled[index]);
            }
            catch (Exception)
            {

            }
        }

        private void PlayFile(string URL, bool isName = false)
        {
            if (isName)
            {
                for (int i = 0; i < urlPlaylist.Length; i++)
                {
                    if (urlPlaylist[i].Contains(URL))
                        URL = urlPlaylist[i];
                }
            }
            Task.Run(() => SetURL(URL, isPlaying));
            for (int i = 0; i < urlPlaylist.Length; i++)
                 if (urlPlaylist[i] == axWindowsMediaPlayer.URL)
                    playlistIndex = i;

            if (!loading)
                CurrentMediaTxtBox.Text = GetCurrentMediaName();
        }

        private void SetURL(string URL, bool startPlaying)
        {
            try
            {
                if (URL != axWindowsMediaPlayer.URL)
                {
                    currentUrlFolder = URL.Remove(URL.LastIndexOf('\\'));
                    axWindowsMediaPlayer.URL = URL;
                    axWindowsMediaPlayer.Ctlcontrols.currentPosition = 0;
                    //axWindowsMediaPlayer.Ctlcontrols.play();
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

        private string GetCurrentMediaName()
        {
            return UrlToName(urlPlaylist[playlistIndex]);
        }

        private string UrlToName(string url)// => url.Remove(url.LastIndexOf('.')).Remove(0, url.LastIndexOf(initialFolderPath, StringComparison.InvariantCulture) + initialFolderPath.Length + 1);
        {
            if (string.IsNullOrEmpty(url))
                return "";
            url = url.Remove(url.LastIndexOf('.'));
            url = url.Remove(0, url.LastIndexOf(initialFolderPath, StringComparison.InvariantCulture) + initialFolderPath.Length + 1);
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
                return 0;
            }
        }

        private void AxWindowsMediaPlayer_DoubleClickEvent(object sender, AxWMPLib._WMPOCXEvents_DoubleClickEvent e)
        {
            axWindowsMediaPlayer.fullScreen = false;
        }
    }
}