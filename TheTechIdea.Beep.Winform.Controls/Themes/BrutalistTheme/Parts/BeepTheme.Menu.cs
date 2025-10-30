using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class BrutalistTheme
    {
        private void ApplyMenu()
        {
            // Brutalist menu - white with black borders
            this.MenuBackColor = Color.FromArgb(255, 255, 255);  // Pure white
            this.MenuForeColor = Color.FromArgb(0, 0, 0);  // Black text
            this.MenuBorderColor = Color.FromArgb(0, 0, 0);  // Black border
            
            // Main menu items
            this.MenuMainItemForeColor = Color.FromArgb(0, 0, 0);
            this.MenuMainItemHoverForeColor = Color.FromArgb(0, 0, 0);
            this.MenuMainItemHoverBackColor = Color.FromArgb(240, 240, 240);  // Light gray hover
            this.MenuMainItemSelectedForeColor = Color.FromArgb(0, 0, 0);
            this.MenuMainItemSelectedBackColor = Color.FromArgb(200, 200, 200);  // Medium gray selected
            
            // Sub menu items
            this.MenuItemForeColor = Color.FromArgb(0, 0, 0);
            this.MenuItemHoverForeColor = Color.FromArgb(0, 0, 0);
            this.MenuItemHoverBackColor = Color.FromArgb(240, 240, 240);  // Light gray hover
            this.MenuItemSelectedForeColor = Color.FromArgb(0, 0, 0);
            this.MenuItemSelectedBackColor = Color.FromArgb(200, 200, 200);  // Medium gray selected
            
            // No gradient for brutalist aesthetic
            this.MenuGradiantStartColor = Color.FromArgb(255, 255, 255);
            this.MenuGradiantEndColor = Color.FromArgb(255, 255, 255);
            this.MenuGradiantMiddleColor = Color.FromArgb(255, 255, 255);
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}