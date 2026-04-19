using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // Menu Fonts & Colors
        public TypographyStyle MenuTitleFont { get; set; } = new TypographyStyle() { FontSize = 14f, FontWeight = FontWeight.Bold, TextColor = Color.White };
        public TypographyStyle MenuItemSelectedFont { get; set; } = new TypographyStyle() { FontSize = 8f, FontWeight = FontWeight.Medium, TextColor = Color.White };
        public TypographyStyle MenuItemUnSelectedFont { get; set; } = new TypographyStyle() { FontSize = 8f, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(200, 255, 255) };
        public Color MenuBackColor { get; set; } = Color.FromArgb(0, 105, 148);
        public Color MenuForeColor { get; set; } = Color.FromArgb(200, 255, 255);
        public Color MenuBorderColor { get; set; } = Color.FromArgb(0, 120, 170);
        public Color MenuMainItemForeColor { get; set; } = Color.FromArgb(200, 255, 255);
        public Color MenuMainItemHoverForeColor { get; set; } = Color.White;
        public Color MenuMainItemHoverBackColor { get; set; } = Color.FromArgb(0, 160, 210);
        public Color MenuMainItemSelectedForeColor { get; set; } = Color.White;
        public Color MenuMainItemSelectedBackColor { get; set; } = Color.FromArgb(0, 180, 230);
        public Color MenuItemForeColor { get; set; } = Color.FromArgb(200, 255, 255);
        public Color MenuItemHoverForeColor { get; set; } = Color.White;
        public Color MenuItemHoverBackColor { get; set; } = Color.FromArgb(0, 160, 210);
        public Color MenuItemSelectedForeColor { get; set; } = Color.White;
        public Color MenuItemSelectedBackColor { get; set; } = Color.FromArgb(0, 180, 230);
        public Color MenuGradiantStartColor { get; set; } = Color.FromArgb(0, 80, 120);
        public Color MenuGradiantEndColor { get; set; } = Color.FromArgb(0, 130, 180);
        public Color MenuGradiantMiddleColor { get; set; } = Color.FromArgb(0, 105, 148);
        public LinearGradientMode MenuGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
