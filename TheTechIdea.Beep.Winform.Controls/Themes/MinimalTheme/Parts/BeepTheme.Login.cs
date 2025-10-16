using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MinimalTheme
    {
        private void ApplyLogin()
        {
            this.LoginPopoverBackgroundColor = Color.FromArgb(255,255,255);
            this.LoginTitleColor = Color.FromArgb(255,255,255);
            this.LoginSubtitleColor = Color.FromArgb(255,255,255);
            this.LoginDescriptionColor = Color.FromArgb(255,255,255);
            this.LoginLinkColor = Color.FromArgb(255,255,255);
            this.LoginButtonBackgroundColor = Color.FromArgb(255,255,255);
            this.LoginButtonTextColor = Color.FromArgb(31,41,55);
            this.LoginDropdownBackgroundColor = Color.FromArgb(255,255,255);
            this.LoginDropdownTextColor = Color.FromArgb(31,41,55);
            this.LoginLogoBackgroundColor = Color.FromArgb(255,255,255);
        }
    }
}