using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class PaperTheme
    {
        private void ApplySideMenu()
        {
            this.SideMenuBackColor = Color.FromArgb(250,250,250);
            this.SideMenuHoverBackColor = Color.FromArgb(250,250,250);
            this.SideMenuSelectedBackColor = Color.FromArgb(250,250,250);
            this.SideMenuForeColor = Color.FromArgb(33,33,33);
            this.SideMenuSelectedForeColor = Color.FromArgb(33,33,33);
            this.SideMenuHoverForeColor = Color.FromArgb(33,33,33);
            this.SideMenuBorderColor = Color.FromArgb(224,224,224);
            this.SideMenuTitleTextColor = Color.FromArgb(33,33,33);
            this.SideMenuTitleBackColor = Color.FromArgb(250,250,250);
            this.SideMenuSubTitleTextColor = Color.FromArgb(33,33,33);
            this.SideMenuSubTitleBackColor = Color.FromArgb(250,250,250);
            this.SideMenuGradiantStartColor = Color.FromArgb(250,250,250);
            this.SideMenuGradiantEndColor = Color.FromArgb(250,250,250);
            this.SideMenuGradiantMiddleColor = Color.FromArgb(250,250,250);
            this.SideMenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}