using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeoMorphismTheme
    {
        private void ApplySwitch()
        {
            this.SwitchBackColor = Color.FromArgb(236,240,243);
            this.SwitchBorderColor = Color.FromArgb(221,228,235);
            this.SwitchForeColor = Color.FromArgb(58,66,86);
            this.SwitchSelectedBackColor = Color.FromArgb(236,240,243);
            this.SwitchSelectedBorderColor = Color.FromArgb(221,228,235);
            this.SwitchSelectedForeColor = Color.FromArgb(58,66,86);
            this.SwitchHoverBackColor = Color.FromArgb(236,240,243);
            this.SwitchHoverBorderColor = Color.FromArgb(221,228,235);
            this.SwitchHoverForeColor = Color.FromArgb(58,66,86);
        }
    }
}