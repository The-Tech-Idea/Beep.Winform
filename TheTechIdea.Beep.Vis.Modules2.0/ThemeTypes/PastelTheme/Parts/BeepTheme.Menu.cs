using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Menu Fonts & Colors
        public TypographyStyle MenuTitleFont { get; set; } = new TypographyStyle() { FontSize = 14, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(80, 80, 80) };
        public TypographyStyle MenuItemSelectedFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.White };
        public TypographyStyle MenuItemUnSelectedFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(120, 120, 120) };
        public Color MenuBackColor { get; set; } = Color.FromArgb(255, 245, 247);
        public Color MenuForeColor { get; set; } = Color.FromArgb(120, 120, 120);
        public Color MenuBorderColor { get; set; } = Color.FromArgb(242, 201, 215);
        public Color MenuMainItemForeColor { get; set; } = Color.FromArgb(120, 120, 120);
        public Color MenuMainItemHoverForeColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color MenuMainItemHoverBackColor { get; set; } = Color.FromArgb(255, 224, 239);
        public Color MenuMainItemSelectedForeColor { get; set; } = Color.White;
        public Color MenuMainItemSelectedBackColor { get; set; } = Color.FromArgb(245, 183, 203);
        public Color MenuItemForeColor { get; set; } = Color.FromArgb(120, 120, 120);
        public Color MenuItemHoverForeColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color MenuItemHoverBackColor { get; set; } = Color.FromArgb(255, 224, 239);
        public Color MenuItemSelectedForeColor { get; set; } = Color.White;
        public Color MenuItemSelectedBackColor { get; set; } = Color.FromArgb(245, 183, 203);
        public Color MenuGradiantStartColor { get; set; } = Color.FromArgb(237, 181, 201);
        public Color MenuGradiantEndColor { get; set; } = Color.FromArgb(247, 221, 229);
        public Color MenuGradiantMiddleColor { get; set; } = Color.FromArgb(242, 201, 215);
        public LinearGradientMode MenuGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}