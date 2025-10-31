using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class SolarizedTheme
    {
        private void ApplyMenu()
        {
            // Solarized menu - scientifically crafted color palette
            this.MenuBackColor = Color.FromArgb(0, 43, 54);  // Dark blue-green
            this.MenuForeColor = Color.FromArgb(238, 232, 213);  // Light beige text
            this.MenuBorderColor = Color.FromArgb(88, 110, 117);  // #586E75
            
            // Main menu items
            this.MenuMainItemForeColor = Color.FromArgb(238, 232, 213);
            this.MenuMainItemHoverForeColor = Color.FromArgb(42, 161, 152);  // Cyan on hover
            this.MenuMainItemHoverBackColor = Color.FromArgb(7, 54, 66);  // #073642
            this.MenuMainItemSelectedForeColor = Color.FromArgb(238, 232, 213);  // Light text
            this.MenuMainItemSelectedBackColor = Color.FromArgb(203, 75, 22);  // Orange
            
            // Sub menu items
            this.MenuItemForeColor = Color.FromArgb(238, 232, 213);
            this.MenuItemHoverForeColor = Color.FromArgb(42, 161, 152);  // Cyan on hover
            this.MenuItemHoverBackColor = Color.FromArgb(7, 54, 66);  // #073642
            this.MenuItemSelectedForeColor = Color.FromArgb(238, 232, 213);  // Light text
            this.MenuItemSelectedBackColor = Color.FromArgb(203, 75, 22);  // Orange
            
            // Dark gradient
            this.MenuGradiantStartColor = Color.FromArgb(0, 43, 54);
            this.MenuGradiantEndColor = Color.FromArgb(0, 43, 54);
            this.MenuGradiantMiddleColor = Color.FromArgb(0, 43, 54);
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}