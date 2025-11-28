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
            this.LoginPopoverBackgroundColor = PanelBackColor;
            this.LoginTitleColor = ForeColor;
            this.LoginSubtitleColor = BackgroundColor;
            this.LoginDescriptionColor = BackgroundColor;
            this.LoginLinkColor = BackgroundColor;
            this.LoginButtonBackgroundColor = PanelBackColor;
            this.LoginButtonTextColor = ForeColor;
            this.LoginDropdownBackgroundColor = PanelBackColor;
            this.LoginDropdownTextColor = ForeColor;
            this.LoginLogoBackgroundColor = PanelBackColor;
        }
    }
}