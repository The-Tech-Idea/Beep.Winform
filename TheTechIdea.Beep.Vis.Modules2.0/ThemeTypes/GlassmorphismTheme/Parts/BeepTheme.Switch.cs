﻿using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GlassmorphismTheme
    {
        // Switch control Fonts & Colors
        public Font SwitchTitleFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Bold);
        public Font SwitchSelectedFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Bold);
        public Font SwitchUnSelectedFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Regular);

        public Color SwitchBackColor { get; set; } = Color.WhiteSmoke;
        public Color SwitchBorderColor { get; set; } = Color.LightGray;
        public Color SwitchForeColor { get; set; } = Color.Black;

        public Color SwitchSelectedBackColor { get; set; } = Color.DeepSkyBlue;
        public Color SwitchSelectedBorderColor { get; set; } = Color.SteelBlue;
        public Color SwitchSelectedForeColor { get; set; } = Color.White;

        public Color SwitchHoverBackColor { get; set; } = Color.LightBlue;
        public Color SwitchHoverBorderColor { get; set; } = Color.CornflowerBlue;
        public Color SwitchHoverForeColor { get; set; } = Color.Black;
    }
}
