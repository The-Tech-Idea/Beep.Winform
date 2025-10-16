using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MinimalTheme
    {
        private void ApplySwitch()
        {
            this.SwitchBackColor = Color.FromArgb(255,255,255);
            this.SwitchBorderColor = Color.FromArgb(209,213,219);
            this.SwitchForeColor = Color.FromArgb(31,41,55);
            this.SwitchSelectedBackColor = Color.FromArgb(255,255,255);
            this.SwitchSelectedBorderColor = Color.FromArgb(209,213,219);
            this.SwitchSelectedForeColor = Color.FromArgb(31,41,55);
            this.SwitchHoverBackColor = Color.FromArgb(255,255,255);
            this.SwitchHoverBorderColor = Color.FromArgb(209,213,219);
            this.SwitchHoverForeColor = Color.FromArgb(31,41,55);
        }
    }
}