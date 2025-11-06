using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GruvBoxTheme
    {
        private void ApplyMenu()
        {
            // GruvBox menu - warm retro theme
            this.MenuTitleFont = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 14F, FontStyle.Bold);
            this.MenuItemSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 12F, FontStyle.Bold);
            this.MenuItemUnSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 12F, FontStyle.Regular);
            
            this.MenuBackColor = Color.FromArgb(40, 40, 40);  // Dark gray
            this.MenuForeColor = Color.FromArgb(235, 219, 178);  // Beige text
            this.MenuBorderColor = Color.FromArgb(80, 73, 69);  // Muted brown
            
            // Main menu items
            this.MenuMainItemForeColor = Color.FromArgb(235, 219, 178);
            this.MenuMainItemHoverForeColor = Color.FromArgb(250, 189, 47);  // Yellow on hover
            this.MenuMainItemHoverBackColor = Color.FromArgb(50, 48, 47);  // Lighter gray
            this.MenuMainItemSelectedForeColor = Color.FromArgb(254, 128, 25);  // Orange
            this.MenuMainItemSelectedBackColor = Color.FromArgb(60, 56, 54);  // Medium brown
            
            // Sub menu items
            this.MenuItemForeColor = Color.FromArgb(235, 219, 178);
            this.MenuItemHoverForeColor = Color.FromArgb(250, 189, 47);  // Yellow on hover
            this.MenuItemHoverBackColor = Color.FromArgb(50, 48, 47);  // Lighter gray
            this.MenuItemSelectedForeColor = Color.FromArgb(254, 128, 25);  // Orange
            this.MenuItemSelectedBackColor = Color.FromArgb(60, 56, 54);  // Medium brown
            
            // Dark gradient
            this.MenuGradiantStartColor = Color.FromArgb(60, 56, 54);
            this.MenuGradiantEndColor = Color.FromArgb(50, 48, 47);
            this.MenuGradiantMiddleColor = Color.FromArgb(40, 40, 40);
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}