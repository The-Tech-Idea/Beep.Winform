using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class UbuntuTheme
    {
        private void ApplySwitch()
        {
            this.SwitchBackColor = Color.FromArgb(242,242,245);
            this.SwitchBorderColor = Color.FromArgb(218,218,222);
            this.SwitchForeColor = Color.FromArgb(44,44,44);
            this.SwitchSelectedBackColor = Color.FromArgb(242,242,245);
            this.SwitchSelectedBorderColor = Color.FromArgb(218,218,222);
            this.SwitchSelectedForeColor = Color.FromArgb(44,44,44);
            this.SwitchHoverBackColor = Color.FromArgb(242,242,245);
            this.SwitchHoverBorderColor = Color.FromArgb(218,218,222);
            this.SwitchHoverForeColor = Color.FromArgb(44,44,44);
        }
    }
}