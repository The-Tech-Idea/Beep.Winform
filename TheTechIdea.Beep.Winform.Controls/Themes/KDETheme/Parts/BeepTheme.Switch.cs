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
            this.SwitchSelectedBackColor = PanelBackColor;
            this.SwitchSelectedBorderColor = BorderColor;
            this.SwitchSelectedForeColor = ForeColor;
            this.SwitchHoverBackColor = PanelBackColor;
            this.SwitchHoverBorderColor = BorderColor;
            this.SwitchHoverForeColor = ForeColor;
        }
    }
}