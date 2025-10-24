using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordTheme
    {
        private void ApplyMenu()
        {
            this.MenuBackColor = Color.FromArgb(46,52,64);
            this.MenuForeColor = Color.FromArgb(216,222,233);
            this.MenuBorderColor = Color.FromArgb(76,86,106);
            this.MenuMainItemForeColor = Color.FromArgb(216,222,233);
            this.MenuMainItemHoverForeColor = Color.FromArgb(216,222,233);
            this.MenuMainItemHoverBackColor = Color.FromArgb(46,52,64);
            this.MenuMainItemSelectedForeColor = Color.FromArgb(216,222,233);
            this.MenuMainItemSelectedBackColor = Color.FromArgb(46,52,64);
            this.MenuItemForeColor = Color.FromArgb(216,222,233);
            this.MenuItemHoverForeColor = Color.FromArgb(216,222,233);
            this.MenuItemHoverBackColor = Color.FromArgb(46,52,64);
            this.MenuItemSelectedForeColor = Color.FromArgb(216,222,233);
            this.MenuItemSelectedBackColor = Color.FromArgb(46,52,64);
            this.MenuGradiantStartColor = Color.FromArgb(46,52,64);
            this.MenuGradiantEndColor = Color.FromArgb(46,52,64);
            this.MenuGradiantMiddleColor = Color.FromArgb(46,52,64);
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}