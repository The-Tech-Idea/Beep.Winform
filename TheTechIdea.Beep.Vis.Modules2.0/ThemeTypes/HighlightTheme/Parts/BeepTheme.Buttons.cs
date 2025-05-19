﻿using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighlightTheme
    {
        // Button Colors and Styles
<<<<<<< HEAD
        public Font ButtonFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Regular);
        public Font ButtonHoverFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);
        public Font ButtonSelectedFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);
=======
        public TypographyStyle ButtonFont { get; set; }
        public TypographyStyle ButtonHoverFont { get; set; }
        public TypographyStyle ButtonSelectedFont { get; set; }
>>>>>>> 00d68a6e1277c6b19c9d032a5dafd4d4e082d634

        public Color ButtonHoverBackColor { get; set; } = Color.FromArgb(255, 230, 180);
        public Color ButtonHoverForeColor { get; set; } = Color.FromArgb(85, 65, 0);
        public Color ButtonHoverBorderColor { get; set; } = Color.FromArgb(255, 196, 0);

        public Color ButtonSelectedBorderColor { get; set; } = Color.FromArgb(255, 153, 0);
        public Color ButtonSelectedBackColor { get; set; } = Color.FromArgb(255, 196, 0);
        public Color ButtonSelectedForeColor { get; set; } = Color.White;

        public Color ButtonSelectedHoverBackColor { get; set; } = Color.FromArgb(255, 179, 0);
        public Color ButtonSelectedHoverForeColor { get; set; } = Color.White;
        public Color ButtonSelectedHoverBorderColor { get; set; } = Color.FromArgb(255, 230, 100);

        public Color ButtonBackColor { get; set; } = Color.FromArgb(255, 215, 102);
        public Color ButtonForeColor { get; set; } = Color.FromArgb(60, 40, 0);
        public Color ButtonBorderColor { get; set; } = Color.FromArgb(255, 179, 0);

        public Color ButtonErrorBackColor { get; set; } = Color.FromArgb(255, 0, 0);
        public Color ButtonErrorForeColor { get; set; } = Color.White;
        public Color ButtonErrorBorderColor { get; set; } = Color.FromArgb(179, 0, 0);

        public Color ButtonPressedBackColor { get; set; } = Color.FromArgb(204, 153, 0);
        public Color ButtonPressedForeColor { get; set; } = Color.White;
        public Color ButtonPressedBorderColor { get; set; } = Color.FromArgb(153, 102, 0);
    }
}
