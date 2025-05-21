using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // List Fonts & Colors
        public TypographyStyle ListTitleFont { get; set; } = new TypographyStyle() { FontSize = 14, FontWeight = FontWeight.Bold, TextColor = Color.White };
        public TypographyStyle ListSelectedFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.White };
        public TypographyStyle ListUnSelectedFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(200, 255, 255) };
        public Color ListBackColor { get; set; } = Color.FromArgb(0, 105, 148);
        public Color ListForeColor { get; set; } = Color.FromArgb(200, 255, 255);
        public Color ListBorderColor { get; set; } = Color.FromArgb(0, 120, 170);
        public Color ListItemForeColor { get; set; } = Color.FromArgb(200, 255, 255);
        public Color ListItemHoverForeColor { get; set; } = Color.White;
        public Color ListItemHoverBackColor { get; set; } = Color.FromArgb(0, 160, 210);
        public Color ListItemSelectedForeColor { get; set; } = Color.White;
        public Color ListItemSelectedBackColor { get; set; } = Color.FromArgb(0, 180, 230);
        public Color ListItemSelectedBorderColor { get; set; } = Color.FromArgb(0, 150, 200);
        public Color ListItemBorderColor { get; set; } = Color.FromArgb(0, 120, 170);
        public Color ListItemHoverBorderColor { get; set; } = Color.FromArgb(0, 130, 180);
    }
}