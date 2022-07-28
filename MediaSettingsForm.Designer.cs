namespace Spotifly
{
    partial class MediaSettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MediaSettingsForm));
            this.MediaOptionsCheckBox = new System.Windows.Forms.CheckBox();
            this.MediaOptionsComboBox = new System.Windows.Forms.ComboBox();
            this.ItemInfoTextBox = new System.Windows.Forms.TextBox();
            this.ConfirmRenameButton = new System.Windows.Forms.Button();
            this.PathToModifyLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // MediaOptionsCheckBox
            // 
            this.MediaOptionsCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.MediaOptionsCheckBox.AutoSize = true;
            this.MediaOptionsCheckBox.Location = new System.Drawing.Point(339, 16);
            this.MediaOptionsCheckBox.Name = "MediaOptionsCheckBox";
            this.MediaOptionsCheckBox.Size = new System.Drawing.Size(15, 14);
            this.MediaOptionsCheckBox.TabIndex = 0;
            this.MediaOptionsCheckBox.UseVisualStyleBackColor = true;
            this.MediaOptionsCheckBox.CheckedChanged += new System.EventHandler(this.MediaOptionsCheckBox_CheckedChanged);
            // 
            // MediaOptionsComboBox
            // 
            this.MediaOptionsComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MediaOptionsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.MediaOptionsComboBox.FormattingEnabled = true;
            this.MediaOptionsComboBox.Location = new System.Drawing.Point(12, 12);
            this.MediaOptionsComboBox.Name = "MediaOptionsComboBox";
            this.MediaOptionsComboBox.Size = new System.Drawing.Size(321, 21);
            this.MediaOptionsComboBox.TabIndex = 1;
            this.MediaOptionsComboBox.TextChanged += new System.EventHandler(this.MediaOptionsComboBox_TextChanged);
            // 
            // ItemInfoTextBox
            // 
            this.ItemInfoTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ItemInfoTextBox.Location = new System.Drawing.Point(12, 39);
            this.ItemInfoTextBox.Name = "FileRenameTextBox";
            this.ItemInfoTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.ItemInfoTextBox.Size = new System.Drawing.Size(321, 20);
            this.ItemInfoTextBox.TabIndex = 2;
            this.ItemInfoTextBox.Visible = false;
            this.ItemInfoTextBox.TextChanged += new System.EventHandler(this.FileRenameTextBox_TextChanged);
            // 
            // ConfirmRenameButton
            // 
            this.ConfirmRenameButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ConfirmRenameButton.Location = new System.Drawing.Point(12, 65);
            this.ConfirmRenameButton.Name = "ConfirmRenameButton";
            this.ConfirmRenameButton.Size = new System.Drawing.Size(75, 23);
            this.ConfirmRenameButton.TabIndex = 3;
            this.ConfirmRenameButton.Text = "Confirm";
            this.ConfirmRenameButton.UseVisualStyleBackColor = true;
            this.ConfirmRenameButton.Visible = false;
            this.ConfirmRenameButton.Click += new System.EventHandler(this.ConfirmRenameButton_Click);
            // 
            // PathToModifyLabel
            // 
            this.PathToModifyLabel.AutoSize = true;
            this.PathToModifyLabel.Location = new System.Drawing.Point(93, 70);
            this.PathToModifyLabel.Name = "FileToRenameNameLabel";
            this.PathToModifyLabel.Size = new System.Drawing.Size(258, 13);
            this.PathToModifyLabel.TabIndex = 5;
            this.PathToModifyLabel.Text = "Media file URL to name passed as function from URL";
            // 
            // MediaSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(363, 94);
            this.Controls.Add(this.PathToModifyLabel);
            this.Controls.Add(this.ConfirmRenameButton);
            this.Controls.Add(this.ItemInfoTextBox);
            this.Controls.Add(this.MediaOptionsComboBox);
            this.Controls.Add(this.MediaOptionsCheckBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MediaSettingsForm";
            this.ShowInTaskbar = false;
            this.Text = "Options for media";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox MediaOptionsCheckBox;
        private System.Windows.Forms.ComboBox MediaOptionsComboBox;
        private System.Windows.Forms.TextBox ItemInfoTextBox;
        private System.Windows.Forms.Button ConfirmRenameButton;
        private System.Windows.Forms.Label PathToModifyLabel;
    }
}