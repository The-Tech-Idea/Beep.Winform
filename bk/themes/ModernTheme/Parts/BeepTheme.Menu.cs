using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ModernTheme
    {
        private void ApplyMenu()
        {
            this.MenuBackColor = Color.FromArgb(255,255,255);
            this.MenuForeColor = Color.FromArgb(17,24,39);
            this.MenuBorderColor = Color.FromArgb(203,213,225);
            this.MenuMainItemForeColor = Color.FromArgb(17,24,39);
            this.MenuMainItemHoverForeColor = Color.FromArgb(17,24,39);
            this.MenuMainItemHoverBackColor = Color.FromArgb(255,255,255);
            this.MenuMainItemSelectedForeColor = Color.FromArgb(17,24,39);
            this.MenuMainItemSelectedBackColor = Color.FromArgb(255,255,255);
            this.MenuItemForeColor = Color.FromArgb(17,24,39);
            this.MenuItemHoverForeColor = Color.FromArgb(17,24,39);
            this.MenuItemHoverBackColor = Color.FromArgb(255,255,255);
            this.MenuItemSelectedForeColor = Color.FromArgb(17,24,39);
            this.MenuItemSelectedBackColor = Color.FromArgb(255,255,255);
            this.MenuGradiantStartColor = Color.FromArgb(255,255,255);
            this.MenuGradiantEndColor = Color.FromArgb(255,255,255);
            this.MenuGradiantMiddleColor = Color.FromArgb(255,255,255);
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}