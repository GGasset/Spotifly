using System;
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

        private void ThemeSelectionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeTheme(ThemeSelectionComboBox.SelectedItem.ToString());
        }
    }
}
