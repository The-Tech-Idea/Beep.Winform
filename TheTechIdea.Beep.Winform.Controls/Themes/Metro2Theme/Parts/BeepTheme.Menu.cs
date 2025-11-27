using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class Metro2Theme
    {
        private void ApplyMenu()
        {
            // Metro2 menu - Windows Metro with accent stripe
            this.MenuTitleFont = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14F, FontStyle.Bold);
            this.MenuItemSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12F, FontStyle.Bold);
            this.MenuItemUnSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12F, FontStyle.Regular);
            
            this.MenuBackColor = PanelBackColor;  // Light gray
            this.MenuForeColor = ForeColor;  // Black text
            this.MenuBorderColor = BorderColor;  // Metro blue border
            
            // Main menu items
            this.MenuMainItemForeColor = ForeColor;
            this.MenuMainItemHoverForeColor = ForeColor;
            this.MenuMainItemHoverBackColor = PanelGradiantMiddleColor;  // Darker gray hover
            this.MenuMainItemSelectedForeColor = OnPrimaryColor;  // White text
            this.MenuMainItemSelectedBackColor = PrimaryColor;  // Metro blue
            
            // Sub menu items
            this.MenuItemForeColor = ForeColor;
            this.MenuItemHoverForeColor = ForeColor;
            this.MenuItemHoverBackColor = PanelGradiantMiddleColor;  // Darker gray hover
            this.MenuItemSelectedForeColor = OnPrimaryColor;  // White text
            this.MenuItemSelectedBackColor = PrimaryColor;  // Metro blue
            
            // Clean gradient
            this.MenuGradiantStartColor = PanelGradiantStartColor;
            this.MenuGradiantEndColor = PanelGradiantEndColor;
            this.MenuGradiantMiddleColor = PanelGradiantMiddleColor;
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}