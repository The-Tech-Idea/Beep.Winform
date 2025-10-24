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
            this.SwitchBackColor = Color.FromArgb(10,8,20);
            this.SwitchBorderColor = Color.FromArgb(90,20,110);
            this.SwitchForeColor = Color.FromArgb(228,244,255);
            this.SwitchSelectedBackColor = Color.FromArgb(10,8,20);
            this.SwitchSelectedBorderColor = Color.FromArgb(90,20,110);
            this.SwitchSelectedForeColor = Color.FromArgb(228,244,255);
            this.SwitchHoverBackColor = Color.FromArgb(10,8,20);
            this.SwitchHoverBorderColor = Color.FromArgb(90,20,110);
            this.SwitchHoverForeColor = Color.FromArgb(228,244,255);
        }
    }
}