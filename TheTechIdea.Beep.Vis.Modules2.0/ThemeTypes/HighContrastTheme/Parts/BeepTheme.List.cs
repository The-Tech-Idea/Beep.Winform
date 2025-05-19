using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighContrastTheme
    {
        // List Fonts & Colors
<<<<<<< HEAD
        public Font ListTitleFont { get; set; } = new Font("Segoe UI", 14, FontStyle.Bold);
        public Font ListSelectedFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Bold);
        public Font ListUnSelectedFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Regular);

        public Color ListBackColor { get; set; } = Color.Black;
        public Color ListForeColor { get; set; } = Color.White;
        public Color ListBorderColor { get; set; } = Color.White;
        public Color ListItemForeColor { get; set; } = Color.White;
        public Color ListItemHoverForeColor { get; set; } = Color.Black;
        public Color ListItemHoverBackColor { get; set; } = Color.Yellow;
        public Color ListItemSelectedForeColor { get; set; } = Color.Black;
        public Color ListItemSelectedBackColor { get; set; } = Color.Cyan;
        public Color ListItemSelectedBorderColor { get; set; } = Color.White;
        public Color ListItemBorderColor { get; set; } = Color.White;
        public Color ListItemHoverBorderColor { get; set; } = Color.Yellow;
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
