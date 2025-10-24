using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordTheme
    {
        private void ApplySideMenu()
        {
            this.SideMenuBackColor = Color.FromArgb(46,52,64);
            this.SideMenuHoverBackColor = Color.FromArgb(46,52,64);
            this.SideMenuSelectedBackColor = Color.FromArgb(46,52,64);
            this.SideMenuForeColor = Color.FromArgb(216,222,233);
            this.SideMenuSelectedForeColor = Color.FromArgb(216,222,233);
            this.SideMenuHoverForeColor = Color.FromArgb(216,222,233);
            this.SideMenuBorderColor = Color.FromArgb(76,86,106);
            this.SideMenuTitleTextColor = Color.FromArgb(216,222,233);
            this.SideMenuTitleBackColor = Color.FromArgb(46,52,64);
            this.SideMenuSubTitleTextColor = Color.FromArgb(216,222,233);
            this.SideMenuSubTitleBackColor = Color.FromArgb(46,52,64);
            this.SideMenuGradiantStartColor = Color.FromArgb(46,52,64);
            this.SideMenuGradiantEndColor = Color.FromArgb(46,52,64);
            this.SideMenuGradiantMiddleColor = Color.FromArgb(46,52,64);
            this.SideMenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}