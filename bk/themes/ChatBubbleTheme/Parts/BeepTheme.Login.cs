using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ChatBubbleTheme
    {
        private void ApplyLogin()
        {
            this.LoginPopoverBackgroundColor = Color.FromArgb(245,248,255);
            this.LoginTitleColor = Color.FromArgb(245,248,255);
            this.LoginSubtitleColor = Color.FromArgb(245,248,255);
            this.LoginDescriptionColor = Color.FromArgb(245,248,255);
            this.LoginLinkColor = Color.FromArgb(245,248,255);
            this.LoginButtonBackgroundColor = Color.FromArgb(245,248,255);
            this.LoginButtonTextColor = Color.FromArgb(24,28,35);
            this.LoginDropdownBackgroundColor = Color.FromArgb(245,248,255);
            this.LoginDropdownTextColor = Color.FromArgb(24,28,35);
            this.LoginLogoBackgroundColor = Color.FromArgb(245,248,255);
        }
    }
}