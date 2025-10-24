using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeonTheme
    {
        private void ApplyMenu()
        {
            this.MenuBackColor = Color.FromArgb(10,12,20);
            this.MenuForeColor = Color.FromArgb(235,245,255);
            this.MenuBorderColor = Color.FromArgb(60,70,100);
            this.MenuMainItemForeColor = Color.FromArgb(235,245,255);
            this.MenuMainItemHoverForeColor = Color.FromArgb(235,245,255);
            this.MenuMainItemHoverBackColor = Color.FromArgb(10,12,20);
            this.MenuMainItemSelectedForeColor = Color.FromArgb(235,245,255);
            this.MenuMainItemSelectedBackColor = Color.FromArgb(10,12,20);
            this.MenuItemForeColor = Color.FromArgb(235,245,255);
            this.MenuItemHoverForeColor = Color.FromArgb(235,245,255);
            this.MenuItemHoverBackColor = Color.FromArgb(10,12,20);
            this.MenuItemSelectedForeColor = Color.FromArgb(235,245,255);
            this.MenuItemSelectedBackColor = Color.FromArgb(10,12,20);
            this.MenuGradiantStartColor = Color.FromArgb(10,12,20);
            this.MenuGradiantEndColor = Color.FromArgb(10,12,20);
            this.MenuGradiantMiddleColor = Color.FromArgb(10,12,20);
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}