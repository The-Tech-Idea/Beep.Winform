using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class OneDarkTheme
    {
        private void ApplyMenu()
        {   
            
            this.MenuTitleFont= ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14F, FontStyle.Bold);
        this.MenuItemSelectedFont  = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12F, FontStyle.Bold);
        this.MenuItemUnSelectedFont= ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12F, FontStyle.Regular);


            // One Dark menu - popular dark theme
            this.MenuBackColor = PanelBackColor;  // Dark background
            this.MenuForeColor = ForeColor;  // Warm grey text
            this.MenuBorderColor = BorderColor;  // #3C4252
            
            // Main menu items
            this.MenuMainItemForeColor = ForeColor;
            this.MenuMainItemHoverForeColor = PrimaryColor;  // One Dark blue on hover
            this.MenuMainItemHoverBackColor = PanelGradiantMiddleColor;  // Lighter dark
            this.MenuMainItemSelectedForeColor = OnPrimaryColor;  // Dark text on primary
            this.MenuMainItemSelectedBackColor = PrimaryColor;  // One Dark blue
            
            // Sub menu items
            this.MenuItemForeColor = ForeColor;
            this.MenuItemHoverForeColor = PrimaryColor;  // One Dark blue on hover
            this.MenuItemHoverBackColor = PanelGradiantMiddleColor;  // Lighter dark
            this.MenuItemSelectedForeColor = OnPrimaryColor;  // Dark text on primary
            this.MenuItemSelectedBackColor = PrimaryColor;  // One Dark blue
            
            // Dark gradient
            this.MenuGradiantStartColor = PanelGradiantStartColor;
            this.MenuGradiantEndColor = PanelGradiantEndColor;
            this.MenuGradiantMiddleColor = PanelGradiantMiddleColor;
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}