using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class HolographicTheme
    {
        private void ApplyMenu()
        {
            // Holographic menu - futuristic gradient theme
            this.MenuBackColor = Color.FromArgb(25, 20, 35);  // Very dark purple
            this.MenuForeColor = Color.FromArgb(200, 150, 255);  // Light purple text
            this.MenuBorderColor = Color.FromArgb(138, 70, 255);  // Purple glow
            
            // Main menu items
            this.MenuMainItemForeColor = Color.FromArgb(200, 150, 255);
            this.MenuMainItemHoverForeColor = Color.FromArgb(255, 122, 217);  // Pink on hover
            this.MenuMainItemHoverBackColor = Color.FromArgb(50, 40, 70);  // Lighter purple
            this.MenuMainItemSelectedForeColor = Color.FromArgb(122, 252, 255);  // Cyan
            this.MenuMainItemSelectedBackColor = Color.FromArgb(60, 50, 90);  // Medium purple
            
            // Sub menu items
            this.MenuItemForeColor = Color.FromArgb(200, 150, 255);
            this.MenuItemHoverForeColor = Color.FromArgb(255, 122, 217);  // Pink on hover
            this.MenuItemHoverBackColor = Color.FromArgb(50, 40, 70);  // Lighter purple
            this.MenuItemSelectedForeColor = Color.FromArgb(122, 252, 255);  // Cyan
            this.MenuItemSelectedBackColor = Color.FromArgb(60, 50, 90);  // Medium purple
            
            // Gradient effect
            this.MenuGradiantStartColor = Color.FromArgb(255, 122, 217);  // Pink
            this.MenuGradiantEndColor = Color.FromArgb(122, 252, 255);  // Cyan
            this.MenuGradiantMiddleColor = Color.FromArgb(176, 141, 255);  // Purple
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
        }
    }
}