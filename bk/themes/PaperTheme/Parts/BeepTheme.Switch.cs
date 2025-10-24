using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class PaperTheme
    {
        private void ApplySwitch()
        {
            this.SwitchBackColor = Color.FromArgb(250,250,250);
            this.SwitchBorderColor = Color.FromArgb(224,224,224);
            this.SwitchForeColor = Color.FromArgb(33,33,33);
            this.SwitchSelectedBackColor = Color.FromArgb(250,250,250);
            this.SwitchSelectedBorderColor = Color.FromArgb(224,224,224);
            this.SwitchSelectedForeColor = Color.FromArgb(33,33,33);
            this.SwitchHoverBackColor = Color.FromArgb(250,250,250);
            this.SwitchHoverBorderColor = Color.FromArgb(224,224,224);
            this.SwitchHoverForeColor = Color.FromArgb(33,33,33);
        }
    }
}