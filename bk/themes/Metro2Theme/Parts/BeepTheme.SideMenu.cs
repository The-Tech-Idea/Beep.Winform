using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class Metro2Theme
    {
        private void ApplySideMenu()
        {
            this.SideMenuBackColor = Color.FromArgb(243,242,241);
            this.SideMenuHoverBackColor = Color.FromArgb(243,242,241);
            this.SideMenuSelectedBackColor = Color.FromArgb(243,242,241);
            this.SideMenuForeColor = Color.FromArgb(32,31,30);
            this.SideMenuSelectedForeColor = Color.FromArgb(32,31,30);
            this.SideMenuHoverForeColor = Color.FromArgb(32,31,30);
            this.SideMenuBorderColor = Color.FromArgb(220,220,220);
            this.SideMenuTitleTextColor = Color.FromArgb(32,31,30);
            this.SideMenuTitleBackColor = Color.FromArgb(243,242,241);
            this.SideMenuSubTitleTextColor = Color.FromArgb(32,31,30);
            this.SideMenuSubTitleBackColor = Color.FromArgb(243,242,241);
            this.SideMenuGradiantStartColor = Color.FromArgb(243,242,241);
            this.SideMenuGradiantEndColor = Color.FromArgb(243,242,241);
            this.SideMenuGradiantMiddleColor = Color.FromArgb(243,242,241);
            this.SideMenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}