using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordicTheme
    {
        private void ApplyMenu()
        {
            // Nordic menu - Scandinavian minimalist theme
            this.MenuTitleFont = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14F, FontStyle.Bold);
            this.MenuItemSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12F, FontStyle.Bold);
            this.MenuItemUnSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12F, FontStyle.Regular);
            
            this.MenuBackColor = BackgroundColor;  // Light gray-blue
            this.MenuForeColor = ForeColor;  // Dark gray text
            this.MenuBorderColor = BorderColor;  // Light gray
            
            // Main menu items
            this.MenuMainItemForeColor = ForeColor;
            this.MenuMainItemHoverForeColor = ForeColor;
            this.MenuMainItemHoverBackColor = PanelGradiantMiddleColor;  // Darker hover
            this.MenuMainItemSelectedForeColor = OnPrimaryColor;  // White text
            this.MenuMainItemSelectedBackColor = PrimaryColor;  // Icy blue
            
            // Sub menu items
            this.MenuItemForeColor = ForeColor;
            this.MenuItemHoverForeColor = ForeColor;
            this.MenuItemHoverBackColor = PanelGradiantMiddleColor;  // Darker hover
            this.MenuItemSelectedForeColor = OnPrimaryColor;  // White text
            this.MenuItemSelectedBackColor = PrimaryColor;  // Icy blue
            
            // Minimal gradient
            this.MenuGradiantStartColor = ThemeUtil.Lighten(BackgroundColor, 0.03);
            this.MenuGradiantEndColor = ThemeUtil.Darken(BackgroundColor, 0.04);
            this.MenuGradiantMiddleColor = ThemeUtil.Lighten(BackgroundColor, 0.01);
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}