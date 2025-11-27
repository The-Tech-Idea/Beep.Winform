using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class FluentTheme
    {
        private void ApplyMenu()
        {
            // Fluent menu - light, modern
            this.MenuTitleFont = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14F, FontStyle.Bold);
            this.MenuItemSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12F, FontStyle.Bold);
            this.MenuItemUnSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12F, FontStyle.Regular);
            
            this.MenuBackColor = BackgroundColor;  // Light gray-blue
            this.MenuForeColor = ForeColor;  // Dark gray text
            this.MenuBorderColor = BorderColor;  // Light gray
            
            // Main menu items
            this.MenuMainItemForeColor = ForeColor;
            this.MenuMainItemHoverForeColor = ForeColor;
            this.MenuMainItemHoverBackColor = PanelGradiantMiddleColor;  // Light blue-gray hover
            this.MenuMainItemSelectedForeColor = PrimaryColor;  // Blue text
            this.MenuMainItemSelectedBackColor = PanelGradiantMiddleColor;  // Light blue
            
            // Sub menu items
            this.MenuItemForeColor = ForeColor;
            this.MenuItemHoverForeColor = ForeColor;
            this.MenuItemHoverBackColor = PanelGradiantMiddleColor;  // Light blue-gray hover
            this.MenuItemSelectedForeColor = PrimaryColor;  // Blue text
            this.MenuItemSelectedBackColor = PanelGradiantMiddleColor;  // Light blue
            
            // Subtle gradient
            this.MenuGradiantStartColor = PanelGradiantStartColor;
            this.MenuGradiantEndColor = PanelGradiantEndColor;
            this.MenuGradiantMiddleColor = PanelGradiantMiddleColor;
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}