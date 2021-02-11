using System;
using System.Drawing;
using System.Windows.Forms;

namespace Spotifly
{
    public partial class Form1
    {
        private class DarkTheme
        {
            public const string ThemeString = "DarkTheme";

            public readonly static Color
                BackColor = Color.FromArgb(18, 18, 18),
                ForeColor = Color.White,
                ControlPanelBackColor = Color.FromArgb(40, 40, 40),
                HighlightedForeColor = SystemColors.ControlDark,
                ButtonColor = Color.FromArgb(179, 179, 179);

            public readonly static Brush
                progressBarBrush = Brushes.LightGray;
        }

        private class LightTheme
        {
            public const string ThemeString = "LightTheme";

            public readonly static Color
                BackColor = SystemColors.ControlLightLight,
                ForeColor = SystemColors.ControlText,
                ControlPanelBackColor = SystemColors.Control,
                HighlightedForeColor = SystemColors.ControlDarkDark,
                ButtonColor = Color.Black;

            public readonly static Brush
                progressBarBrush = Brushes.Black;
        }

        private string currentTheme;

        public static string[] GetThemes() => new string[] { LightTheme.ThemeString, DarkTheme.ThemeString };

        internal void ChangeTheme(string theme)// only works for my app
        {
            GetColorsForTheme(currentTheme = theme, out Color backColor, out Color foreColor, out _, out Color buttonColor, out _);
            ChangeFormAndControlsColor(this, backColor, foreColor);

            PrevMediaBttn.Image = SubstituteNotBlankFromImage(PrevMediaBttn.Image, buttonColor);
            PlayBttn.Image = SubstituteNotBlankFromImage(PlayBttn.Image, buttonColor);
            NextMediaBttn.Image = SubstituteNotBlankFromImage(NextMediaBttn.Image, buttonColor);
            ShuffleBttn.Image = SubstituteNotBlankFromImage(ShuffleBttn.Image, buttonColor);

            switch (theme)
            {
                case DarkTheme.ThemeString:
                    ChangeControlAndAllChildControlsColor(ControlPanel, DarkTheme.ControlPanelBackColor, foreColor);
                    break;

                case LightTheme.ThemeString:
                    ChangeControlAndAllChildControlsColor(ControlPanel, LightTheme.ControlPanelBackColor, foreColor);
                    break;

                default:
                    break;
            }
            SetShuffleBttn(shuffle);
        }

        private static Image SubstituteNotBlankFromImage(Image image, Color newColor)
        {
            Color color;
            Bitmap bitmap = new Bitmap(image);
            for (int x = 0; x < bitmap.Width; x++)
                for (int y = 0; y < bitmap.Height; y++)
                {
                    color = bitmap.GetPixel(x, y);
                    if (color.A != 0 && color != Color.Transparent)
                        bitmap.SetPixel(x, y, newColor);
                }

            image.Dispose();
            return bitmap;
        }

        internal void GetColorsForTheme(string theme, out Color backColor, out Color foreColor, out Color highlightedForeColor, out Color buttonColor, out Brush progressBarBrush)
        {
            switch (currentTheme = theme)
            {
                case null:
                    GetColorsForTheme(LightTheme.ThemeString, out backColor, out foreColor, out highlightedForeColor, out buttonColor, out progressBarBrush);
                    break;

                case LightTheme.ThemeString:
                    backColor = LightTheme.BackColor;
                    foreColor = LightTheme.ForeColor;
                    highlightedForeColor = LightTheme.HighlightedForeColor;
                    buttonColor = LightTheme.ButtonColor;
                    progressBarBrush = LightTheme.progressBarBrush;
                    break;

                case DarkTheme.ThemeString:
                    backColor = DarkTheme.BackColor;
                    foreColor = DarkTheme.ForeColor;
                    highlightedForeColor = DarkTheme.HighlightedForeColor;
                    buttonColor = DarkTheme.ButtonColor;
                    progressBarBrush = DarkTheme.progressBarBrush;
                    break;

                default:
                    throw new ArgumentException("Theme doesn't exist");
            }
        }

        private void ChangeFormAndControlsColor(Form form, Color BackColor, Color ForeColor)
        {
            form.BackColor = BackColor;
            form.ForeColor = ForeColor;
            foreach (Control control in form.Controls)
                ChangeControlAndAllChildControlsColor(control, BackColor, ForeColor);
        }

        private void ChangeControlAndAllChildControlsColor(Control control, Color BackColor, Color ForeColor)
        {
            control.BackColor = BackColor;
            foreach (Control control1 in control.Controls)
            {
                control1.BackColor = BackColor;
                control1.ForeColor = ForeColor;
                if (control1.Controls.Count > 0)
                    for (int i = 0; i < control1.Controls.Count; i++)
                        ChangeControlAndAllChildControlsColor(control1.Controls[i], BackColor, ForeColor);
            }
            ControlPanel.BackColor = BackColor;
        }
    }
}