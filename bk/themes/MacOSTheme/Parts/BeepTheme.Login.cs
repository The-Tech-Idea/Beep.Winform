using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MacOSTheme
    {
        private void ApplyLogin()
        {
            this.LoginPopoverBackgroundColor = Color.FromArgb(250,250,252);
            this.LoginTitleColor = Color.FromArgb(250,250,252);
            this.LoginSubtitleColor = Color.FromArgb(250,250,252);
            this.LoginDescriptionColor = Color.FromArgb(250,250,252);
            this.LoginLinkColor = Color.FromArgb(250,250,252);
            this.LoginButtonBackgroundColor = Color.FromArgb(250,250,252);
            this.LoginButtonTextColor = Color.FromArgb(28,28,30);
            this.LoginDropdownBackgroundColor = Color.FromArgb(250,250,252);
            this.LoginDropdownTextColor = Color.FromArgb(28,28,30);
            this.LoginLogoBackgroundColor = Color.FromArgb(250,250,252);
        }
    }
}