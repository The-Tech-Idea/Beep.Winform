using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CyberpunkTheme
    {
        private void ApplyMenu()
        {
            this.MenuBackColor = Color.FromArgb(10,8,20);
            this.MenuForeColor = Color.FromArgb(228,244,255);
            this.MenuBorderColor = Color.FromArgb(90,20,110);
            this.MenuMainItemForeColor = Color.FromArgb(228,244,255);
            this.MenuMainItemHoverForeColor = Color.FromArgb(228,244,255);
            this.MenuMainItemHoverBackColor = Color.FromArgb(10,8,20);
            this.MenuMainItemSelectedForeColor = Color.FromArgb(228,244,255);
            this.MenuMainItemSelectedBackColor = Color.FromArgb(10,8,20);
            this.MenuItemForeColor = Color.FromArgb(228,244,255);
            this.MenuItemHoverForeColor = Color.FromArgb(228,244,255);
            this.MenuItemHoverBackColor = Color.FromArgb(10,8,20);
            this.MenuItemSelectedForeColor = Color.FromArgb(228,244,255);
            this.MenuItemSelectedBackColor = Color.FromArgb(10,8,20);
            this.MenuGradiantStartColor = Color.FromArgb(10,8,20);
            this.MenuGradiantEndColor = Color.FromArgb(10,8,20);
            this.MenuGradiantMiddleColor = Color.FromArgb(10,8,20);
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
        }
    }
}