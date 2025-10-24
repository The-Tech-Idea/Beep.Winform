using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class KDETheme
    {
        private void ApplyLogin()
        {
            this.LoginPopoverBackgroundColor = Color.FromArgb(248,249,250);
            this.LoginTitleColor = Color.FromArgb(248,249,250);
            this.LoginSubtitleColor = Color.FromArgb(248,249,250);
            this.LoginDescriptionColor = Color.FromArgb(248,249,250);
            this.LoginLinkColor = Color.FromArgb(248,249,250);
            this.LoginButtonBackgroundColor = Color.FromArgb(248,249,250);
            this.LoginButtonTextColor = Color.FromArgb(33,37,41);
            this.LoginDropdownBackgroundColor = Color.FromArgb(248,249,250);
            this.LoginDropdownTextColor = Color.FromArgb(33,37,41);
            this.LoginLogoBackgroundColor = Color.FromArgb(248,249,250);
        }
    }
}