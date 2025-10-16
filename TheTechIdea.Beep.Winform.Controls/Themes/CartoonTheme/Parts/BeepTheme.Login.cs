using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CartoonTheme
    {
        private void ApplyLogin()
        {
            this.LoginPopoverBackgroundColor = Color.FromArgb(255,251,235);
            this.LoginTitleColor = Color.FromArgb(255,251,235);
            this.LoginSubtitleColor = Color.FromArgb(255,251,235);
            this.LoginDescriptionColor = Color.FromArgb(255,251,235);
            this.LoginLinkColor = Color.FromArgb(255,251,235);
            this.LoginButtonBackgroundColor = Color.FromArgb(255,251,235);
            this.LoginButtonTextColor = Color.FromArgb(33,37,41);
            this.LoginDropdownBackgroundColor = Color.FromArgb(255,251,235);
            this.LoginDropdownTextColor = Color.FromArgb(33,37,41);
            this.LoginLogoBackgroundColor = Color.FromArgb(255,251,235);
        }
    }
}