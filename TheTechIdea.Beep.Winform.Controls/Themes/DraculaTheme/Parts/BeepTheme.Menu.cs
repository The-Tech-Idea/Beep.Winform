using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class DraculaTheme
    {
        private void ApplyMenu()
        {
            // Dracula menu - dark purple theme
            this.MenuBackColor = Color.FromArgb(40, 42, 54);  // Dark background
            this.MenuForeColor = Color.FromArgb(248, 248, 242);  // Cream text
            this.MenuBorderColor = Color.FromArgb(68, 71, 90);  // Gray border
            
            // Main menu items
            this.MenuMainItemForeColor = Color.FromArgb(248, 248, 242);
            this.MenuMainItemHoverForeColor = Color.FromArgb(255, 121, 198);  // Pink on hover
            this.MenuMainItemHoverBackColor = Color.FromArgb(52, 55, 72);  // Lighter purple
            this.MenuMainItemSelectedForeColor = Color.FromArgb(255, 121, 198);
            this.MenuMainItemSelectedBackColor = Color.FromArgb(68, 71, 90);  // Medium purple
            
            // Sub menu items
            this.MenuItemForeColor = Color.FromArgb(248, 248, 242);
            this.MenuItemHoverForeColor = Color.FromArgb(255, 121, 198);  // Pink on hover
            this.MenuItemHoverBackColor = Color.FromArgb(52, 55, 72);  // Lighter purple
            this.MenuItemSelectedForeColor = Color.FromArgb(255, 121, 198);
            this.MenuItemSelectedBackColor = Color.FromArgb(68, 71, 90);  // Medium purple
            
            // Purple gradient
            this.MenuGradiantStartColor = Color.FromArgb(68, 71, 90);
            this.MenuGradiantEndColor = Color.FromArgb(52, 55, 72);
            this.MenuGradiantMiddleColor = Color.FromArgb(60, 63, 82);
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
        }
    }
}