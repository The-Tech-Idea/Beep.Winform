using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GNOMETheme
    {
        private void ApplySideMenu()
        {
            this.SideMenuBackColor = Color.FromArgb(246,245,244);
            this.SideMenuHoverBackColor = Color.FromArgb(246,245,244);
            this.SideMenuSelectedBackColor = Color.FromArgb(246,245,244);
            this.SideMenuForeColor = Color.FromArgb(46,52,54);
            this.SideMenuSelectedForeColor = Color.FromArgb(46,52,54);
            this.SideMenuHoverForeColor = Color.FromArgb(46,52,54);
            this.SideMenuBorderColor = Color.FromArgb(205,207,212);
            this.SideMenuTitleTextColor = Color.FromArgb(46,52,54);
            this.SideMenuTitleBackColor = Color.FromArgb(246,245,244);
            this.SideMenuSubTitleTextColor = Color.FromArgb(46,52,54);
            this.SideMenuSubTitleBackColor = Color.FromArgb(246,245,244);
            this.SideMenuGradiantStartColor = Color.FromArgb(246,245,244);
            this.SideMenuGradiantEndColor = Color.FromArgb(246,245,244);
            this.SideMenuGradiantMiddleColor = Color.FromArgb(246,245,244);
            this.SideMenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}