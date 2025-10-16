using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CartoonTheme
    {
        private void ApplySwitch()
        {
            this.SwitchBackColor = Color.FromArgb(255,251,235);
            this.SwitchBorderColor = Color.FromArgb(247,208,136);
            this.SwitchForeColor = Color.FromArgb(33,37,41);
            this.SwitchSelectedBackColor = Color.FromArgb(255,251,235);
            this.SwitchSelectedBorderColor = Color.FromArgb(247,208,136);
            this.SwitchSelectedForeColor = Color.FromArgb(33,37,41);
            this.SwitchHoverBackColor = Color.FromArgb(255,251,235);
            this.SwitchHoverBorderColor = Color.FromArgb(247,208,136);
            this.SwitchHoverForeColor = Color.FromArgb(33,37,41);
        }
    }
}