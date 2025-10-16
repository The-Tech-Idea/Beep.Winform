using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordicTheme
    {
        private void ApplySwitch()
        {
            this.SwitchBackColor = Color.FromArgb(250,250,251);
            this.SwitchBorderColor = Color.FromArgb(229,231,235);
            this.SwitchForeColor = Color.FromArgb(31,41,55);
            this.SwitchSelectedBackColor = Color.FromArgb(250,250,251);
            this.SwitchSelectedBorderColor = Color.FromArgb(229,231,235);
            this.SwitchSelectedForeColor = Color.FromArgb(31,41,55);
            this.SwitchHoverBackColor = Color.FromArgb(250,250,251);
            this.SwitchHoverBorderColor = Color.FromArgb(229,231,235);
            this.SwitchHoverForeColor = Color.FromArgb(31,41,55);
        }
    }
}