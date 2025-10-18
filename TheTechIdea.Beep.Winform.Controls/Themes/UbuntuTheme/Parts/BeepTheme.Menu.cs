using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class UbuntuTheme
    {
        private void ApplyMenu()
        {
            this.MenuBackColor = Color.FromArgb(242,242,245);
            this.MenuForeColor = Color.FromArgb(44,44,44);
            this.MenuBorderColor = Color.FromArgb(218,218,222);
            this.MenuMainItemForeColor = Color.FromArgb(44,44,44);
            this.MenuMainItemHoverForeColor = Color.FromArgb(44,44,44);
            this.MenuMainItemHoverBackColor = Color.FromArgb(242,242,245);
            this.MenuMainItemSelectedForeColor = Color.FromArgb(44,44,44);
            this.MenuMainItemSelectedBackColor = Color.FromArgb(242,242,245);
            this.MenuItemForeColor = Color.FromArgb(44,44,44);
            this.MenuItemHoverForeColor = Color.FromArgb(44,44,44);
            this.MenuItemHoverBackColor = Color.FromArgb(242,242,245);
            this.MenuItemSelectedForeColor = Color.FromArgb(44,44,44);
            this.MenuItemSelectedBackColor = Color.FromArgb(242,242,245);
            this.MenuGradiantStartColor = Color.FromArgb(242,242,245);
            this.MenuGradiantEndColor = Color.FromArgb(242,242,245);
            this.MenuGradiantMiddleColor = Color.FromArgb(242,242,245);
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}