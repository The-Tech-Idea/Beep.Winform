using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordTheme
    {
        private void ApplySwitch()
        {
            this.SwitchBackColor = Color.FromArgb(46,52,64);
            this.SwitchBorderColor = Color.FromArgb(76,86,106);
            this.SwitchForeColor = Color.FromArgb(216,222,233);
            this.SwitchSelectedBackColor = Color.FromArgb(46,52,64);
            this.SwitchSelectedBorderColor = Color.FromArgb(76,86,106);
            this.SwitchSelectedForeColor = Color.FromArgb(216,222,233);
            this.SwitchHoverBackColor = Color.FromArgb(46,52,64);
            this.SwitchHoverBorderColor = Color.FromArgb(76,86,106);
            this.SwitchHoverForeColor = Color.FromArgb(216,222,233);
        }
    }
}