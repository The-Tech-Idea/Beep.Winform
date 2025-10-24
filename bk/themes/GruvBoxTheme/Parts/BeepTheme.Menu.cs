using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GruvBoxTheme
    {
        private void ApplyMenu()
        {
            this.MenuBackColor = Color.FromArgb(40,40,40);
            this.MenuForeColor = Color.FromArgb(235,219,178);
            this.MenuBorderColor = Color.FromArgb(168,153,132);
            this.MenuMainItemForeColor = Color.FromArgb(235,219,178);
            this.MenuMainItemHoverForeColor = Color.FromArgb(235,219,178);
            this.MenuMainItemHoverBackColor = Color.FromArgb(40,40,40);
            this.MenuMainItemSelectedForeColor = Color.FromArgb(235,219,178);
            this.MenuMainItemSelectedBackColor = Color.FromArgb(40,40,40);
            this.MenuItemForeColor = Color.FromArgb(235,219,178);
            this.MenuItemHoverForeColor = Color.FromArgb(235,219,178);
            this.MenuItemHoverBackColor = Color.FromArgb(40,40,40);
            this.MenuItemSelectedForeColor = Color.FromArgb(235,219,178);
            this.MenuItemSelectedBackColor = Color.FromArgb(40,40,40);
            this.MenuGradiantStartColor = Color.FromArgb(40,40,40);
            this.MenuGradiantEndColor = Color.FromArgb(40,40,40);
            this.MenuGradiantMiddleColor = Color.FromArgb(40,40,40);
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}