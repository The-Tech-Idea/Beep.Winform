using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MaterialDesignTheme
    {
        // List Fonts & Colors
<<<<<<< HEAD
        public Font ListTitleFont { get; set; } = new Font("Roboto", 16f, FontStyle.Bold);
        public Font ListSelectedFont { get; set; } = new Font("Roboto", 14f, FontStyle.Regular);
        public Font ListUnSelectedFont { get; set; } = new Font("Roboto", 14f, FontStyle.Regular);

        public Color ListBackColor { get; set; } = Color.White;
        public Color ListForeColor { get; set; } = Color.FromArgb(33, 33, 33); // Dark Grey 900
        public Color ListBorderColor { get; set; } = Color.FromArgb(224, 224, 224); // Grey 300

        public Color ListItemForeColor { get; set; } = Color.FromArgb(33, 33, 33); // Dark Grey 900
        public Color ListItemHoverForeColor { get; set; } = Color.FromArgb(25, 118, 210); // Blue 700
        public Color ListItemHoverBackColor { get; set; } = Color.FromArgb(232, 240, 254); // Light Blue 50

        public Color ListItemSelectedForeColor { get; set; } = Color.White;
        public Color ListItemSelectedBackColor { get; set; } = Color.FromArgb(25, 118, 210); // Blue 700
        public Color ListItemSelectedBorderColor { get; set; } = Color.FromArgb(21, 101, 192); // Blue 800

        public Color ListItemBorderColor { get; set; } = Color.FromArgb(224, 224, 224); // Grey 300
        public Color ListItemHoverBorderColor { get; set; } = Color.FromArgb(25, 118, 210); // Blue 700
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
