using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeonTheme
    {
        private void ApplyMenu()
        {
            // Neon menu - vibrant neon theme
            this.MenuBackColor = Color.FromArgb(15, 15, 25);  // Deep navy
            this.MenuForeColor = Color.FromArgb(0, 255, 255);  // Cyan text
            this.MenuBorderColor = Color.FromArgb(0, 255, 255);  // Cyan glow
            
            // Main menu items
            this.MenuMainItemForeColor = Color.FromArgb(0, 255, 255);
            this.MenuMainItemHoverForeColor = Color.FromArgb(100, 255, 255);  // Brighter cyan on hover
            this.MenuMainItemHoverBackColor = Color.FromArgb(25, 25, 40);  // Lighter navy
            this.MenuMainItemSelectedForeColor = Color.FromArgb(15, 15, 25);  // Dark text
            this.MenuMainItemSelectedBackColor = Color.FromArgb(0, 255, 255);  // Cyan background
            
            // Sub menu items
            this.MenuItemForeColor = Color.FromArgb(0, 255, 255);
            this.MenuItemHoverForeColor = Color.FromArgb(100, 255, 255);  // Brighter cyan on hover
            this.MenuItemHoverBackColor = Color.FromArgb(25, 25, 40);  // Lighter navy
            this.MenuItemSelectedForeColor = Color.FromArgb(15, 15, 25);  // Dark text
            this.MenuItemSelectedBackColor = Color.FromArgb(0, 255, 255);  // Cyan background
            
            // Dark gradient
            this.MenuGradiantStartColor = Color.FromArgb(20, 24, 38);
            this.MenuGradiantEndColor = Color.FromArgb(15, 18, 30);
            this.MenuGradiantMiddleColor = Color.FromArgb(10, 12, 20);
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}