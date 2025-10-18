using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ArcLinuxTheme
    {
        private void ApplySwitch()
        {
            this.SwitchBackColor = Color.FromArgb(245,246,247);
            this.SwitchBorderColor = Color.FromArgb(220,223,230);
            this.SwitchForeColor = Color.FromArgb(43,45,48);
            this.SwitchSelectedBackColor = Color.FromArgb(245,246,247);
            this.SwitchSelectedBorderColor = Color.FromArgb(220,223,230);
            this.SwitchSelectedForeColor = Color.FromArgb(43,45,48);
            this.SwitchHoverBackColor = Color.FromArgb(245,246,247);
            this.SwitchHoverBorderColor = Color.FromArgb(220,223,230);
            this.SwitchHoverForeColor = Color.FromArgb(43,45,48);
        }
    }
}