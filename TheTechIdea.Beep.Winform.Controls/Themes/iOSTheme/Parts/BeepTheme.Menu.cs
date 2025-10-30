using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class iOSTheme
    {
        private void ApplyMenu()
        {
            // iOS menu - clean, modern theme
            this.MenuBackColor = Color.FromArgb(242, 242, 247);  // Light gray
            this.MenuForeColor = Color.FromArgb(28, 28, 30);  // Dark gray text
            this.MenuBorderColor = Color.FromArgb(198, 198, 207);  // Medium gray
            
            // Main menu items
            this.MenuMainItemForeColor = Color.FromArgb(28, 28, 30);
            this.MenuMainItemHoverForeColor = Color.FromArgb(28, 28, 30);
            this.MenuMainItemHoverBackColor = Color.FromArgb(220, 220, 230);  // Darker gray hover
            this.MenuMainItemSelectedForeColor = Color.FromArgb(255, 255, 255);  // White text
            this.MenuMainItemSelectedBackColor = Color.FromArgb(10, 132, 255);  // iOS blue
            
            // Sub menu items
            this.MenuItemForeColor = Color.FromArgb(28, 28, 30);
            this.MenuItemHoverForeColor = Color.FromArgb(28, 28, 30);
            this.MenuItemHoverBackColor = Color.FromArgb(220, 220, 230);  // Darker gray hover
            this.MenuItemSelectedForeColor = Color.FromArgb(255, 255, 255);  // White text
            this.MenuItemSelectedBackColor = Color.FromArgb(10, 132, 255);  // iOS blue
            
            // Clean gradient
            this.MenuGradiantStartColor = Color.FromArgb(255, 255, 255);
            this.MenuGradiantEndColor = Color.FromArgb(252, 252, 252);
            this.MenuGradiantMiddleColor = Color.FromArgb(248, 248, 248);
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}