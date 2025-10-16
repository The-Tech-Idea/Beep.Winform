using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class FluentTheme
    {
        private void ApplySwitch()
        {
            this.SwitchBackColor = Color.FromArgb(245,246,248);
            this.SwitchBorderColor = Color.FromArgb(218,223,230);
            this.SwitchForeColor = Color.FromArgb(32,32,32);
            this.SwitchSelectedBackColor = Color.FromArgb(245,246,248);
            this.SwitchSelectedBorderColor = Color.FromArgb(218,223,230);
            this.SwitchSelectedForeColor = Color.FromArgb(32,32,32);
            this.SwitchHoverBackColor = Color.FromArgb(245,246,248);
            this.SwitchHoverBorderColor = Color.FromArgb(218,223,230);
            this.SwitchHoverForeColor = Color.FromArgb(32,32,32);
        }
    }
}