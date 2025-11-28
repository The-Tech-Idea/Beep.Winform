using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class iOSTheme
    {
        private void ApplyMenu()
        {
            // iOS menu - clean, modern theme
            this.MenuTitleFont = ThemeUtils.ConvertFontToTypographyStyle("SF Pro", 14F, FontStyle.Bold);
            this.MenuItemSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("SF Pro", 12F, FontStyle.Bold);
            this.MenuItemUnSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("SF Pro", 12F, FontStyle.Regular);
            
            this.MenuBackColor = BackgroundColor;  // Light gray
            this.MenuForeColor = ForeColor;  // Dark gray text
            this.MenuBorderColor = BorderColor;  // Medium gray
            
            // Main menu items
            this.MenuMainItemForeColor = ForeColor;
            this.MenuMainItemHoverForeColor = ForeColor;
            this.MenuMainItemHoverBackColor = ThemeUtil.Darken(BackgroundColor, 0.06);  // Darker gray hover
            this.MenuMainItemSelectedForeColor = OnPrimaryColor;  // White text
            this.MenuMainItemSelectedBackColor = PrimaryColor;  // iOS blue
            
            // Sub menu items
            this.MenuItemForeColor = ForeColor;
            this.MenuItemHoverForeColor = ForeColor;
            this.MenuItemHoverBackColor = ThemeUtil.Darken(BackgroundColor, 0.06);  // Darker gray hover
            this.MenuItemSelectedForeColor = OnPrimaryColor;  // White text
            this.MenuItemSelectedBackColor = PrimaryColor;  // iOS blue
            
            // Clean gradient
            this.MenuGradiantStartColor = PanelGradiantStartColor;
            this.MenuGradiantEndColor = PanelGradiantEndColor;
            this.MenuGradiantMiddleColor = PanelGradiantMiddleColor;
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}