﻿using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MonochromeTheme
    {
        // Menu Fonts & Colors
        public TypographyStyle MenuTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14, FontStyle.Bold);
        public TypographyStyle MenuItemSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Bold);
        public TypographyStyle MenuItemUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Regular);

        public Color MenuBackColor { get; set; } = Color.White;
        public Color MenuForeColor { get; set; } = Color.Black;
        public Color MenuBorderColor { get; set; } = Color.Gray;

        public Color MenuMainItemForeColor { get; set; } = Color.Black;
        public Color MenuMainItemHoverForeColor { get; set; } = Color.DimGray;
        public Color MenuMainItemHoverBackColor { get; set; } = Color.LightGray;
        public Color MenuMainItemSelectedForeColor { get; set; } = Color.White;
        public Color MenuMainItemSelectedBackColor { get; set; } = Color.Black;

        public Color MenuItemForeColor { get; set; } = Color.Black;
        public Color MenuItemHoverForeColor { get; set; } = Color.DimGray;
        public Color MenuItemHoverBackColor { get; set; } = Color.LightGray;
        public Color MenuItemSelectedForeColor { get; set; } = Color.White;
        public Color MenuItemSelectedBackColor { get; set; } = Color.Black;

        public Color MenuGradiantStartColor { get; set; } = Color.White;
        public Color MenuGradiantEndColor { get; set; } = Color.LightGray;
        public Color MenuGradiantMiddleColor { get; set; } = Color.Gray;
        public LinearGradientMode MenuGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
