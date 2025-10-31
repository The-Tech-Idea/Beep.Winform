using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ModernTheme
    {
        private void ApplySwitch()
        {
            this.SwitchBackColor = Color.FromArgb(255,255,255);
            this.SwitchBorderColor = Color.FromArgb(203,213,225);
            this.SwitchForeColor = Color.FromArgb(17,24,39);
            this.SwitchSelectedBackColor = Color.FromArgb(255,255,255);
            this.SwitchSelectedBorderColor = Color.FromArgb(203,213,225);
            this.SwitchSelectedForeColor = Color.FromArgb(17,24,39);
            this.SwitchHoverBackColor = Color.FromArgb(255,255,255);
            this.SwitchHoverBorderColor = Color.FromArgb(203,213,225);
            this.SwitchHoverForeColor = Color.FromArgb(17,24,39);
        }
    }
}