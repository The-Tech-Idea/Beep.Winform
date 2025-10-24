using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class FluentTheme
    {
        private void ApplyLogin()
        {
            this.LoginPopoverBackgroundColor = Color.FromArgb(245,246,248);
            this.LoginTitleColor = Color.FromArgb(245,246,248);
            this.LoginSubtitleColor = Color.FromArgb(245,246,248);
            this.LoginDescriptionColor = Color.FromArgb(245,246,248);
            this.LoginLinkColor = Color.FromArgb(245,246,248);
            this.LoginButtonBackgroundColor = Color.FromArgb(245,246,248);
            this.LoginButtonTextColor = Color.FromArgb(32,32,32);
            this.LoginDropdownBackgroundColor = Color.FromArgb(245,246,248);
            this.LoginDropdownTextColor = Color.FromArgb(32,32,32);
            this.LoginLogoBackgroundColor = Color.FromArgb(245,246,248);
        }
    }
}