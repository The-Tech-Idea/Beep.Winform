using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GruvBoxTheme
    {
        private void ApplySideMenu()
        {
            this.SideMenuBackColor = Color.FromArgb(40,40,40);
            this.SideMenuHoverBackColor = Color.FromArgb(40,40,40);
            this.SideMenuSelectedBackColor = Color.FromArgb(40,40,40);
            this.SideMenuForeColor = Color.FromArgb(235,219,178);
            this.SideMenuSelectedForeColor = Color.FromArgb(235,219,178);
            this.SideMenuHoverForeColor = Color.FromArgb(235,219,178);
            this.SideMenuBorderColor = Color.FromArgb(168,153,132);
            this.SideMenuTitleTextColor = Color.FromArgb(235,219,178);
            this.SideMenuTitleBackColor = Color.FromArgb(40,40,40);
            this.SideMenuSubTitleTextColor = Color.FromArgb(235,219,178);
            this.SideMenuSubTitleBackColor = Color.FromArgb(40,40,40);
            this.SideMenuGradiantStartColor = Color.FromArgb(40,40,40);
            this.SideMenuGradiantEndColor = Color.FromArgb(40,40,40);
            this.SideMenuGradiantMiddleColor = Color.FromArgb(40,40,40);
            this.SideMenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}