using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MinimalTheme
    {
        private void ApplyMenu()
        {
            // Minimal menu - clean, minimal aesthetic
            this.MenuTitleFont = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14F, FontStyle.Bold);
            this.MenuItemSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12F, FontStyle.Bold);
            this.MenuItemUnSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12F, FontStyle.Regular);
            
            this.MenuBackColor = BackgroundColor;  // White
            this.MenuForeColor = ForeColor;  // Dark grey text
            this.MenuBorderColor = BorderColor;  // Light grey
            
            // Main menu items
            this.MenuMainItemForeColor = ForeColor;
            this.MenuMainItemHoverForeColor = ForeColor;
            this.MenuMainItemHoverBackColor = SurfaceColor;
            this.MenuMainItemSelectedForeColor = ForeColor;
            this.MenuMainItemSelectedBackColor = ThemeUtil.Lighten(SurfaceColor, 0.03);
            
            // Sub menu items
            this.MenuItemForeColor = ForeColor;
            this.MenuItemHoverForeColor = ForeColor;
            this.MenuItemHoverBackColor = SurfaceColor;
            this.MenuItemSelectedForeColor = ForeColor;
            this.MenuItemSelectedBackColor = ThemeUtil.Lighten(SurfaceColor, 0.03);
            
            // Clean gradient
            this.MenuGradiantStartColor = BackgroundColor;
            this.MenuGradiantEndColor = BackgroundColor;
            this.MenuGradiantMiddleColor = BackgroundColor;
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
