using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class HolographicTheme
    {
        private void ApplyLogin()
        {
            this.LoginPopoverBackgroundColor = Color.FromArgb(15,16,32);
            this.LoginTitleColor = Color.FromArgb(15,16,32);
            this.LoginSubtitleColor = Color.FromArgb(15,16,32);
            this.LoginDescriptionColor = Color.FromArgb(15,16,32);
            this.LoginLinkColor = Color.FromArgb(15,16,32);
            this.LoginButtonBackgroundColor = Color.FromArgb(15,16,32);
            this.LoginButtonTextColor = Color.FromArgb(245,247,255);
            this.LoginDropdownBackgroundColor = Color.FromArgb(15,16,32);
            this.LoginDropdownTextColor = Color.FromArgb(245,247,255);
            this.LoginLogoBackgroundColor = Color.FromArgb(15,16,32);
        }
    }
}