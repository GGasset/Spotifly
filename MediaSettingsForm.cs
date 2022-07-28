using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace Spotifly
{
    public partial class MediaSettingsForm : Form
    {
        internal List<string> OptionsStrs;
        internal List<bool> Options;
        private Size StartingSize = new Size(309, 86), RenamingSize = new Size(379, 133);
        internal bool textBoxMode;
        internal string optionPath;
        private Form1 PrincipalForm;

        public MediaSettingsForm(Form1 principalForm)
        {
            InitializeComponent();
            this.PrincipalForm = principalForm;
            SetRenamingMode(false);
            optionPath = string.Empty;

            OptionsStrs = new List<string>()
            {
                //"Add to queue",
                "Create folder",
                "Delete item",
                "Copy item",
                "Move item",
                "Rename item"
            };
            MediaOptionsComboBox.Text = OptionsStrs[0];

            Options = new List<bool>();
            foreach (var optionStr in OptionsStrs)
            {
                MediaOptionsComboBox.Items.Add(optionStr);
                Options.Add(false);
            }
        }

        private void MediaOptionsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!MediaOptionsCheckBox.Checked)
            {
                SetRenamingMode(false);
            }
            else if (GetSelectedOption() == "Create folder")
            {
                string folderName = PrincipalForm.folderPath;
                if (folderName.EndsWith("\\"))
                    folderName = folderName.Remove(folderName.LastIndexOf("\\"));
                folderName.Remove(0, folderName.LastIndexOf("\\"));

                SetRenamingMode(true, folderName);
                FileToRenameLabel.Text = "Create folder at";
            }

            ChangeCurrentOption(MediaOptionsComboBox.Text);
        }

        internal void SetMediaOptionsCheckBox(bool v)
        {
            MediaOptionsCheckBox.Checked = v;
        }

        private void MediaOptionsComboBox_TextChanged(object sender, EventArgs e)
        {
            if (textBoxMode)
                SetRenamingMode(false);
        }

        public void SetRenamingMode(bool v, string fileToRenameName = "")
        {
            if (v)
            {
                this.Size = RenamingSize;
            }
            else
            {
                this.Size = StartingSize;
                fileToRenameName = string.Empty;
            }

            FileToRenameLabel.Visible = FileToRenameNameLabel.Visible = FileRenameTextBox.Visible = ConfirmRenameButton.Visible = textBoxMode = v;
            FileToRenameNameLabel.Text = fileToRenameName;
        }

        private void ChangeCurrentOption(string option)
        {
            for (int i = 0; i < OptionsStrs.Count; i++)
            {
                Options[i] = option == OptionsStrs[i] && MediaOptionsCheckBox.Checked;
            }
        }

        public string GetSelectedOption()
        {
            if (!MediaOptionsCheckBox.Checked)
                return "None";

            return MediaOptionsComboBox.Text;
        }

        private void ConfirmRenameButton_Click(object sender, EventArgs e)
        {
            if (FileRenameTextBox.Text.Replace(" ", "") == "")
            {
                MessageBox.Show("Cannot do anything with empty text", "Error", MessageBoxButtons.OK);
                return;
            }

            if (GetSelectedOption() == "Rename item")
                RenameFile();
            else
                CreateFolder();

            SetRenamingMode(false);
            MediaOptionsCheckBox.Checked = false;
        }

        private void CreateFolder()
        {
            string path = $@"{PrincipalForm.folderPath}\{FileRenameTextBox.Text}\";
            Directory.CreateDirectory(path);
        }

        private void RenameFile()
        {

            bool isFile = false;
            foreach (var supportedExtension in PrincipalForm.SupportedExtensions)
                isFile = isFile || optionPath.EndsWith(supportedExtension);

            if (isFile)
            {
                string folderPath = optionPath.Remove(optionPath.LastIndexOf(@"\") + 1);
                //string fileName = PrincipalForm.UrlToName(fileToRename);
                string extension = optionPath.Remove(0, optionPath.LastIndexOf("."));
                File.Move(optionPath, $@"{folderPath}{FileRenameTextBox.Text}{extension}");
            }
        }

        private void FileRenameTextBox_TextChanged(object sender, EventArgs e)
        {
            FileRenameTextBox.Text = FileRenameTextBox.Text.Replace("\\", "").Replace("\"", " ").Replace("<", " ").Replace(">", " ").Replace("|", " ").Replace("...", " ").Replace("*", " ").Replace("/", " ")
                    .Replace("?", "").Replace("¿", "");
        }
    }
}
