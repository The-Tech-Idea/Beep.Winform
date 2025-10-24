using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MacOSTheme
    {
        private void ApplySwitch()
        {
            this.SwitchBackColor = Color.FromArgb(250,250,252);
            this.SwitchBorderColor = Color.FromArgb(229,229,234);
            this.SwitchForeColor = Color.FromArgb(28,28,30);
            this.SwitchSelectedBackColor = Color.FromArgb(250,250,252);
            this.SwitchSelectedBorderColor = Color.FromArgb(229,229,234);
            this.SwitchSelectedForeColor = Color.FromArgb(28,28,30);
            this.SwitchHoverBackColor = Color.FromArgb(250,250,252);
            this.SwitchHoverBorderColor = Color.FromArgb(229,229,234);
            this.SwitchHoverForeColor = Color.FromArgb(28,28,30);
        }
    }
}