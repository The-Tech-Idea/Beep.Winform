using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // List Fonts & Colors
        public TypographyStyle ListTitleFont { get; set; } = new TypographyStyle() { FontSize = 14, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(80, 80, 80) };
        public TypographyStyle ListSelectedFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.White };
        public TypographyStyle ListUnSelectedFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(120, 120, 120) };
        public Color ListBackColor { get; set; } = Color.FromArgb(255, 245, 247);
        public Color ListForeColor { get; set; } = Color.FromArgb(120, 120, 120);
        public Color ListBorderColor { get; set; } = Color.FromArgb(242, 201, 215);
        public Color ListItemForeColor { get; set; } = Color.FromArgb(120, 120, 120);
        public Color ListItemHoverForeColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color ListItemHoverBackColor { get; set; } = Color.FromArgb(255, 224, 239);
        public Color ListItemSelectedForeColor { get; set; } = Color.White;
        public Color ListItemSelectedBackColor { get; set; } = Color.FromArgb(245, 183, 203);
        public Color ListItemSelectedBorderColor { get; set; } = Color.FromArgb(230, 170, 190);
        public Color ListItemBorderColor { get; set; } = Color.FromArgb(242, 201, 215);
        public Color ListItemHoverBorderColor { get; set; } = Color.FromArgb(237, 181, 201);
    }
}