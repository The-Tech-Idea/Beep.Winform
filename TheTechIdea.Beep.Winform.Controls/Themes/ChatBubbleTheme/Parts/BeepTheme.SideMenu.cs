using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ChatBubbleTheme
    {
        private void ApplySideMenu()
        {
            this.SideMenuBackColor = Color.FromArgb(245,248,255);
            this.SideMenuHoverBackColor = Color.FromArgb(245,248,255);
            this.SideMenuSelectedBackColor = Color.FromArgb(245,248,255);
            this.SideMenuForeColor = Color.FromArgb(24,28,35);
            this.SideMenuSelectedForeColor = Color.FromArgb(24,28,35);
            this.SideMenuHoverForeColor = Color.FromArgb(24,28,35);
            this.SideMenuBorderColor = Color.FromArgb(210,220,235);
            this.SideMenuTitleTextColor = Color.FromArgb(24,28,35);
            this.SideMenuTitleBackColor = Color.FromArgb(245,248,255);
            this.SideMenuSubTitleTextColor = Color.FromArgb(24,28,35);
            this.SideMenuSubTitleBackColor = Color.FromArgb(245,248,255);
            this.SideMenuGradiantStartColor = Color.FromArgb(245,248,255);
            this.SideMenuGradiantEndColor = Color.FromArgb(245,248,255);
            this.SideMenuGradiantMiddleColor = Color.FromArgb(245,248,255);
            this.SideMenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}