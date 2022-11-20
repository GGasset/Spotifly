using System;
using System.IO;
using System.Windows.Forms;

namespace Spotifly
{
    public partial class Form1
    {
        private void AboutBttn_Click(object sender, EventArgs e)
        {
            string about = (string)table["about"];
            MessageBox.Show(about.Remove(about.IndexOf(" About", StringComparison.InvariantCulture)), about.Remove(0, about.IndexOf("About", StringComparison.InvariantCulture)));
        }

        private void ChangeInitialFolderBttn_Click(object sender, EventArgs e)
        {
            string previousFolderPath = initialFolderPath;
            // Ask and change initial folder path, optionally create folder,
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog
            {
                ShowNewFolderButton = true,
            };
            folderBrowser.ShowDialog(this);

            initialFolderPath = folderBrowser.SelectedPath;
            folderPath = folderBrowser.SelectedPath;

            var result = MessageBox.Show(this, "Do you want to move your media files and folders into the new folder?", "Move files", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                MessageBox.Show($"Now {AppName} is not going to respond while moving files, please wait or you will loose some of your files.");
                MoveFiles(false, previousFolderPath, previousFolderPath, folderPath, folderPath);

                string[] folders = Directory.GetDirectories(previousFolderPath, "*", SearchOption.AllDirectories);
                for (int i = folders.Length - 1; i >= 0; i--)
                {
                    Directory.Delete(folders[i]);
                }

                MessageBox.Show("FInished moving files!");
            }
            else
            {
                result = MessageBox.Show(this, "Do you want to copy your media files and folders into the new folder?", "Copy files", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    MessageBox.Show($"Now {AppName} is not going to respond while copying files, please wait or all your files won't be copied.");
                    MoveFiles(true, previousFolderPath, previousFolderPath, folderPath, folderPath);
                    MessageBox.Show("Finished copying files!");
                }
            }

            queuedMedia.Clear();
            currentUrlFolder = folderBrowser.SelectedPath;
            SearchTxtBox.Text = "";
            MediaListView_DrawMedia();
            playlistIndex = 0;
        }

        private void MoveFiles(bool copyInsteadOfMove, string baseFolderPath, string folderPath, string toBaseFolderPath, string toCurrentFolderPath)
        {
            GetFilteredFilesAndFolders(folderPath, out string[] files, out string[] folders);

            for (int i = 0; i < files.Length; i++)
            {
                string currentFile = files[i];
                string file = currentFile.Remove(0, currentFile.LastIndexOf(@"\") + 1);
                string toFilePath = toCurrentFolderPath;
                toFilePath += @"\";
                toFilePath += file;
                File.Copy(currentFile, toFilePath, true);
                if (!copyInsteadOfMove)
                    File.Delete(currentFile);
            }

            foreach (string folder in folders)
            {
                string newCurrentFolderPath = toCurrentFolderPath;
                string fromFolderName = folder.Remove(0, folder.LastIndexOf(@"\") + 1);
                newCurrentFolderPath += @"\" + fromFolderName;
                Directory.CreateDirectory(newCurrentFolderPath);
                MoveFiles(copyInsteadOfMove, baseFolderPath, folder, toBaseFolderPath, newCurrentFolderPath);
            }
        }

        private void ShowTutorialBttn_Click(object sender, EventArgs e)
        {
            ShowTutorial();
        }

        private void ShowTutorial()
        {
            string tutorialHeaderTxt = "Tutorial";
            DialogResult dialogResult;
            dialogResult = MessageBox.Show("After clicking media player, you can press Q or E (q, e) so you can rewind or advance 10 seconds.", tutorialHeaderTxt, MessageBoxButtons.OKCancel);
            if (dialogResult == DialogResult.Cancel)
                return;
        }

        private void ThemeSelectionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeTheme(ThemeSelectionComboBox.SelectedItem.ToString());
        }
    }
}