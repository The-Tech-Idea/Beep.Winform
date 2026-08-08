using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class KDETheme
    {
        private void ApplySwitch()
        {
            this.SwitchBackColor = PanelBackColor;
            this.SwitchBorderColor = BorderColor;
            this.SwitchForeColor = ForeColor;
            this.SwitchSelectedBackColor = PrimaryColor;
            this.SwitchSelectedBorderColor = PrimaryColor;
            this.SwitchSelectedForeColor = OnPrimaryColor;
            this.SwitchHoverBackColor = PanelBackColor;
            this.SwitchHoverBorderColor = BorderColor;
            this.SwitchHoverForeColor = ForeColor;
        }
    }
}