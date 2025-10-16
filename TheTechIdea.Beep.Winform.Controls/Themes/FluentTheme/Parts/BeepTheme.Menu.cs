using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class FluentTheme
    {
        private void ApplyMenu()
        {
            this.MenuBackColor = Color.FromArgb(245,246,248);
            this.MenuForeColor = Color.FromArgb(32,32,32);
            this.MenuBorderColor = Color.FromArgb(218,223,230);
            this.MenuMainItemForeColor = Color.FromArgb(32,32,32);
            this.MenuMainItemHoverForeColor = Color.FromArgb(32,32,32);
            this.MenuMainItemHoverBackColor = Color.FromArgb(245,246,248);
            this.MenuMainItemSelectedForeColor = Color.FromArgb(32,32,32);
            this.MenuMainItemSelectedBackColor = Color.FromArgb(245,246,248);
            this.MenuItemForeColor = Color.FromArgb(32,32,32);
            this.MenuItemHoverForeColor = Color.FromArgb(32,32,32);
            this.MenuItemHoverBackColor = Color.FromArgb(245,246,248);
            this.MenuItemSelectedForeColor = Color.FromArgb(32,32,32);
            this.MenuItemSelectedBackColor = Color.FromArgb(245,246,248);
            this.MenuGradiantStartColor = Color.FromArgb(245,246,248);
            this.MenuGradiantEndColor = Color.FromArgb(245,246,248);
            this.MenuGradiantMiddleColor = Color.FromArgb(245,246,248);
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}