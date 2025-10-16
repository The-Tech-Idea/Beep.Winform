using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GNOMETheme
    {
        private void ApplySwitch()
        {
            this.SwitchBackColor = Color.FromArgb(246,245,244);
            this.SwitchBorderColor = Color.FromArgb(205,207,212);
            this.SwitchForeColor = Color.FromArgb(46,52,54);
            this.SwitchSelectedBackColor = Color.FromArgb(246,245,244);
            this.SwitchSelectedBorderColor = Color.FromArgb(205,207,212);
            this.SwitchSelectedForeColor = Color.FromArgb(46,52,54);
            this.SwitchHoverBackColor = Color.FromArgb(246,245,244);
            this.SwitchHoverBorderColor = Color.FromArgb(205,207,212);
            this.SwitchHoverForeColor = Color.FromArgb(46,52,54);
        }
    }
}