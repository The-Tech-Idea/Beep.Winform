using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class TokyoTheme
    {
        private void ApplyMenu()
        {
            // Tokyo Night menu - inspired by Tokyo Night VSCode theme
            this.MenuTitleFont = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14F, FontStyle.Bold);
            this.MenuItemSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12F, FontStyle.Bold);
            this.MenuItemUnSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12F, FontStyle.Regular);
            
            this.MenuBackColor = BackgroundColor;  // Dark purple
            this.MenuForeColor = ForeColor;  // Light purple-blue text
            this.MenuBorderColor = BorderColor;  // #56617F
            
            // Main menu items
            this.MenuMainItemForeColor = ForeColor;
            this.MenuMainItemHoverForeColor = PrimaryColor;  // Tokyo cyan on hover
            this.MenuMainItemHoverBackColor = SurfaceColor;  // #24283B
            this.MenuMainItemSelectedForeColor = OnPrimaryColor;  // Dark text on cyan
            this.MenuMainItemSelectedBackColor = PrimaryColor;  // Tokyo cyan
            
            // Sub menu items
            this.MenuItemForeColor = ForeColor;
            this.MenuItemHoverForeColor = PrimaryColor;  // Tokyo cyan on hover
            this.MenuItemHoverBackColor = SurfaceColor;  // #24283B
            this.MenuItemSelectedForeColor = OnPrimaryColor;  // Dark text on cyan
            this.MenuItemSelectedBackColor = PrimaryColor;  // Tokyo cyan
            
            // Dark gradient
            this.MenuGradiantStartColor = PanelGradiantStartColor;
            this.MenuGradiantEndColor = PanelGradiantEndColor;
            this.MenuGradiantMiddleColor = PanelGradiantMiddleColor;
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}