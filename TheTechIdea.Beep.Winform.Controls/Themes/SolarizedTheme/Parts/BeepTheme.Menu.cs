using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class SolarizedTheme
    {
        private void ApplyMenu()
        {
            // Solarized menu - scientifically crafted color palette
            this.MenuTitleFont = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 14F, FontStyle.Bold);
            this.MenuItemSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 12F, FontStyle.Bold);
            this.MenuItemUnSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 12F, FontStyle.Regular);
            
            this.MenuBackColor = BackgroundColor;  // Dark blue-green
            this.MenuForeColor = ForeColor;  // Light beige text
            this.MenuBorderColor = BorderColor;  // #586E75
            
            // Main menu items
            this.MenuMainItemForeColor = ForeColor;
            this.MenuMainItemHoverForeColor = SecondaryColor;  // Cyan on hover
            this.MenuMainItemHoverBackColor = SurfaceColor;  // #073642
            this.MenuMainItemSelectedForeColor = OnPrimaryColor;  // Light text
            this.MenuMainItemSelectedBackColor = AccentColor;  // Orange
            
            // Sub menu items
            this.MenuItemForeColor = ForeColor;
            this.MenuItemHoverForeColor = SecondaryColor;  // Cyan on hover
            this.MenuItemHoverBackColor = SurfaceColor;  // #073642
            this.MenuItemSelectedForeColor = OnPrimaryColor;  // Light text
            this.MenuItemSelectedBackColor = AccentColor;  // Orange
            
            // Dark gradient based on background
            this.MenuGradiantStartColor = BackgroundColor;
            this.MenuGradiantEndColor = BackgroundColor;
            this.MenuGradiantMiddleColor = ThemeUtil.Lighten(BackgroundColor, 0.04);
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}