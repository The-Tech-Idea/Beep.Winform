using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MacOSTheme
    {
        private void ApplyMenu()
        {
            // MacOS menu - clean macOS theme
            this.MenuTitleFont = ThemeUtils.ConvertFontToTypographyStyle("SF Pro", 14F, FontStyle.Bold);
            this.MenuItemSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("SF Pro", 12F, FontStyle.Bold);
            this.MenuItemUnSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("SF Pro", 12F, FontStyle.Regular);
            
            this.MenuBackColor = BackgroundColor;  // Light gray
            this.MenuForeColor = ForeColor;  // Dark gray text
            this.MenuBorderColor = BorderColor;  // Medium gray
            
            // Main menu items
            this.MenuMainItemForeColor = ForeColor;
            this.MenuMainItemHoverForeColor = ForeColor;
            this.MenuMainItemHoverBackColor = PanelGradiantMiddleColor;  // Darker gray hover
            this.MenuMainItemSelectedForeColor = ForeColor;
            this.MenuMainItemSelectedBackColor = PanelBackColor;  // Medium gray
            
            // Sub menu items
            this.MenuItemForeColor = ForeColor;
            this.MenuItemHoverForeColor = ForeColor;
            this.MenuItemHoverBackColor = PanelGradiantMiddleColor;  // Darker gray hover
            this.MenuItemSelectedForeColor = ForeColor;
            this.MenuItemSelectedBackColor = PanelBackColor;  // Medium gray
            
            // Clean gradient
            this.MenuGradiantStartColor = ThemeUtil.Lighten(BackgroundColor, 0.02);
            this.MenuGradiantEndColor = BackgroundColor;
            this.MenuGradiantMiddleColor = ThemeUtil.Lighten(BackgroundColor, 0.01);
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}