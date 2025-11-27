using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordTheme
    {
        private void ApplyMenu()
        {
            // Nord menu - Arctic-inspired dark theme
            this.MenuTitleFont = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14F, FontStyle.Bold);
            this.MenuItemSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12F, FontStyle.Bold);
            this.MenuItemUnSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12F, FontStyle.Regular);
            
            this.MenuBackColor = PanelBackColor;  // Dark blue-gray
            this.MenuForeColor = ForeColor;  // Light gray-blue text
            this.MenuBorderColor = BorderColor;  // #434C5E
            
            // Main menu items
            this.MenuMainItemForeColor = ForeColor;
            this.MenuMainItemHoverForeColor = PrimaryColor;  // Nord cyan on hover
            this.MenuMainItemHoverBackColor = PanelGradiantStartColor;  // Lighter gray
            this.MenuMainItemSelectedForeColor = OnPrimaryColor;  // Dark text on primary
            this.MenuMainItemSelectedBackColor = PrimaryColor;  // Nord cyan
            
            // Sub menu items
            this.MenuItemForeColor = ForeColor;
            this.MenuItemHoverForeColor = PrimaryColor;  // Nord cyan on hover
            this.MenuItemHoverBackColor = PanelGradiantStartColor;  // Lighter gray
            this.MenuItemSelectedForeColor = OnPrimaryColor;  // Dark text on primary
            this.MenuItemSelectedBackColor = PrimaryColor;  // Nord cyan
            
            // Dark gradient
            this.MenuGradiantStartColor = PanelGradiantStartColor;
            this.MenuGradiantEndColor = PanelGradiantEndColor;
            this.MenuGradiantMiddleColor = PanelGradiantMiddleColor;
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}