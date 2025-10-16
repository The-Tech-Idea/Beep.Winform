using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class BrutalistTheme
    {
        private void ApplySwitch()
        {
            this.SwitchBackColor = Color.FromArgb(250,250,250);
            this.SwitchBorderColor = Color.FromArgb(0,0,0);
            this.SwitchForeColor = Color.FromArgb(20,20,20);
            this.SwitchSelectedBackColor = Color.FromArgb(250,250,250);
            this.SwitchSelectedBorderColor = Color.FromArgb(0,0,0);
            this.SwitchSelectedForeColor = Color.FromArgb(20,20,20);
            this.SwitchHoverBackColor = Color.FromArgb(250,250,250);
            this.SwitchHoverBorderColor = Color.FromArgb(0,0,0);
            this.SwitchHoverForeColor = Color.FromArgb(20,20,20);
        }
    }
}