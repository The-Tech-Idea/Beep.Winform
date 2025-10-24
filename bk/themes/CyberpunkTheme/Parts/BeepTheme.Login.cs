using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CyberpunkTheme
    {
        private void ApplyLogin()
        {
            this.LoginPopoverBackgroundColor = Color.FromArgb(10,8,20);
            this.LoginTitleColor = Color.FromArgb(10,8,20);
            this.LoginSubtitleColor = Color.FromArgb(10,8,20);
            this.LoginDescriptionColor = Color.FromArgb(10,8,20);
            this.LoginLinkColor = Color.FromArgb(10,8,20);
            this.LoginButtonBackgroundColor = Color.FromArgb(10,8,20);
            this.LoginButtonTextColor = Color.FromArgb(228,244,255);
            this.LoginDropdownBackgroundColor = Color.FromArgb(10,8,20);
            this.LoginDropdownTextColor = Color.FromArgb(228,244,255);
            this.LoginLogoBackgroundColor = Color.FromArgb(10,8,20);
        }
    }
}