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
            
            this.MenuBackColor = Color.FromArgb(242, 245, 248);  // Light gray-blue
            this.MenuForeColor = Color.FromArgb(60, 60, 60);  // Dark gray text
            this.MenuBorderColor = Color.FromArgb(220, 220, 220);  // Light gray
            
            // Main menu items
            this.MenuMainItemForeColor = Color.FromArgb(60, 60, 60);
            this.MenuMainItemHoverForeColor = Color.FromArgb(60, 60, 60);
            this.MenuMainItemHoverBackColor = Color.FromArgb(228, 232, 238);  // Darker hover
            this.MenuMainItemSelectedForeColor = Color.FromArgb(255, 255, 255);  // White text
            this.MenuMainItemSelectedBackColor = Color.FromArgb(136, 192, 208);  // Icy blue
            
            // Sub menu items
            this.MenuItemForeColor = Color.FromArgb(60, 60, 60);
            this.MenuItemHoverForeColor = Color.FromArgb(60, 60, 60);
            this.MenuItemHoverBackColor = Color.FromArgb(228, 232, 238);  // Darker hover
            this.MenuItemSelectedForeColor = Color.FromArgb(255, 255, 255);  // White text
            this.MenuItemSelectedBackColor = Color.FromArgb(136, 192, 208);  // Icy blue
            
            // Minimal gradient
            this.MenuGradiantStartColor = ThemeUtil.Lighten(BackgroundColor, 0.03);
            this.MenuGradiantEndColor = ThemeUtil.Darken(BackgroundColor, 0.04);
            this.MenuGradiantMiddleColor = ThemeUtil.Lighten(BackgroundColor, 0.01);
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}