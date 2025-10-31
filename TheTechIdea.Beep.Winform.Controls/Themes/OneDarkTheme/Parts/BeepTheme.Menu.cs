using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class OneDarkTheme
    {
        private void ApplyMenu()
        {   
            
            this.MenuTitleFont= ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14F, FontStyle.Bold);
        this.MenuItemSelectedFont  = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12F, FontStyle.Bold);
        this.MenuItemUnSelectedFont= ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12F, FontStyle.Regular);


            // One Dark menu - popular dark theme
            this.MenuBackColor = Color.FromArgb(40, 44, 52);  // Dark background
            this.MenuForeColor = Color.FromArgb(171, 178, 191);  // Warm grey text
            this.MenuBorderColor = Color.FromArgb(60, 66, 82);  // #3C4252
            
            // Main menu items
            this.MenuMainItemForeColor = Color.FromArgb(171, 178, 191);
            this.MenuMainItemHoverForeColor = Color.FromArgb(97, 175, 239);  // One Dark blue on hover
            this.MenuMainItemHoverBackColor = Color.FromArgb(47, 51, 61);  // Lighter dark
            this.MenuMainItemSelectedForeColor = Color.FromArgb(40, 44, 52);  // Dark text
            this.MenuMainItemSelectedBackColor = Color.FromArgb(97, 175, 239);  // One Dark blue
            
            // Sub menu items
            this.MenuItemForeColor = Color.FromArgb(171, 178, 191);
            this.MenuItemHoverForeColor = Color.FromArgb(97, 175, 239);  // One Dark blue on hover
            this.MenuItemHoverBackColor = Color.FromArgb(47, 51, 61);  // Lighter dark
            this.MenuItemSelectedForeColor = Color.FromArgb(40, 44, 52);  // Dark text
            this.MenuItemSelectedBackColor = Color.FromArgb(97, 175, 239);  // One Dark blue
            
            // Dark gradient
            this.MenuGradiantStartColor = Color.FromArgb(25, 25, 25);
            this.MenuGradiantEndColor = Color.FromArgb(40, 40, 40);
            this.MenuGradiantMiddleColor = Color.FromArgb(30, 30, 30);
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}