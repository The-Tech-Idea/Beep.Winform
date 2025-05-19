using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DesertTheme
    {
        // Menu Fonts & Colors
        public TypographyStyle MenuTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 16, FontStyle.Bold);
        public TypographyStyle MenuItemSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14, FontStyle.Bold);
        public TypographyStyle MenuItemUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14, FontStyle.Regular);

        public Color MenuBackColor { get; set; } = Color.FromArgb(255, 244, 214); // light sand
        public Color MenuForeColor { get; set; } = Color.FromArgb(101, 67, 33); // dark brown
        public Color MenuBorderColor { get; set; } = Color.FromArgb(194, 178, 128); // sandy beige

        public Color MenuMainItemForeColor { get; set; } = Color.FromArgb(139, 69, 19); // saddle brown
        public Color MenuMainItemHoverForeColor { get; set; } = Color.FromArgb(160, 82, 45); // sienna
        public Color MenuMainItemHoverBackColor { get; set; } = Color.FromArgb(255, 228, 196); // bisque
        public Color MenuMainItemSelectedForeColor { get; set; } = Color.White;
        public Color MenuMainItemSelectedBackColor { get; set; } = Color.FromArgb(210, 105, 30); // chocolate

        public Color MenuItemForeColor { get; set; } = Color.FromArgb(101, 67, 33); // dark brown
        public Color MenuItemHoverForeColor { get; set; } = Color.FromArgb(160, 82, 45); // sienna
        public Color MenuItemHoverBackColor { get; set; } = Color.FromArgb(255, 235, 205); // blanched almond
        public Color MenuItemSelectedForeColor { get; set; } = Color.White;
        public Color MenuItemSelectedBackColor { get; set; } = Color.FromArgb(210, 105, 30); // chocolate

        public Color MenuGradiantStartColor { get; set; } = Color.FromArgb(255, 244, 214); // light sand
        public Color MenuGradiantEndColor { get; set; } = Color.FromArgb(194, 178, 128); // sandy beige
        public Color MenuGradiantMiddleColor { get; set; } = Color.FromArgb(222, 184, 135); // burlywood
        public LinearGradientMode MenuGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
