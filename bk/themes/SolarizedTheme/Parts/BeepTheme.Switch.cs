using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class SolarizedTheme
    {
        private void ApplySwitch()
        {
            this.SwitchBackColor = Color.FromArgb(0,43,54);
            this.SwitchBorderColor = Color.FromArgb(88,110,117);
            this.SwitchForeColor = Color.FromArgb(147,161,161);
            this.SwitchSelectedBackColor = Color.FromArgb(0,43,54);
            this.SwitchSelectedBorderColor = Color.FromArgb(88,110,117);
            this.SwitchSelectedForeColor = Color.FromArgb(147,161,161);
            this.SwitchHoverBackColor = Color.FromArgb(0,43,54);
            this.SwitchHoverBorderColor = Color.FromArgb(88,110,117);
            this.SwitchHoverForeColor = Color.FromArgb(147,161,161);
        }
    }
}