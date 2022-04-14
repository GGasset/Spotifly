using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Spotifly
{
    public partial class Form1
    {
        private bool addToQueue = false;
        private readonly string initialFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) + $@"\{AppName}";
        private string[] filteredFilesMemory = Array.Empty<string>(), foldersMemory = Array.Empty<string>(), urlPlaylist;


        private void MediaListView_DrawMedia(bool unshuffleNeed = false)
        {
            string[] filesUrls, folders;
            GetFilteredFilesAndFolders(folderPath, out filesUrls, out folders);

            if (!ArrayElementsEqual(AppendArrays(folders, filesUrls), AppendArrays(foldersMemory, filteredFilesMemory)))
            {
                SetListViewItems(filesUrls, folders);
                if (currentUrlFolder == folderPath)
                    urlPlaylist = filesUrls;
                if (shuffle)
                    ShufflePlaylist();

                BackupInMemory(folders, filesUrls);
            }




            if (unshuffleNeed)//Unshuffle Playlist
                UnshufflePlaylist(filesUrls);

            void UnshufflePlaylist(string[] unshuffledPlaylist)
            {
                urlPlaylist = unshuffledPlaylist;
                CheckPlaylistIndex();
            }
        }

        private void GetFilteredFilesAndFolders(string path, out string[] files, out string[] folder)
        {
            if (path == null)
                path = folderPath;
            files = FilterFiles(Directory.GetFiles(path));
            folder = Directory.GetDirectories(path);
        }

        /// <summary>
        /// This is used to see if a reload media list view items is needed
        /// </summary>
        /// <param name="folders"></param>
        /// <param name="filteredFiles"></param>
        private void BackupInMemory(string[] folders, string[] filteredFiles)
        {
            foldersMemory = folders;
            filteredFilesMemory = filteredFiles;
        }

        private string[] FilterFiles(string[] files)
        {
            string[] supportedExtensions = ".WEBM .MPG .MP2 .MPEG .MPE .MPV .OGG .MP4 .M4P .M4V .AVI .WMV .WAV".ToLower(System.Globalization.CultureInfo.InvariantCulture).Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            string[] filteredFiles;
            bool[] compatibleFilesIndex = new bool[files.Length];
            int compatibleFiles = 0;
            for (int i = 0; i < files.Length; i++)
            {
                bool compatible = false;
                for (int ii = 0; ii < supportedExtensions.Length && !compatible; ii++)
                    if (files[i].Contains(supportedExtensions[ii]))
                    {
                        compatible = true;
                        compatibleFilesIndex[i] = true;
                        compatibleFiles++;
                    }
            }
            filteredFiles = new string[compatibleFiles];
            for (int i = 0; i < compatibleFiles; i++)
                if (compatibleFilesIndex[i])
                    filteredFiles[i] = files[i];
            return filteredFiles;
        }

        private string[] AppendArrays(string[] ar1, string[] ar2)
        {
            string[] output = new string[ar1.Length + ar2.Length];
            for (int i = 0; i < ar1.Length; i++)
                output[i] = ar1[i];
            for (int i = ar1.Length; i < ar1.Length + ar2.Length; i++)
                output[i] = ar2[i - ar1.Length];
            return output;
        }

        private bool ArrayElementsEqual(string[] ar0, string[] ar1)
        {
            bool modifyNeed = ar0.Length != ar1.Length;

            if (!modifyNeed)
                foreach (string url in ar0)//detect if any element of filtered files doesnt exist in memory (any file was added)
                {
                    int numberOfConcurrences = 0;
                    foreach (string urlMemoryUrl in ar1)
                        numberOfConcurrences += 1 * Convert.ToInt32(url == urlMemoryUrl);
                    modifyNeed = numberOfConcurrences == 0;
                    if (modifyNeed)
                        break;
                }
            if (!modifyNeed)
                foreach (string urlMemoryUrl in filteredFilesMemory)//detect if any element of memory doesnt exist in filtered files (any file was deleted)
                {
                    int numberOfConcurrences = 0;
                    foreach (string url in ar0)
                        numberOfConcurrences += 1 * Convert.ToInt32(url == urlMemoryUrl);
                    modifyNeed = numberOfConcurrences == 0;
                    if (modifyNeed)
                        break;
                }
            return !modifyNeed;
        }

        private void SetListViewItems(string[] files, string[] folders)
        {
            ImageList icons = new ImageList();
            MediaListView.Clear();
            icons.Images.Add(Properties.Resources.FolderImage);
            for (int i = 0; i < folders.Length; i++)
                MediaListView.Items.Add(folders[i].Remove(0, folders[i].LastIndexOf('\\') + 1), 0);
            if (files.Length > 0)
                icons.Images.Add(Icon.ExtractAssociatedIcon(files[0]));
            for (int i = 0; i < files.Length; i++)
                MediaListView.Items.Add(files[i]
                    .Remove(files[i].LastIndexOf(".", StringComparison.InvariantCulture))
                    .Remove(0, files[i].LastIndexOf('\\') + 1), 1);

            folderLabel.Text = folderPath;
            SongCountLabel.Text = $"Media Files Count: {files.Length}";

            MediaListView.SmallImageList = icons;
        }

        private void MediaListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
                if (Directory.Exists($@"{folderPath}\{e.Item.Text}"))
                {
                    folderPath = $@"{folderPath}\{e.Item.Text}";
                    BackBttn.Visible = folderPath != initialFolderPath;
                    GetFilteredFilesAndFolders(folderPath, out string[] files, out string[] folders);
                    SetListViewItems(files, folders);
                }
                else if (e.ItemIndex - foldersMemory.Length >= 0)
                {
                    if (!addToQueue)
                    {
                        if (shuffle)
                            PlayFileInUnshuffled(e.Item.Text);
                        else
                            //PlayFile(e.ItemIndex - playlistIndex - foldersMemory.Length, true);
                            PlayFile(e.Item.Text, true);

                        if (ResizeForMediaCheckBox.Checked)
                        {
                            SetFormSizeForCurrentMedia();
                            FormBorderStyle = FormBorderStyle.FixedSingle;
                        }
                        SetActivePanel(0);
                        Focus();
                        GetColorsForTheme(currentTheme, out _, out _, out _, out Color buttonColor, out _);
                        PlayBttn.Image = SubstituteNotBlankFromImage(Properties.Resources.Pause, buttonColor);
                    }
                    else
                    {
                        priorityQueue.Enqueue(e.ItemIndex - foldersMemory.Length);
                        addToQueue = false;
                        EnqueueBttn.Text = EnqueueBttn.Text.Replace(" t", "");
                    }
                }
        }

        private void EnqueueBttn_Click(object sender, EventArgs e)
        {
            if (addToQueue)
            {
                EnqueueBttn.Text = "Add to Queue";
            }
            else
            {
                EnqueueBttn.Text = "Add to Queue t";
            }
            addToQueue = !addToQueue;
        }

        private void BackBttn_Click(object sender, EventArgs e)
        {
            folderPath = folderPath.Remove(folderPath.LastIndexOf(@"\", StringComparison.InvariantCulture));
            BackBttn.Visible = folderPath != initialFolderPath;
            GetFilteredFilesAndFolders(folderPath, out string[] files, out string[] folders);
            SetListViewItems(files, folders);
        }

        private void OpenCurrentFldrBttn_Click(object sender, EventArgs e)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                Arguments = folderPath,
                FileName = "explorer.exe"
            };
            Process.Start(startInfo);
        }

        private void DownloadedMediaPanel_VisibleChanged(object sender, EventArgs e)
        {
            MediaListView.Size = new Size(DownloadedMediaPanel.Width, DownloadedMediaPanel.Height - 25);
        }

        private void DownloadedMediaPanel_Resize(object sender, EventArgs e)
        {
            MediaListView.Size = new Size(DownloadedMediaPanel.Width, DownloadedMediaPanel.Height - 25);
        }
    }
}
