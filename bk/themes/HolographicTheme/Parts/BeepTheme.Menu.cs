using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class HolographicTheme
    {
        private void ApplyMenu()
        {
            this.MenuBackColor = Color.FromArgb(15,16,32);
            this.MenuForeColor = Color.FromArgb(245,247,255);
            this.MenuBorderColor = Color.FromArgb(74,79,123);
            this.MenuMainItemForeColor = Color.FromArgb(245,247,255);
            this.MenuMainItemHoverForeColor = Color.FromArgb(245,247,255);
            this.MenuMainItemHoverBackColor = Color.FromArgb(15,16,32);
            this.MenuMainItemSelectedForeColor = Color.FromArgb(245,247,255);
            this.MenuMainItemSelectedBackColor = Color.FromArgb(15,16,32);
            this.MenuItemForeColor = Color.FromArgb(245,247,255);
            this.MenuItemHoverForeColor = Color.FromArgb(245,247,255);
            this.MenuItemHoverBackColor = Color.FromArgb(15,16,32);
            this.MenuItemSelectedForeColor = Color.FromArgb(245,247,255);
            this.MenuItemSelectedBackColor = Color.FromArgb(15,16,32);
            this.MenuGradiantStartColor = Color.FromArgb(15,16,32);
            this.MenuGradiantEndColor = Color.FromArgb(15,16,32);
            this.MenuGradiantMiddleColor = Color.FromArgb(15,16,32);
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
        }
    }
}