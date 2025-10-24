using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GruvBoxTheme
    {
        private void ApplySwitch()
        {
            this.SwitchBackColor = Color.FromArgb(40,40,40);
            this.SwitchBorderColor = Color.FromArgb(168,153,132);
            this.SwitchForeColor = Color.FromArgb(235,219,178);
            this.SwitchSelectedBackColor = Color.FromArgb(40,40,40);
            this.SwitchSelectedBorderColor = Color.FromArgb(168,153,132);
            this.SwitchSelectedForeColor = Color.FromArgb(235,219,178);
            this.SwitchHoverBackColor = Color.FromArgb(40,40,40);
            this.SwitchHoverBorderColor = Color.FromArgb(168,153,132);
            this.SwitchHoverForeColor = Color.FromArgb(235,219,178);
        }
    }
}