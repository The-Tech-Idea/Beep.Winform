using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GNOMETheme
    {
        private void ApplyMenu()
        {
            this.MenuBackColor = Color.FromArgb(246,245,244);
            this.MenuForeColor = Color.FromArgb(46,52,54);
            this.MenuBorderColor = Color.FromArgb(205,207,212);
            this.MenuMainItemForeColor = Color.FromArgb(46,52,54);
            this.MenuMainItemHoverForeColor = Color.FromArgb(46,52,54);
            this.MenuMainItemHoverBackColor = Color.FromArgb(246,245,244);
            this.MenuMainItemSelectedForeColor = Color.FromArgb(46,52,54);
            this.MenuMainItemSelectedBackColor = Color.FromArgb(246,245,244);
            this.MenuItemForeColor = Color.FromArgb(46,52,54);
            this.MenuItemHoverForeColor = Color.FromArgb(46,52,54);
            this.MenuItemHoverBackColor = Color.FromArgb(246,245,244);
            this.MenuItemSelectedForeColor = Color.FromArgb(46,52,54);
            this.MenuItemSelectedBackColor = Color.FromArgb(246,245,244);
            this.MenuGradiantStartColor = Color.FromArgb(246,245,244);
            this.MenuGradiantEndColor = Color.FromArgb(246,245,244);
            this.MenuGradiantMiddleColor = Color.FromArgb(246,245,244);
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}