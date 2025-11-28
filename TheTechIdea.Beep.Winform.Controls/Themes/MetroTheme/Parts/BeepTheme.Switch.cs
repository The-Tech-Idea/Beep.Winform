using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MetroTheme
    {
        private void ApplySwitch()
        {
            this.SwitchBackColor = BackgroundColor;
            this.SwitchBorderColor = BorderColor;
            this.SwitchForeColor = ForeColor;
            this.SwitchSelectedBackColor = BackgroundColor;
            this.SwitchSelectedBorderColor = BorderColor;
            this.SwitchSelectedForeColor = ForeColor;
            this.SwitchHoverBackColor = BackgroundColor;
            this.SwitchHoverBorderColor = BorderColor;
            this.SwitchHoverForeColor = ForeColor;
        }
    }
}