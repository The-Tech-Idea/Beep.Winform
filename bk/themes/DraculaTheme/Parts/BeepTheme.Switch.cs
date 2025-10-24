using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class DraculaTheme
    {
        private void ApplySwitch()
        {
            this.SwitchBackColor = Color.FromArgb(40,42,54);
            this.SwitchBorderColor = Color.FromArgb(98,114,164);
            this.SwitchForeColor = Color.FromArgb(248,248,242);
            this.SwitchSelectedBackColor = Color.FromArgb(40,42,54);
            this.SwitchSelectedBorderColor = Color.FromArgb(98,114,164);
            this.SwitchSelectedForeColor = Color.FromArgb(248,248,242);
            this.SwitchHoverBackColor = Color.FromArgb(40,42,54);
            this.SwitchHoverBorderColor = Color.FromArgb(98,114,164);
            this.SwitchHoverForeColor = Color.FromArgb(248,248,242);
        }
    }
}