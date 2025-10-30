using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class UbuntuTheme
    {
        private void ApplyMenu()
        {
            // Ubuntu menu - Ubuntu Linux desktop aesthetic
            this.MenuBackColor = Color.FromArgb(242, 242, 245);  // Light grey
            this.MenuForeColor = Color.FromArgb(44, 44, 44);  // Dark grey text
            this.MenuBorderColor = Color.FromArgb(218, 218, 222);  // Light grey
            
            // Main menu items
            this.MenuMainItemForeColor = Color.FromArgb(44, 44, 44);
            this.MenuMainItemHoverForeColor = Color.FromArgb(44, 44, 44);
            this.MenuMainItemHoverBackColor = Color.FromArgb(230, 230, 235);  // Darker hover
            this.MenuMainItemSelectedForeColor = Color.White;  // White text
            this.MenuMainItemSelectedBackColor = Color.FromArgb(233, 84, 32);  // Ubuntu orange
            
            // Sub menu items
            this.MenuItemForeColor = Color.FromArgb(44, 44, 44);
            this.MenuItemHoverForeColor = Color.FromArgb(44, 44, 44);
            this.MenuItemHoverBackColor = Color.FromArgb(230, 230, 235);  // Darker hover
            this.MenuItemSelectedForeColor = Color.White;  // White text
            this.MenuItemSelectedBackColor = Color.FromArgb(233, 84, 32);  // Ubuntu orange
            
            // Clean gradient
            this.MenuGradiantStartColor = ThemeUtil.Lighten(BackgroundColor, 0.02);
            this.MenuGradiantEndColor = ThemeUtil.Darken(BackgroundColor, 0.04);
            this.MenuGradiantMiddleColor = ThemeUtil.Lighten(BackgroundColor, 0.01);
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}