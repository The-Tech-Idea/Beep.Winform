using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class TokyoTheme
    {
        private void ApplyMenu()
        {
            this.MenuBackColor = Color.FromArgb(26,27,38);
            this.MenuForeColor = Color.FromArgb(192,202,245);
            this.MenuBorderColor = Color.FromArgb(86,95,137);
            this.MenuMainItemForeColor = Color.FromArgb(192,202,245);
            this.MenuMainItemHoverForeColor = Color.FromArgb(192,202,245);
            this.MenuMainItemHoverBackColor = Color.FromArgb(26,27,38);
            this.MenuMainItemSelectedForeColor = Color.FromArgb(192,202,245);
            this.MenuMainItemSelectedBackColor = Color.FromArgb(26,27,38);
            this.MenuItemForeColor = Color.FromArgb(192,202,245);
            this.MenuItemHoverForeColor = Color.FromArgb(192,202,245);
            this.MenuItemHoverBackColor = Color.FromArgb(26,27,38);
            this.MenuItemSelectedForeColor = Color.FromArgb(192,202,245);
            this.MenuItemSelectedBackColor = Color.FromArgb(26,27,38);
            this.MenuGradiantStartColor = Color.FromArgb(26,27,38);
            this.MenuGradiantEndColor = Color.FromArgb(26,27,38);
            this.MenuGradiantMiddleColor = Color.FromArgb(26,27,38);
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}