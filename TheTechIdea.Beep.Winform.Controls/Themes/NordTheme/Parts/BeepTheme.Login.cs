using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordTheme
    {
        private void ApplyLogin()
        {
            this.LoginPopoverBackgroundColor = Color.FromArgb(46,52,64);
            this.LoginTitleColor = Color.FromArgb(46,52,64);
            this.LoginSubtitleColor = Color.FromArgb(46,52,64);
            this.LoginDescriptionColor = Color.FromArgb(46,52,64);
            this.LoginLinkColor = Color.FromArgb(46,52,64);
            this.LoginButtonBackgroundColor = Color.FromArgb(46,52,64);
            this.LoginButtonTextColor = Color.FromArgb(216,222,233);
            this.LoginDropdownBackgroundColor = Color.FromArgb(46,52,64);
            this.LoginDropdownTextColor = Color.FromArgb(216,222,233);
            this.LoginLogoBackgroundColor = Color.FromArgb(46,52,64);
        }
    }
}