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
            this.SwitchBackColor = Color.FromArgb(243,242,241);
            this.SwitchBorderColor = Color.FromArgb(225,225,225);
            this.SwitchForeColor = Color.FromArgb(32,31,30);
            this.SwitchSelectedBackColor = Color.FromArgb(243,242,241);
            this.SwitchSelectedBorderColor = Color.FromArgb(225,225,225);
            this.SwitchSelectedForeColor = Color.FromArgb(32,31,30);
            this.SwitchHoverBackColor = Color.FromArgb(243,242,241);
            this.SwitchHoverBorderColor = Color.FromArgb(225,225,225);
            this.SwitchHoverForeColor = Color.FromArgb(32,31,30);
        }
    }
}