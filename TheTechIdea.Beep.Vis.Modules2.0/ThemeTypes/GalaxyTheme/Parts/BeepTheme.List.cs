using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GalaxyTheme
    {
        // List Fonts & Colors
<<<<<<< HEAD
        public Font ListTitleFont { get; set; } = new Font("Segoe UI", 12f, FontStyle.Bold);
        public Font ListSelectedFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Bold);
        public Font ListUnSelectedFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Regular);

        public Color ListBackColor { get; set; } = Color.FromArgb(0x1F, 0x19, 0x39); // SurfaceColor
        public Color ListForeColor { get; set; } = Color.White;
        public Color ListBorderColor { get; set; } = Color.FromArgb(0x33, 0x33, 0x33); // Subtle border

        public Color ListItemForeColor { get; set; } = Color.White;
        public Color ListItemHoverForeColor { get; set; } = Color.White;
        public Color ListItemHoverBackColor { get; set; } = Color.FromArgb(0x23, 0x23, 0x4E); // Hover dark blue

        public Color ListItemSelectedForeColor { get; set; } = Color.White;
        public Color ListItemSelectedBackColor { get; set; } = Color.FromArgb(0x0F, 0x34, 0x60); // AccentColor
        public Color ListItemSelectedBorderColor { get; set; } = Color.White;

        public Color ListItemBorderColor { get; set; } = Color.FromArgb(0x44, 0x44, 0x44); // Divider tone
        public Color ListItemHoverBorderColor { get; set; } = Color.FromArgb(0x4E, 0xC5, 0xF1); // Light blue hover
=======
        public TypographyStyle ListTitleFont { get; set; }
        public TypographyStyle ListSelectedFont { get; set; }
        public TypographyStyle ListUnSelectedFont { get; set; }
        public Color ListBackColor { get; set; }
        public Color ListForeColor { get; set; }
        public Color ListBorderColor { get; set; }
        public Color ListItemForeColor { get; set; }
        public Color ListItemHoverForeColor { get; set; }
        public Color ListItemHoverBackColor { get; set; }
        public Color ListItemSelectedForeColor { get; set; }
        public Color ListItemSelectedBackColor { get; set; }
        public Color ListItemSelectedBorderColor { get; set; }
        public Color ListItemBorderColor { get; set; }
        public Color ListItemHoverBorderColor { get; set; }
>>>>>>> 00d68a6e1277c6b19c9d032a5dafd4d4e082d634
    }
}
