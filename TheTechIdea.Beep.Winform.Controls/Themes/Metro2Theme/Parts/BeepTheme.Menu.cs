using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class Metro2Theme
    {
        private void ApplyMenu()
        {
            // Metro2 menu - Windows Metro with accent stripe
            this.MenuBackColor = Color.FromArgb(240, 240, 240);  // Light gray
            this.MenuForeColor = Color.FromArgb(0, 0, 0);  // Black text
            this.MenuBorderColor = Color.FromArgb(0, 120, 215);  // Metro blue border
            
            // Main menu items
            this.MenuMainItemForeColor = Color.FromArgb(0, 0, 0);
            this.MenuMainItemHoverForeColor = Color.FromArgb(0, 0, 0);
            this.MenuMainItemHoverBackColor = Color.FromArgb(220, 220, 220);  // Darker gray hover
            this.MenuMainItemSelectedForeColor = Color.FromArgb(255, 255, 255);  // White text
            this.MenuMainItemSelectedBackColor = Color.FromArgb(0, 120, 215);  // Metro blue
            
            // Sub menu items
            this.MenuItemForeColor = Color.FromArgb(0, 0, 0);
            this.MenuItemHoverForeColor = Color.FromArgb(0, 0, 0);
            this.MenuItemHoverBackColor = Color.FromArgb(220, 220, 220);  // Darker gray hover
            this.MenuItemSelectedForeColor = Color.FromArgb(255, 255, 255);  // White text
            this.MenuItemSelectedBackColor = Color.FromArgb(0, 120, 215);  // Metro blue
            
            // Clean gradient
            this.MenuGradiantStartColor = Color.FromArgb(255, 255, 255);
            this.MenuGradiantEndColor = Color.FromArgb(240, 240, 240);
            this.MenuGradiantMiddleColor = Color.FromArgb(248, 248, 248);
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}