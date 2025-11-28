using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CartoonTheme
    {
        private void ApplySwitch()
        {
            this.SwitchBackColor = SurfaceColor;
            this.SwitchBorderColor = InactiveBorderColor;
            this.SwitchForeColor = ForeColor;
            this.SwitchSelectedBackColor = PrimaryColor;
            this.SwitchSelectedBorderColor = PrimaryColor;
            this.SwitchSelectedForeColor = OnPrimaryColor;
            this.SwitchHoverBackColor = PanelGradiantStartColor;
            this.SwitchHoverBorderColor = ActiveBorderColor;
            this.SwitchHoverForeColor = ForeColor;
        }
    }
}