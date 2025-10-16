using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeoMorphismTheme
    {
        private void ApplyLogin()
        {
            this.LoginPopoverBackgroundColor = Color.FromArgb(236,240,243);
            this.LoginTitleColor = Color.FromArgb(236,240,243);
            this.LoginSubtitleColor = Color.FromArgb(236,240,243);
            this.LoginDescriptionColor = Color.FromArgb(236,240,243);
            this.LoginLinkColor = Color.FromArgb(236,240,243);
            this.LoginButtonBackgroundColor = Color.FromArgb(236,240,243);
            this.LoginButtonTextColor = Color.FromArgb(58,66,86);
            this.LoginDropdownBackgroundColor = Color.FromArgb(236,240,243);
            this.LoginDropdownTextColor = Color.FromArgb(58,66,86);
            this.LoginLogoBackgroundColor = Color.FromArgb(236,240,243);
        }
    }
}