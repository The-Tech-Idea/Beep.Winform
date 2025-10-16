using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CartoonTheme
    {
        private void ApplyMenu()
        {
            this.MenuBackColor = Color.FromArgb(255,251,235);
            this.MenuForeColor = Color.FromArgb(33,37,41);
            this.MenuBorderColor = Color.FromArgb(247,208,136);
            this.MenuMainItemForeColor = Color.FromArgb(33,37,41);
            this.MenuMainItemHoverForeColor = Color.FromArgb(33,37,41);
            this.MenuMainItemHoverBackColor = Color.FromArgb(255,251,235);
            this.MenuMainItemSelectedForeColor = Color.FromArgb(33,37,41);
            this.MenuMainItemSelectedBackColor = Color.FromArgb(255,251,235);
            this.MenuItemForeColor = Color.FromArgb(33,37,41);
            this.MenuItemHoverForeColor = Color.FromArgb(33,37,41);
            this.MenuItemHoverBackColor = Color.FromArgb(255,251,235);
            this.MenuItemSelectedForeColor = Color.FromArgb(33,37,41);
            this.MenuItemSelectedBackColor = Color.FromArgb(255,251,235);
            this.MenuGradiantStartColor = Color.FromArgb(255,251,235);
            this.MenuGradiantEndColor = Color.FromArgb(255,251,235);
            this.MenuGradiantMiddleColor = Color.FromArgb(255,251,235);
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
        }
    }
}