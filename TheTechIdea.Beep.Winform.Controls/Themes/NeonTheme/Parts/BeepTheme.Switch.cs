using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeonTheme
    {
        private void ApplySwitch()
        {
            this.SwitchBackColor = PanelGradiantMiddleColor;
            this.SwitchBorderColor = InactiveBorderColor;
            this.SwitchForeColor = ForeColor;
            this.SwitchSelectedBackColor = PanelGradiantMiddleColor;
            this.SwitchSelectedBorderColor = InactiveBorderColor;
            this.SwitchSelectedForeColor = ForeColor;
            this.SwitchHoverBackColor = PanelGradiantMiddleColor;
            this.SwitchHoverBorderColor = InactiveBorderColor;
            this.SwitchHoverForeColor = ForeColor;
        }
    }
}