using System;
using System.Drawing;
using System.Windows.Forms;
using WMPLib;

namespace Spotifly
{
    public partial class Form1
    {
        private void PlayBttn_Click(object sender, EventArgs e)
        {
            if (loading)
                return;
            GetColorsForTheme(currentTheme, out _, out _, out _, out Color ButtonColor, out _);

            isPlaying = !isPlaying;

            if (isPlaying)
            {
                PlayBttn.Image = SubstituteNotBlankFromImage(Properties.Resources.Pause, ButtonColor);
            }
            else
            {
                PlayBttn.Image = SubstituteNotBlankFromImage(Properties.Resources.Playy, ButtonColor);
            }

            //if (axWindowsMediaPlayer.playState == WMPPlayState.wmppsReady || axWindowsMediaPlayer.playState == WMPPlayState.wmppsPaused || axWindowsMediaPlayer.playState == WMPPlayState.wmppsStopped ||
            //    axWindowsMediaPlayer.playState == WMPPlayState.wmppsUndefined)
            //{
            //    axWindowsMediaPlayer.Ctlcontrols.play();
            //}
            //else if (axWindowsMediaPlayer.playState != WMPPlayState.wmppsScanForward && axWindowsMediaPlayer.playState != WMPPlayState.wmppsScanReverse)
            //{
            //    axWindowsMediaPlayer.Ctlcontrols.pause();
            //}
        }

        private void ProgressBar_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && !loading)
            {
                double percentage = (double)(e.X) / ElapsedTimeBarPictureBox.Width * 100;
                axWindowsMediaPlayer.Ctlcontrols.currentPosition = axWindowsMediaPlayer.currentMedia.duration / 100 * percentage;
                SetProgressBarValueForCurrentMediaPos();
            }
        }

        private void PrevMediaBttn_Click(object sender, EventArgs e)
        {
            if (loading)
                return;

            //if the current position is less than 5 seconds
            if (axWindowsMediaPlayer.Ctlcontrols.currentPosition <= 5)
            {
                PlayFile(-1);
            }
            else
                axWindowsMediaPlayer.Ctlcontrols.currentPosition = 0;
        }

        private void NextMediaBttn_Click(object sender, EventArgs e)
        {
            if (loading)
                return;
            PlayFile(1);
            ElapsedTimeBarPictureBox.Image = null;
        }

        private void ShuffleBttn_Click(object sender, EventArgs e)
        {
            if (loading)
                return;
            SetShuffleBttn(!shuffle);
        }

        private void SetShuffleBttn(bool value)
        {
            Color initialBackColor = ShuffleBttn.Parent.BackColor;
            if (shuffle = value)
            {
                int nR, nG, nB;
                Color midARGB = Color.FromArgb(127, 127, 127);
                nR = ShiftValueTowards(initialBackColor.R, midARGB.R);
                nG = ShiftValueTowards(initialBackColor.G, midARGB.G);
                nB = ShiftValueTowards(initialBackColor.B, midARGB.B);
                ShuffleBttn.BackColor = Color.FromArgb(nR, nG, nB);

                int ShiftValueTowards(int initialValue, int valueToShiftTowards) => initialValue + (valueToShiftTowards - initialValue) / 4;
            }
            else
            {
                ShuffleBttn.BackColor = initialBackColor;
            }
            if (value)
                ShufflePlaylist();
            else
                MediaListView_DrawMedia(null, true);
        }

        private void ShufflePlaylist()
        {
            try
            {
                for (int i = 0; i < urlPlaylist.Length; i++)//shuffle Playlist
                {
                    i += 1 * Convert.ToByte(i == playlistIndex);
                    int rand = -1;
                    while (rand == -1 || rand == playlistIndex)
                        rand = random.Next(i, urlPlaylist.Length);
                    string newUrl = urlPlaylist[rand];
                    urlPlaylist[rand] = urlPlaylist[i];
                    urlPlaylist[i] = newUrl;
                }
            }
            catch
            { }
        }

        private void FullScreenBttn_Click(object sender, EventArgs e)
        {
            if (loading)
                return;
            try
            {
                if (!axWindowsMediaPlayer.fullScreen)
                {
                    axWindowsMediaPlayer.uiMode = "mini";
                    axWindowsMediaPlayer.fullScreen = true;
                }
                else
                {
                    axWindowsMediaPlayer.fullScreen = false;
                    axWindowsMediaPlayer.uiMode = "none";
                }
            }
            catch (Exception)
            { }
        }

        private void VolumeTrackBar_Scroll(object sender, EventArgs e)
        {
            axWindowsMediaPlayer.settings.volume = VolumeTrackBar.Value;
        }

        private void ElapsedTimeLabel_MouseClick(object sender, MouseEventArgs e)
        {
            if (loading)
                return;
            showRemainingTimeInElapsed = !showRemainingTimeInElapsed;
            ElapsedTimeLabel.Text = showRemainingTimeInElapsed ? GetRemainingTimeString() : axWindowsMediaPlayer.Ctlcontrols.currentPositionString;
        }

        private string GetRemainingTimeString()
        {
            string output = "-";
            int remainingSeconds = Convert.ToInt32(axWindowsMediaPlayer.currentMedia.duration - axWindowsMediaPlayer.Ctlcontrols.currentPosition);
            int remainingMinutes = remainingSeconds / 60;
            int remainingHours = remainingMinutes / 60;
            remainingSeconds -= remainingMinutes * 60;
            remainingMinutes -= remainingHours * 60;
            if (remainingHours >= 10)
                output += remainingHours.ToString() + ":";
            else if (remainingHours != 0)
                output += $"0{remainingHours}:";
            if (remainingMinutes >= 10)
                output += remainingMinutes.ToString() + ":";
            else
                output += $"0{remainingMinutes}:";
            if (remainingSeconds >= 10)
                output += remainingSeconds.ToString();
            else
                output += $"0{remainingSeconds}";
            return output;
            //return $"-{(remainingMinutes < 10 ? $"0{remainingMinutes}" : $"{remainingMinutes}")}:{(remainingSeconds < 10 ? $"0{remainingSeconds}" : $"{remainingSeconds}")}";// Doesn't show remaining hours if needed
        }

        private Brush progressBarBrush = Brushes.White;

        private void SetProgressBarValueForCurrentMediaPos()
        {
            Bitmap bitmap = new Bitmap(ElapsedTimeBarPictureBox.Width, ElapsedTimeBarPictureBox.Height);
            Graphics graphics = Graphics.FromImage(bitmap);
            int pixels;
            try
            {
                pixels = Convert.ToInt32(ElapsedTimeBarPictureBox.Width * axWindowsMediaPlayer.Ctlcontrols.currentPosition / axWindowsMediaPlayer.currentMedia.duration);
            }
            catch (NullReferenceException)
            {
                bitmap.Dispose();
                graphics.Dispose();
                return;
            }
            GetColorsForTheme(currentTheme, out _, out _, out _, out _, out progressBarBrush);
            graphics.FillRectangle(progressBarBrush, 0, 0, pixels, ElapsedTimeBarPictureBox.Height);
            ElapsedTimeBarPictureBox.Image = new Bitmap(bitmap);
            bitmap.Dispose();
            graphics.Dispose();
        }
    }
}