using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Spotifly
{
    public partial class MediaSettingsForm : Form
    {
        internal List<string> OptionsStrs;
        internal List<bool> Options;
        private Size StartingSize = new Size(309, 86), RenamingSize = new Size(379, 133);
        internal bool RenamingMode;
        internal string fileToRename;
        private Form1 PrincipalForm;

        public MediaSettingsForm(Form1 principalForm)
        {
            InitializeComponent();
            this.PrincipalForm = principalForm;
            SetRenamingMode(false);
            fileToRename = string.Empty;

            OptionsStrs = new List<string>()
            {
                //"Add to queue",
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

            ChangeCurrentOption(MediaOptionsComboBox.Text);
        }

        private void MediaOptionsComboBox_TextChanged(object sender, EventArgs e)
        {
            if (RenamingMode)
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

            FileToRenameLabel.Visible = FileToRenameNameLabel.Visible = FileRenameTextBox.Visible = ConfirmRenameButton.Visible = RenamingMode = v;
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
                MessageBox.Show("Cannot rename to an Empty Name", "Error", MessageBoxButtons.OK);
                return;
            }

            bool isDirectory;
        }
    }
}
