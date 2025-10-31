using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ModernTheme
    {
        private void ApplyMenu()
        {
            // Modern menu - clean, modern theme
            this.MenuBackColor = Color.FromArgb(255, 255, 255);  // White
            this.MenuForeColor = Color.FromArgb(17, 24, 39);  // Dark gray text
            this.MenuBorderColor = Color.FromArgb(203, 213, 225);  // Light gray
            
            // Main menu items
            this.MenuMainItemForeColor = Color.FromArgb(17, 24, 39);
            this.MenuMainItemHoverForeColor = Color.FromArgb(17, 24, 39);
            this.MenuMainItemHoverBackColor = Color.FromArgb(240, 245, 250);  // Light blue-gray hover
            this.MenuMainItemSelectedForeColor = Color.FromArgb(255, 255, 255);  // White text
            this.MenuMainItemSelectedBackColor = Color.FromArgb(99, 102, 241);  // Indigo
            
            // Sub menu items
            this.MenuItemForeColor = Color.FromArgb(17, 24, 39);
            this.MenuItemHoverForeColor = Color.FromArgb(17, 24, 39);
            this.MenuItemHoverBackColor = Color.FromArgb(240, 245, 250);  // Light blue-gray hover
            this.MenuItemSelectedForeColor = Color.FromArgb(255, 255, 255);  // White text
            this.MenuItemSelectedBackColor = Color.FromArgb(99, 102, 241);  // Indigo
            
            // Clean gradient
            this.MenuGradiantStartColor = Color.FromArgb(255, 255, 255);
            this.MenuGradiantEndColor = Color.FromArgb(248, 250, 252);
            this.MenuGradiantMiddleColor = Color.FromArgb(252, 252, 254);
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}