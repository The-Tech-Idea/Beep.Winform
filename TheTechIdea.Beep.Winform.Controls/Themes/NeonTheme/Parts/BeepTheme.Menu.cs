using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeonTheme
    {
        private void ApplyMenu()
        {
            // Neon menu - vibrant neon theme
            this.MenuTitleFont = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 14F, FontStyle.Bold);
            this.MenuItemSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 12F, FontStyle.Bold);
            this.MenuItemUnSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 12F, FontStyle.Regular);
            
            this.MenuBackColor = BackgroundColor;  // Deep navy
            this.MenuForeColor = ForeColor;  // Cyan text
            this.MenuBorderColor = BorderColor;  // Cyan glow
            
            // Main menu items
            this.MenuMainItemForeColor = ForeColor;
            this.MenuMainItemHoverForeColor = ForeColor;  // Brighter cyan-ish on hover mapped to ForeColor
            this.MenuMainItemHoverBackColor = PanelGradiantStartColor;  // Lighter navy
            this.MenuMainItemSelectedForeColor = OnPrimaryColor;  // Dark text on neon selected
            this.MenuMainItemSelectedBackColor = SecondaryColor;  // Cyan background
            
            // Sub menu items
            this.MenuItemForeColor = ForeColor;
            this.MenuItemHoverForeColor = ForeColor;
            this.MenuItemHoverBackColor = PanelGradiantStartColor;  // Lighter navy
            this.MenuItemSelectedForeColor = OnPrimaryColor;
            this.MenuItemSelectedBackColor = SecondaryColor;
            
            // Dark gradient
            this.MenuGradiantStartColor = PanelGradiantStartColor;
            this.MenuGradiantEndColor = PanelGradiantEndColor;
            this.MenuGradiantMiddleColor = PanelGradiantMiddleColor;
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}