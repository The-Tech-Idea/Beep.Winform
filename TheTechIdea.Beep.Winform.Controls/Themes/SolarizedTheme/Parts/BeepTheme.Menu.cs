using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class SolarizedTheme
    {
        private void ApplyMenu()
        {
            this.MenuBackColor = Color.FromArgb(0,43,54);
            this.MenuForeColor = Color.FromArgb(147,161,161);
            this.MenuBorderColor = Color.FromArgb(88,110,117);
            this.MenuMainItemForeColor = Color.FromArgb(147,161,161);
            this.MenuMainItemHoverForeColor = Color.FromArgb(147,161,161);
            this.MenuMainItemHoverBackColor = Color.FromArgb(0,43,54);
            this.MenuMainItemSelectedForeColor = Color.FromArgb(147,161,161);
            this.MenuMainItemSelectedBackColor = Color.FromArgb(0,43,54);
            this.MenuItemForeColor = Color.FromArgb(147,161,161);
            this.MenuItemHoverForeColor = Color.FromArgb(147,161,161);
            this.MenuItemHoverBackColor = Color.FromArgb(0,43,54);
            this.MenuItemSelectedForeColor = Color.FromArgb(147,161,161);
            this.MenuItemSelectedBackColor = Color.FromArgb(0,43,54);
            this.MenuGradiantStartColor = Color.FromArgb(0,43,54);
            this.MenuGradiantEndColor = Color.FromArgb(0,43,54);
            this.MenuGradiantMiddleColor = Color.FromArgb(0,43,54);
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}