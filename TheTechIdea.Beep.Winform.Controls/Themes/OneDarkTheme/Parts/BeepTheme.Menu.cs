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


            this.MenuBackColor = Color.FromArgb(40,44,52);
            this.MenuForeColor = Color.FromArgb(171,178,191);
            this.MenuBorderColor = Color.FromArgb(92,99,112);
            this.MenuMainItemForeColor = Color.FromArgb(171,178,191);
            this.MenuMainItemHoverForeColor = Color.FromArgb(171,178,191);
            this.MenuMainItemHoverBackColor = Color.FromArgb(40,44,52);
            this.MenuMainItemSelectedForeColor = Color.FromArgb(171,178,191);
            this.MenuMainItemSelectedBackColor = Color.FromArgb(40,44,52);
            this.MenuItemForeColor = Color.FromArgb(171,178,191);
            this.MenuItemHoverForeColor = Color.FromArgb(171,178,191);
            this.MenuItemHoverBackColor = Color.FromArgb(40,44,52);
            this.MenuItemSelectedForeColor = Color.FromArgb(171,178,191);
            this.MenuItemSelectedBackColor = Color.FromArgb(40,44,52);
            this.MenuGradiantStartColor =Color.FromArgb(25, 25, 25);
            this.MenuGradiantEndColor = Color.FromArgb(40, 40, 40);
            this.MenuGradiantMiddleColor =  Color.FromArgb(30, 30, 30);
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}