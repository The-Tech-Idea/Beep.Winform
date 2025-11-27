using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeoMorphismTheme
    {
        private void ApplySwitch()
        {
            this.SwitchBackColor = PanelBackColor;
            this.SwitchBorderColor = BorderColor;
            this.SwitchForeColor = ForeColor;
            this.SwitchSelectedBackColor = PanelGradiantMiddleColor;
            this.SwitchSelectedBorderColor = ActiveBorderColor;
            this.SwitchSelectedForeColor = ForeColor;
            this.SwitchHoverBackColor = PanelGradiantMiddleColor;
            this.SwitchHoverBorderColor = ActiveBorderColor;
            this.SwitchHoverForeColor = ForeColor;
        }
    }
}