using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MetroTheme
    {
        private void ApplyMenu()
        {
            // Metro menu - Windows Metro theme
            this.MenuBackColor = Color.FromArgb(243, 242, 241);  // Light gray
            this.MenuForeColor = Color.FromArgb(32, 31, 30);  // Dark text
            this.MenuBorderColor = Color.FromArgb(225, 225, 225);  // Light gray
            
            // Main menu items
            this.MenuMainItemForeColor = Color.FromArgb(32, 31, 30);
            this.MenuMainItemHoverForeColor = Color.FromArgb(32, 31, 30);
            this.MenuMainItemHoverBackColor = Color.FromArgb(220, 220, 220);  // Darker gray hover
            this.MenuMainItemSelectedForeColor = Color.FromArgb(255, 255, 255);  // White text
            this.MenuMainItemSelectedBackColor = Color.FromArgb(0, 120, 215);  // Metro blue
            
            // Sub menu items
            this.MenuItemForeColor = Color.FromArgb(32, 31, 30);
            this.MenuItemHoverForeColor = Color.FromArgb(32, 31, 30);
            this.MenuItemHoverBackColor = Color.FromArgb(220, 220, 220);  // Darker gray hover
            this.MenuItemSelectedForeColor = Color.FromArgb(255, 255, 255);  // White text
            this.MenuItemSelectedBackColor = Color.FromArgb(0, 120, 215);  // Metro blue
            
            // Clean gradient
            this.MenuGradiantStartColor = Color.FromArgb(255, 255, 255);
            this.MenuGradiantEndColor = Color.FromArgb(243, 242, 241);
            this.MenuGradiantMiddleColor = Color.FromArgb(250, 250, 249);
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}