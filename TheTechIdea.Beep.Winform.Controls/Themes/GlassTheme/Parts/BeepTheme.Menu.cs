using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GlassTheme
    {
        private void ApplyMenu()
        {
            // Glass menu - frosted glass theme
            this.MenuBackColor = Color.FromArgb(236, 244, 255);  // Light blue
            this.MenuForeColor = Color.FromArgb(17, 24, 39);  // Dark gray text
            this.MenuBorderColor = Color.FromArgb(200, 220, 240);  // Visible border
            
            // Main menu items
            this.MenuMainItemForeColor = Color.FromArgb(17, 24, 39);
            this.MenuMainItemHoverForeColor = Color.FromArgb(17, 24, 39);
            this.MenuMainItemHoverBackColor = Color.FromArgb(216, 234, 250);  // Light blue hover
            this.MenuMainItemSelectedForeColor = Color.FromArgb(17, 24, 39);
            this.MenuMainItemSelectedBackColor = Color.FromArgb(190, 220, 245);  // Medium blue selected
            
            // Sub menu items
            this.MenuItemForeColor = Color.FromArgb(17, 24, 39);
            this.MenuItemHoverForeColor = Color.FromArgb(17, 24, 39);
            this.MenuItemHoverBackColor = Color.FromArgb(216, 234, 250);  // Light blue hover
            this.MenuItemSelectedForeColor = Color.FromArgb(17, 24, 39);
            this.MenuItemSelectedBackColor = Color.FromArgb(190, 220, 245);  // Medium blue selected
            
            // Frosted gradient
            this.MenuGradiantStartColor = Color.FromArgb(240, 248, 255);
            this.MenuGradiantEndColor = Color.FromArgb(220, 235, 250);
            this.MenuGradiantMiddleColor = Color.FromArgb(230, 242, 255);
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}