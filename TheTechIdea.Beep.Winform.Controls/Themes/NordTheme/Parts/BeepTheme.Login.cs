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
            this.LoginPopoverBackgroundColor = PanelBackColor;
            this.LoginTitleColor = PanelBackColor;
            this.LoginSubtitleColor = PanelBackColor;
            this.LoginDescriptionColor = PanelBackColor;
            this.LoginLinkColor = PanelBackColor;
            this.LoginButtonBackgroundColor = PanelBackColor;
            this.LoginButtonTextColor = ForeColor;
            this.LoginDropdownBackgroundColor = PanelBackColor;
            this.LoginDropdownTextColor = ForeColor;
            this.LoginLogoBackgroundColor = PanelBackColor;
        }
    }
}