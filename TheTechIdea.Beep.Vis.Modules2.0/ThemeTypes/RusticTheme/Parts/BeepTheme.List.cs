using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RusticTheme
    {
        // List Fonts & Colors
        public TypographyStyle ListTitleFont { get; set; }
        public TypographyStyle ListSelectedFont { get; set; }
        public TypographyStyle ListUnSelectedFont { get; set; }
        public Color ListBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color ListForeColor { get; set; } = Color.FromArgb(51, 51, 51);
        public Color ListBorderColor { get; set; } = Color.FromArgb(205, 133, 63);
        public Color ListItemForeColor { get; set; } = Color.FromArgb(51, 51, 51);
        public Color ListItemHoverForeColor { get; set; } = Color.FromArgb(62, 39, 35);
        public Color ListItemHoverBackColor { get; set; } = Color.FromArgb(222, 184, 135);
        public Color ListItemSelectedForeColor { get; set; } = Color.White;
        public Color ListItemSelectedBackColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color ListItemSelectedBorderColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color ListItemBorderColor { get; set; } = Color.FromArgb(205, 133, 63);
        public Color ListItemHoverBorderColor { get; set; } = Color.FromArgb(160, 82, 45);
    }
}
