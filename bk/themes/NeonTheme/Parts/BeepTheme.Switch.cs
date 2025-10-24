using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeonTheme
    {
        private void ApplySwitch()
        {
            this.SwitchBackColor = Color.FromArgb(10,12,20);
            this.SwitchBorderColor = Color.FromArgb(60,70,100);
            this.SwitchForeColor = Color.FromArgb(235,245,255);
            this.SwitchSelectedBackColor = Color.FromArgb(10,12,20);
            this.SwitchSelectedBorderColor = Color.FromArgb(60,70,100);
            this.SwitchSelectedForeColor = Color.FromArgb(235,245,255);
            this.SwitchHoverBackColor = Color.FromArgb(10,12,20);
            this.SwitchHoverBorderColor = Color.FromArgb(60,70,100);
            this.SwitchHoverForeColor = Color.FromArgb(235,245,255);
        }
    }
}