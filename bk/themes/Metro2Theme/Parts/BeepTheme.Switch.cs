using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class Metro2Theme
    {
        private void ApplySwitch()
        {
            this.SwitchBackColor = Color.FromArgb(243,242,241);
            this.SwitchBorderColor = Color.FromArgb(220,220,220);
            this.SwitchForeColor = Color.FromArgb(32,31,30);
            this.SwitchSelectedBackColor = Color.FromArgb(243,242,241);
            this.SwitchSelectedBorderColor = Color.FromArgb(220,220,220);
            this.SwitchSelectedForeColor = Color.FromArgb(32,31,30);
            this.SwitchHoverBackColor = Color.FromArgb(243,242,241);
            this.SwitchHoverBorderColor = Color.FromArgb(220,220,220);
            this.SwitchHoverForeColor = Color.FromArgb(32,31,30);
        }
    }
}