using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class DraculaTheme
    {
        private void ApplyMenu()
        {
            // Dracula menu - dark purple theme
            this.MenuTitleFont = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14F, FontStyle.Bold);
            this.MenuItemSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12F, FontStyle.Bold);
            this.MenuItemUnSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12F, FontStyle.Regular);
            
            this.MenuBackColor = BackgroundColor;  // Dark background
            this.MenuForeColor = ForeColor;  // Cream text
            this.MenuBorderColor = BorderColor;  // Gray border
            
            // Main menu items
            this.MenuMainItemForeColor = ForeColor;
            this.MenuMainItemHoverForeColor = AccentColor;  // Pink on hover
            this.MenuMainItemHoverBackColor = PanelGradiantMiddleColor;  // Lighter surface
            this.MenuMainItemSelectedForeColor = AccentColor;
            this.MenuMainItemSelectedBackColor = PanelGradiantMiddleColor;  // Medium surface
            
            // Sub menu items
            this.MenuItemForeColor = ForeColor;
            this.MenuItemHoverForeColor = AccentColor;  // Pink on hover
            this.MenuItemHoverBackColor = PanelGradiantMiddleColor;  // Lighter surface
            this.MenuItemSelectedForeColor = AccentColor;
            this.MenuItemSelectedBackColor = PanelGradiantMiddleColor;  // Medium surface
            
            // Purple gradient
            this.MenuGradiantStartColor = PanelGradiantStartColor;
            this.MenuGradiantEndColor = PanelGradiantEndColor;
            this.MenuGradiantMiddleColor = PanelGradiantMiddleColor;
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
        }
    }
}