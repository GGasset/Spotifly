using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Spotifly
{
    public partial class Form1
    {
        private MediaSettingsForm mediaSettingsForm;
        private bool addToQueue = false;
        private string initialFolderPath;
        private string[] filteredFilesMemory = Array.Empty<string>(), foldersMemory = Array.Empty<string>(), urlPlaylist;
        private string fileFilterMemory = "";

        private void MediaListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (!e.IsSelected)
                return;
            string pathWithoutExtension = $@"{folderPath}\{e.Item.Text}";
            bool isDirectory = Directory.Exists(pathWithoutExtension);
            string option;

            /*if (mediaSettingsForm.IsDisposed)
            {
                ChangeMedia(e.Item.Text);
                return;
            }
            else if ((option = mediaSettingsForm.GetSelectedOption()) == "None")
            {
                if (isDirectory)

                ChangeMedia(e.Item.Text);
                return;
            }

            if (option == "Add to queue")
            {
                if (!isDirectory)
                    AddToQueue(e.Item.Text);
                else
                {
                    GetFilteredFilesAndFolders(pathWithoutExtension, out string[] files, out _);
                    AddToQueue(files);
                }
                UpdateQueuedMediaListView();

                return;
            }*/



            if (willDelete)
            {
                DeleteFileBttn_Click(this, null);

                DialogResult selectedButton = MessageBox.Show(!isDirectory ? $"Are you sure you want to delete {e.Item.Text}?"
                    : $"Are you sure you want to delete {e.Item.Text}. IMPORTANT: this will delete all the items and folders inside.", "Important", MessageBoxButtons.YesNoCancel);

                if (selectedButton == DialogResult.Yes)
                {
                    if (isDirectory)
                    {
                        Directory.Delete(pathWithoutExtension, true);
                    }
                    else
                    {
                        string path = "";
                        string[] files = Directory.GetFiles(folderPath);
                        foreach (var file in files)
                        {
                            path = file;
                            if (UrlToName(path) == e.Item.Text)
                                break;
                        }
                        File.Delete(path);
                    }
                    MessageBox.Show($"{e.Item.Text} deleted!", "Finished deleting", MessageBoxButtons.OK);
                    return;
                }
            }
            else if (isDirectory)
            {
                ChangeDirectory(e.Item.Text);
            }
            else if (e.ItemIndex - foldersMemory.Length >= 0)
            {
                if (!addToQueue)
                {
                    ChangeMedia(e.Item.Text);
                }
                else
                {
                    AddToQueue(e.Item.Text);
                    UpdateQueuedMediaListView();
                }
            }
        }

        private void AddToQueue(string itemName)
        {
            priorityQueue.Enqueue(itemName);


            // TODO: Delete this if when finished
            if (!ToggleAddToQueueCheckBox.Checked)
            {
                EnqueueBttn_Click(this, null);
            }

            if (ClearFilterWhenMediaIsSelectedCheckBox.Checked)
            {
                SearchTxtBox.Text = string.Empty;
            }
        }

        private void AddToQueue(string[] filesToAdd)
        {
            foreach (var file in filesToAdd)
            {
                priorityQueue.Enqueue(file);
            }

            if (ClearFilterWhenMediaIsSelectedCheckBox.Checked)
                SearchTxtBox.Text = string.Empty;
        }

        private void ChangeDirectory(string folderName)
        {
            this.folderPath = $@"{this.folderPath}\{folderName}";
            BackBttn.Visible = folderName != initialFolderPath;
            GetFilteredFilesAndFolders(this.folderPath, out string[] files, out string[] folders);
            SetListViewItems(files, folders);
        }

        private void ChangeMedia(string mediaName)
        {
            string previousMedia = urlPlaylist[playlistIndex];

            isQueued = false;
            if (shuffle)
                PlayFileInUnshuffled(mediaName);
            else
                //PlayFile(e.ItemIndex - playlistIndex - foldersMemory.Length, true);
                PlayFile(mediaName, true);

            if (ResizeForMediaCheckBox.Checked)
            {
                SetFormSizeForCurrentMedia();
                FormBorderStyle = FormBorderStyle.FixedSingle;
            }
            if (ClearFilterWhenMediaIsSelectedCheckBox.Checked)
            {
                SearchTxtBox.Text = string.Empty;
            }
            if (ChangePanelWhenMediaIsSelectedCheckBox.Checked)
            {
                SetActivePanel(0);
            }

            currentUrlFolder = folderPath;

            bool isDifferentFolderFromLastPlayedMediaFolder = GetFolderFromUrl(previousMedia) != currentUrlFolder;
            if (isDifferentFolderFromLastPlayedMediaFolder)
            {
                GetFilteredFilesAndFolders(currentUrlFolder, out string[] files, out _);
                urlPlaylist = files;
                CheckPlaylistIndex();
            }

            Focus();
        }

        private void MediaSettingsBttn_Click(object sender, EventArgs e)
        {
            if (!mediaSettingsForm.IsDisposed)
            {
                mediaSettingsForm.Visible = true;
                mediaSettingsForm.BringToFront();
                return;
            }

            mediaSettingsForm = new MediaSettingsForm();

            mediaSettingsForm.Show();
            mediaSettingsForm.FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        private void MediaListView_DrawMedia(string fileFilter = null, bool unshuffleNeed = false)
        {
            string[] filesUrls, folders;
            GetFilteredFilesAndFolders(folderPath, out filesUrls, out folders);

            if (fileFilter == null)
            {
                fileFilter = fileFilterMemory;
            }

            if (!ArrayElementsEqual(AppendArrays(folders, filesUrls), AppendArrays(foldersMemory, filteredFilesMemory)) || fileFilter != fileFilterMemory)
            {
                SetListViewItems(filesUrls, folders, fileFilter);
                if (currentUrlFolder == folderPath)
                    urlPlaylist = filesUrls;
                if (shuffle)
                    ShufflePlaylist();

                BackupInMemory(folders, filesUrls);
                fileFilterMemory = fileFilter;
                CheckPlaylistIndex();
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
            folder = Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);
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

        string[] supportedExtensions = ".WEBM .MP4 .WAV".ToLower(System.Globalization.CultureInfo.InvariantCulture).Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
        private string[] FilterFiles(string[] files)
        {
            string[] filteredFiles;
            bool[] compatibleFilesIndex = new bool[files.Length];
            int compatibleFiles = 0;
            for (int i = 0; i < files.Length; i++)
            {
                bool compatible = false;
                for (int j = 0; j < supportedExtensions.Length && !compatible; j++)
                    if (files[i].Contains(supportedExtensions[j]))
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

        private void SetListViewItems(string[] files, string[] folders, string fileFilter = "")
        {
            ImageList icons = new ImageList();

            icons.Images.Add(Properties.Resources.FolderImage);
            icons.Images.Add(Properties.Resources.video_icon);

            var folderItems = new ListViewItem[folders.Length];
            for (int i = 0; i < folders.Length; i++)
            {
                folderItems[i] = new ListViewItem(folders[i].Remove(0, folders[i].LastIndexOf('\\') + 1), 0);
            }

            string[] filteredFiles = FilterFilesByFilter(files, fileFilter);
            var fileItems = new ListViewItem[filteredFiles.Length];
            for (int i = 0; i < filteredFiles.Length; i++)
            {
                fileItems[i] = new ListViewItem(UrlToName(filteredFiles[i]), 1);
            }

            folderLabel.Text = folderPath;
            SongCountLabel.Text = $"Media Files Count: {files.Length}";

            if (fileFilter != string.Empty)
            {
                SongCountLabel.Text += $" and {filteredFiles.Length} filtered files";
            }

            MediaListView.SmallImageList = icons;
            MediaListView.Clear();
            MediaListView.Items.AddRange(folderItems);
            MediaListView.Items.AddRange(fileItems);
        }

        private string[] FilterFilesByFilter(string[] files, string filter)
        {
            List<string> filteredFiles = new List<string>();
            filter = filter.ToLowerInvariant();
            string[] filterWords = filter.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string fileName in files)
            {
                string file = fileName.ToLowerInvariant();
                string fileVariant = file.Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u");
                bool passesFilter = file.Contains(filter) || fileVariant.Contains(filter);

                bool containsAllWords = false;
                foreach (string filterWord in filterWords)
                {
                    if (file.Contains(filterWord) || fileVariant.Contains(filterWord))
                    {
                        containsAllWords = true;
                    }
                    else
                    {
                        containsAllWords = false;
                        break;
                    }
                }

                if (passesFilter || containsAllWords)
                {
                    filteredFiles.Add(fileName);
                }
            }
            return filteredFiles.ToArray();
        }

        private void SearchTxtBox_TextChanged(object sender, EventArgs e)
        {
            MediaListView_DrawMedia(SearchTxtBox.Text);
        }

        private void EnqueueBttn_Click(object sender, EventArgs e)
        {
            addToQueue = !addToQueue;

            if (!addToQueue)
            {
                EnqueueBttn.Text = "Add to Queue";
            }
            else
            {
                EnqueueBttn.Text = "Add to Queue t";
            }
        }

        private bool willDelete = false;

        private void DeleteFileBttn_Click(object sender, EventArgs e)
        {
            if (willDelete)
            {
                willDelete = false;
                DeleteFileBttn.Text = "Delete";
                return;
            }

            var pressedButton = MessageBox.Show("Are you sure you want to delete a File?", "Alert", MessageBoxButtons.YesNoCancel);
            if (pressedButton == DialogResult.Yes)
            {
                willDelete = true;
                DeleteFileBttn.Text = "Will Delete";
            }
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

        private void ClearFilterBttn_Click(object sender, EventArgs e)
        {
            SearchTxtBox.Text = string.Empty;
        }

        private string GetFolderFromUrl(string mediaUrl)
        {
            mediaUrl = mediaUrl.Remove(mediaUrl.LastIndexOf(@"\"));
            return mediaUrl;
        }

        private void DownloadedMediaPanel_VisibleChanged(object sender, EventArgs e)
        {
            //MediaListView.Size = new Size(DownloadedMediaPanel.Width, DownloadedMediaPanel.Height - 25);
        }

        private void DownloadedMediaPanel_Resize(object sender, EventArgs e)
        {
            //MediaListView.Size = new Size(DownloadedMediaPanel.Width, DownloadedMediaPanel.Height - 25);
        }
    }
}