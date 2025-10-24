using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class KDETheme
    {
        private void ApplySideMenu()
        {
            this.SideMenuBackColor = Color.FromArgb(248,249,250);
            this.SideMenuHoverBackColor = Color.FromArgb(248,249,250);
            this.SideMenuSelectedBackColor = Color.FromArgb(248,249,250);
            this.SideMenuForeColor = Color.FromArgb(33,37,41);
            this.SideMenuSelectedForeColor = Color.FromArgb(33,37,41);
            this.SideMenuHoverForeColor = Color.FromArgb(33,37,41);
            this.SideMenuBorderColor = Color.FromArgb(222,226,230);
            this.SideMenuTitleTextColor = Color.FromArgb(33,37,41);
            this.SideMenuTitleBackColor = Color.FromArgb(248,249,250);
            this.SideMenuSubTitleTextColor = Color.FromArgb(33,37,41);
            this.SideMenuSubTitleBackColor = Color.FromArgb(248,249,250);
            this.SideMenuGradiantStartColor = Color.FromArgb(248,249,250);
            this.SideMenuGradiantEndColor = Color.FromArgb(248,249,250);
            this.SideMenuGradiantMiddleColor = Color.FromArgb(248,249,250);
            this.SideMenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}