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
            this.MenuBackColor = Color.FromArgb(230,0,126);
            this.MenuForeColor = Color.FromArgb(32,31,30);
            this.MenuBorderColor = Color.FromArgb(220,220,220);
            this.MenuMainItemForeColor = Color.FromArgb(32,31,30);
            this.MenuMainItemHoverForeColor = Color.FromArgb(32,31,30);
            this.MenuMainItemHoverBackColor = Color.FromArgb(230,0,126);
            this.MenuMainItemSelectedForeColor = Color.FromArgb(32,31,30);
            this.MenuMainItemSelectedBackColor = Color.FromArgb(230,0,126);
            this.MenuItemForeColor = Color.FromArgb(32,31,30);
            this.MenuItemHoverForeColor = Color.FromArgb(32,31,30);
            this.MenuItemHoverBackColor = Color.FromArgb(230,0,126);
            this.MenuItemSelectedForeColor = Color.FromArgb(32,31,30);
            this.MenuItemSelectedBackColor = Color.FromArgb(230,0,126);
            this.MenuGradiantStartColor = Color.FromArgb(243,242,241);
            this.MenuGradiantEndColor = Color.FromArgb(243,242,241);
            this.MenuGradiantMiddleColor = Color.FromArgb(243,242,241);
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}