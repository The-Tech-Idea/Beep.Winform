using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeoMorphismTheme
    {
        private void ApplyMenu()
        {
            // NeoMorphism menu - soft neomorphic theme
            this.MenuTitleFont = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14F, FontStyle.Bold);
            this.MenuItemSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12F, FontStyle.Bold);
            this.MenuItemUnSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12F, FontStyle.Regular);
            
            this.MenuBackColor = Color.FromArgb(240, 240, 245);  // Light gray-blue
            this.MenuForeColor = Color.FromArgb(50, 50, 60);  // Dark gray text
            this.MenuBorderColor = Color.FromArgb(220, 220, 225);  // Soft border
            
            // Main menu items
            this.MenuMainItemForeColor = Color.FromArgb(50, 50, 60);
            this.MenuMainItemHoverForeColor = Color.FromArgb(50, 50, 60);
            this.MenuMainItemHoverBackColor = Color.FromArgb(228, 228, 233);  // Darker hover
            this.MenuMainItemSelectedForeColor = Color.FromArgb(255, 255, 255);  // White text
            this.MenuMainItemSelectedBackColor = Color.FromArgb(76, 110, 245);  // Blue
            
            // Sub menu items
            this.MenuItemForeColor = Color.FromArgb(50, 50, 60);
            this.MenuItemHoverForeColor = Color.FromArgb(50, 50, 60);
            this.MenuItemHoverBackColor = Color.FromArgb(228, 228, 233);  // Darker hover
            this.MenuItemSelectedForeColor = Color.FromArgb(255, 255, 255);  // White text
            this.MenuItemSelectedBackColor = Color.FromArgb(76, 110, 245);  // Blue
            
            // Soft gradient
            this.MenuGradiantStartColor = ThemeUtil.Lighten(BackgroundColor, 0.06);
            this.MenuGradiantEndColor = ThemeUtil.Darken(BackgroundColor, 0.07);
            this.MenuGradiantMiddleColor = ThemeUtil.Lighten(BackgroundColor, 0.03);
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
        }
    }
}