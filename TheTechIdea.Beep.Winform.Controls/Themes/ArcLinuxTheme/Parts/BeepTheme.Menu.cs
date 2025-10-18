using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ArcLinuxTheme
    {
        private void ApplyMenu()
        {
            this.MenuBackColor = Color.FromArgb(245,246,247);
            this.MenuForeColor = Color.FromArgb(43,45,48);
            this.MenuBorderColor = Color.FromArgb(220,223,230);
            this.MenuMainItemForeColor = Color.FromArgb(43,45,48);
            this.MenuMainItemHoverForeColor = Color.FromArgb(43,45,48);
            this.MenuMainItemHoverBackColor = Color.FromArgb(245,246,247);
            this.MenuMainItemSelectedForeColor = Color.FromArgb(43,45,48);
            this.MenuMainItemSelectedBackColor = Color.FromArgb(245,246,247);
            this.MenuItemForeColor = Color.FromArgb(43,45,48);
            this.MenuItemHoverForeColor = Color.FromArgb(43,45,48);
            this.MenuItemHoverBackColor = Color.FromArgb(245,246,247);
            this.MenuItemSelectedForeColor = Color.FromArgb(43,45,48);
            this.MenuItemSelectedBackColor = Color.FromArgb(245,246,247);
            this.MenuGradiantStartColor = Color.FromArgb(245,246,247);
            this.MenuGradiantEndColor = Color.FromArgb(245,246,247);
            this.MenuGradiantMiddleColor = Color.FromArgb(245,246,247);
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}