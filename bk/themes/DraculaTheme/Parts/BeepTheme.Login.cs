using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class DraculaTheme
    {
        private void ApplyLogin()
        {
            this.LoginPopoverBackgroundColor = Color.FromArgb(40,42,54);
            this.LoginTitleColor = Color.FromArgb(40,42,54);
            this.LoginSubtitleColor = Color.FromArgb(40,42,54);
            this.LoginDescriptionColor = Color.FromArgb(40,42,54);
            this.LoginLinkColor = Color.FromArgb(40,42,54);
            this.LoginButtonBackgroundColor = Color.FromArgb(40,42,54);
            this.LoginButtonTextColor = Color.FromArgb(248,248,242);
            this.LoginDropdownBackgroundColor = Color.FromArgb(40,42,54);
            this.LoginDropdownTextColor = Color.FromArgb(248,248,242);
            this.LoginLogoBackgroundColor = Color.FromArgb(40,42,54);
        }
    }
}