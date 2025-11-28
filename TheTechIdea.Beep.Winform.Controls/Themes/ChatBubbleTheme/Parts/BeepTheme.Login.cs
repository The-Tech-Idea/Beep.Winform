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
            this.LoginPopoverBackgroundColor = PanelGradiantStartColor;
            this.LoginTitleColor = ForeColor;
            this.LoginSubtitleColor = ForeColor;
            this.LoginDescriptionColor = ForeColor;
            this.LoginLinkColor = AccentColor;
            this.LoginButtonBackgroundColor = BackgroundColor;
            this.LoginButtonTextColor = ForeColor;
            this.LoginDropdownBackgroundColor = BackgroundColor;
            this.LoginDropdownTextColor = ForeColor;
            this.LoginLogoBackgroundColor = BackgroundColor;
        }
    }
}