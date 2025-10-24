using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class OneDarkTheme
    {
        private void ApplySwitch()
        {
            this.SwitchBackColor = Color.FromArgb(40,44,52);
            this.SwitchBorderColor = Color.FromArgb(92,99,112);
            this.SwitchForeColor = Color.FromArgb(171,178,191);
            this.SwitchSelectedBackColor = Color.FromArgb(40,44,52);
            this.SwitchSelectedBorderColor = Color.FromArgb(92,99,112);
            this.SwitchSelectedForeColor = Color.FromArgb(171,178,191);
            this.SwitchHoverBackColor = Color.FromArgb(40,44,52);
            this.SwitchHoverBorderColor = Color.FromArgb(92,99,112);
            this.SwitchHoverForeColor = Color.FromArgb(171,178,191);
        }
    }
}