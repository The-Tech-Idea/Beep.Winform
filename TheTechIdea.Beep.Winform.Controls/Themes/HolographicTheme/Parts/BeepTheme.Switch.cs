using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class HolographicTheme
    {
        private void ApplySwitch()
        {
            this.SwitchBackColor = Color.FromArgb(15,16,32);
            this.SwitchBorderColor = Color.FromArgb(74,79,123);
            this.SwitchForeColor = Color.FromArgb(245,247,255);
            this.SwitchSelectedBackColor = Color.FromArgb(15,16,32);
            this.SwitchSelectedBorderColor = Color.FromArgb(74,79,123);
            this.SwitchSelectedForeColor = Color.FromArgb(245,247,255);
            this.SwitchHoverBackColor = Color.FromArgb(15,16,32);
            this.SwitchHoverBorderColor = Color.FromArgb(74,79,123);
            this.SwitchHoverForeColor = Color.FromArgb(245,247,255);
        }
    }
}