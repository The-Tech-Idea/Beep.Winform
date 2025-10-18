using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class DraculaTheme
    {
        private void ApplySideMenu()
        {
            this.SideMenuBackColor = Color.FromArgb(40,42,54);
            this.SideMenuHoverBackColor = Color.FromArgb(40,42,54);
            this.SideMenuSelectedBackColor = Color.FromArgb(40,42,54);
            this.SideMenuForeColor = Color.FromArgb(248,248,242);
            this.SideMenuSelectedForeColor = Color.FromArgb(248,248,242);
            this.SideMenuHoverForeColor = Color.FromArgb(248,248,242);
            this.SideMenuBorderColor = Color.FromArgb(98,114,164);
            this.SideMenuTitleTextColor = Color.FromArgb(248,248,242);
            this.SideMenuTitleBackColor = Color.FromArgb(40,42,54);
            this.SideMenuSubTitleTextColor = Color.FromArgb(248,248,242);
            this.SideMenuSubTitleBackColor = Color.FromArgb(40,42,54);
            this.SideMenuGradiantStartColor = Color.FromArgb(40,42,54);
            this.SideMenuGradiantEndColor = Color.FromArgb(40,42,54);
            this.SideMenuGradiantMiddleColor = Color.FromArgb(40,42,54);
            this.SideMenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
        }
    }
}