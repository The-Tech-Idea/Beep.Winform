using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class TokyoTheme
    {
        private void ApplyMenu()
        {
            // Tokyo Night menu - inspired by Tokyo Night VSCode theme
            this.MenuBackColor = Color.FromArgb(26, 27, 38);  // Dark purple
            this.MenuForeColor = Color.FromArgb(169, 177, 214);  // Light purple-blue text
            this.MenuBorderColor = Color.FromArgb(86, 95, 137);  // #56617F
            
            // Main menu items
            this.MenuMainItemForeColor = Color.FromArgb(169, 177, 214);
            this.MenuMainItemHoverForeColor = Color.FromArgb(122, 162, 247);  // Tokyo cyan on hover
            this.MenuMainItemHoverBackColor = Color.FromArgb(36, 40, 59);  // #24283B
            this.MenuMainItemSelectedForeColor = Color.FromArgb(26, 27, 38);  // Dark text
            this.MenuMainItemSelectedBackColor = Color.FromArgb(122, 162, 247);  // Tokyo cyan
            
            // Sub menu items
            this.MenuItemForeColor = Color.FromArgb(169, 177, 214);
            this.MenuItemHoverForeColor = Color.FromArgb(122, 162, 247);  // Tokyo cyan on hover
            this.MenuItemHoverBackColor = Color.FromArgb(36, 40, 59);  // #24283B
            this.MenuItemSelectedForeColor = Color.FromArgb(26, 27, 38);  // Dark text
            this.MenuItemSelectedBackColor = Color.FromArgb(122, 162, 247);  // Tokyo cyan
            
            // Dark gradient
            this.MenuGradiantStartColor = Color.FromArgb(36, 40, 59);
            this.MenuGradiantEndColor = Color.FromArgb(36, 40, 59);
            this.MenuGradiantMiddleColor = Color.FromArgb(26, 27, 38);
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}