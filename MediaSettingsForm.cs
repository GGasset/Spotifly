using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Spotifly
{
    public partial class MediaSettingsForm : Form
    {
        internal List<string> OptionsStr;
        internal List<bool> Options;
        private Size StartingSize = new Size(309, 86), RenamingSize = new Size(309, 133);
        internal bool RenamingMode;
        internal string fileToRename;

        public MediaSettingsForm()
        {
            InitializeComponent();
            this.Size = StartingSize;
            fileToRename = string.Empty;

            OptionsStr = new List<string>()
            {
                "Add to queue",
                "Delete file",
                "Copy file",
                "Move file",
                "Rename file"
            };
            MediaOptionsComboBox.Text = OptionsStr[0];

            Options = new List<bool>();
            foreach (var optionStr in OptionsStr)
            {
                MediaOptionsComboBox.Items.Add(optionStr);
                Options.Add(false);
            }
        }

        private void MediaOptionsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (RenamingMode)
            {
                ChangeRenamingMode(false);
            }

            ChangeCurrentOption(MediaOptionsComboBox.Text);
        }

        private void MediaOptionsComboBox_TextChanged(object sender, EventArgs e)
        {
            if (RenamingMode)
                ChangeRenamingMode(false);

            ChangeRenamingMode(MediaOptionsComboBox.Text == "Rename file");
        }

        public void ChangeRenamingMode(bool v)
        {
            if (v)
            {
                this.Size = RenamingSize;
            }
            else
            {
                this.Size = StartingSize;
                fileToRename = string.Empty;
            }

            FileToRenameLabel.Visible = FileToRenameName.Visible = FileRenameTextBox.Visible = ConfirmRenameButton.Visible = v;
            MediaOptionsComboBox.Size = FileRenameTextBox.Size;
        }

        private void ChangeCurrentOption(string option)
        {
            for (int i = 0; i < OptionsStr.Count; i++)
            {
                Options[i] = option == OptionsStr[i] && MediaOptionsCheckBox.Checked;
            }
        }

        public string GetSelectedOption()
        {
            if (!MediaOptionsCheckBox.Checked)
                return "None";

            return MediaOptionsComboBox.Text;
        }
    }
}
