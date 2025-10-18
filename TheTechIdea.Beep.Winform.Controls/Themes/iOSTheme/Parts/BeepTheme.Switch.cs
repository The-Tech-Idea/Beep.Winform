using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class iOSTheme
    {
        private void ApplySwitch()
        {
            this.SwitchBackColor = Color.FromArgb(242,242,247);
            this.SwitchBorderColor = Color.FromArgb(198,198,207);
            this.SwitchForeColor = Color.FromArgb(28,28,30);
            this.SwitchSelectedBackColor = Color.FromArgb(242,242,247);
            this.SwitchSelectedBorderColor = Color.FromArgb(198,198,207);
            this.SwitchSelectedForeColor = Color.FromArgb(28,28,30);
            this.SwitchHoverBackColor = Color.FromArgb(242,242,247);
            this.SwitchHoverBorderColor = Color.FromArgb(198,198,207);
            this.SwitchHoverForeColor = Color.FromArgb(28,28,30);
        }
    }
}