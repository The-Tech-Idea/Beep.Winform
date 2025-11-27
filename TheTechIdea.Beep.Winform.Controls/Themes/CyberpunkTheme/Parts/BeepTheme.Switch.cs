using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CyberpunkTheme
    {
        private void ApplySwitch()
        {
            this.SwitchBackColor = BackgroundColor;
            this.SwitchBorderColor = SecondaryColor;
            this.SwitchForeColor = ForeColor;
            this.SwitchSelectedBackColor = PrimaryColor;
            this.SwitchSelectedBorderColor = PrimaryColor;
            this.SwitchSelectedForeColor = OnPrimaryColor;
            this.SwitchHoverBackColor = PanelGradiantMiddleColor;
            this.SwitchHoverBorderColor = SecondaryColor;
            this.SwitchHoverForeColor = ForeColor;
        }
    }
}