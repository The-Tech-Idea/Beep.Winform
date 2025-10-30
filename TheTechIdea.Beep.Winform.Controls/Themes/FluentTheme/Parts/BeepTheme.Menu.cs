using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class FluentTheme
    {
        private void ApplyMenu()
        {
            // Fluent menu - light, modern
            this.MenuBackColor = Color.FromArgb(245, 246, 248);  // Light gray-blue
            this.MenuForeColor = Color.FromArgb(32, 32, 32);  // Dark gray text
            this.MenuBorderColor = Color.FromArgb(218, 223, 230);  // Light gray
            
            // Main menu items
            this.MenuMainItemForeColor = Color.FromArgb(32, 32, 32);
            this.MenuMainItemHoverForeColor = Color.FromArgb(32, 32, 32);
            this.MenuMainItemHoverBackColor = Color.FromArgb(230, 235, 240);  // Light blue-gray hover
            this.MenuMainItemSelectedForeColor = Color.FromArgb(0, 80, 160);  // Blue text
            this.MenuMainItemSelectedBackColor = Color.FromArgb(220, 230, 245);  // Light blue
            
            // Sub menu items
            this.MenuItemForeColor = Color.FromArgb(32, 32, 32);
            this.MenuItemHoverForeColor = Color.FromArgb(32, 32, 32);
            this.MenuItemHoverBackColor = Color.FromArgb(230, 235, 240);  // Light blue-gray hover
            this.MenuItemSelectedForeColor = Color.FromArgb(0, 80, 160);  // Blue text
            this.MenuItemSelectedBackColor = Color.FromArgb(220, 230, 245);  // Light blue
            
            // Subtle gradient
            this.MenuGradiantStartColor = Color.FromArgb(245, 246, 248);
            this.MenuGradiantEndColor = Color.FromArgb(235, 237, 240);
            this.MenuGradiantMiddleColor = Color.FromArgb(240, 242, 245);
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}