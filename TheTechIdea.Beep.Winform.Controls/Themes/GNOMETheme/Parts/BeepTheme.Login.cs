using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GNOMETheme
    {
        private void ApplyLogin()
        {
            this.LoginPopoverBackgroundColor = Color.FromArgb(246,245,244);
            this.LoginTitleColor = Color.FromArgb(246,245,244);
            this.LoginSubtitleColor = Color.FromArgb(246,245,244);
            this.LoginDescriptionColor = Color.FromArgb(246,245,244);
            this.LoginLinkColor = Color.FromArgb(246,245,244);
            this.LoginButtonBackgroundColor = Color.FromArgb(246,245,244);
            this.LoginButtonTextColor = Color.FromArgb(46,52,54);
            this.LoginDropdownBackgroundColor = Color.FromArgb(246,245,244);
            this.LoginDropdownTextColor = Color.FromArgb(46,52,54);
            this.LoginLogoBackgroundColor = Color.FromArgb(246,245,244);
        }
    }
}