using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordTheme
    {
        private void ApplyMenu()
        {
            // Nord menu - Arctic-inspired dark theme
            this.MenuBackColor = Color.FromArgb(46, 52, 64);  // Dark blue-gray
            this.MenuForeColor = Color.FromArgb(216, 222, 233);  // Light gray-blue text
            this.MenuBorderColor = Color.FromArgb(67, 76, 94);  // #434C5E
            
            // Main menu items
            this.MenuMainItemForeColor = Color.FromArgb(216, 222, 233);
            this.MenuMainItemHoverForeColor = Color.FromArgb(136, 192, 208);  // Nord cyan on hover
            this.MenuMainItemHoverBackColor = Color.FromArgb(59, 66, 82);  // Lighter gray
            this.MenuMainItemSelectedForeColor = Color.FromArgb(46, 52, 64);  // Dark text
            this.MenuMainItemSelectedBackColor = Color.FromArgb(136, 192, 208);  // Nord cyan
            
            // Sub menu items
            this.MenuItemForeColor = Color.FromArgb(216, 222, 233);
            this.MenuItemHoverForeColor = Color.FromArgb(136, 192, 208);  // Nord cyan on hover
            this.MenuItemHoverBackColor = Color.FromArgb(59, 66, 82);  // Lighter gray
            this.MenuItemSelectedForeColor = Color.FromArgb(46, 52, 64);  // Dark text
            this.MenuItemSelectedBackColor = Color.FromArgb(136, 192, 208);  // Nord cyan
            
            // Dark gradient
            this.MenuGradiantStartColor = Color.FromArgb(59, 66, 82);
            this.MenuGradiantEndColor = Color.FromArgb(59, 66, 82);
            this.MenuGradiantMiddleColor = Color.FromArgb(46, 52, 64);
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}