using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MinimalTheme
    {
        private void ApplyMenu()
        {
            // Minimal menu - clean, minimal aesthetic
            this.MenuBackColor = BackgroundColor;  // White
            this.MenuForeColor = ForeColor;  // Dark grey text
            this.MenuBorderColor = BorderColor;  // Light grey
            
            // Main menu items
            this.MenuMainItemForeColor = ForeColor;
            this.MenuMainItemHoverForeColor = ForeColor;
            this.MenuMainItemHoverBackColor = Color.FromArgb(245, 245, 245);  // SurfaceColor (light grey)
            this.MenuMainItemSelectedForeColor = ForeColor;
            this.MenuMainItemSelectedBackColor = Color.FromArgb(230, 230, 230);  // Medium grey
            
            // Sub menu items
            this.MenuItemForeColor = ForeColor;
            this.MenuItemHoverForeColor = ForeColor;
            this.MenuItemHoverBackColor = Color.FromArgb(245, 245, 245);  // SurfaceColor (light grey)
            this.MenuItemSelectedForeColor = ForeColor;
            this.MenuItemSelectedBackColor = Color.FromArgb(230, 230, 230);  // Medium grey
            
            // Clean gradient
            this.MenuGradiantStartColor = BackgroundColor;
            this.MenuGradiantEndColor = BackgroundColor;
            this.MenuGradiantMiddleColor = BackgroundColor;
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
