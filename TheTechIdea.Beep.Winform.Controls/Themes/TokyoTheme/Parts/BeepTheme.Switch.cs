using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class TokyoTheme
    {
        private void ApplySwitch()
        {
            this.SwitchBackColor = Color.FromArgb(26,27,38);
            this.SwitchBorderColor = Color.FromArgb(86,95,137);
            this.SwitchForeColor = Color.FromArgb(192,202,245);
            this.SwitchSelectedBackColor = Color.FromArgb(26,27,38);
            this.SwitchSelectedBorderColor = Color.FromArgb(86,95,137);
            this.SwitchSelectedForeColor = Color.FromArgb(192,202,245);
            this.SwitchHoverBackColor = Color.FromArgb(26,27,38);
            this.SwitchHoverBorderColor = Color.FromArgb(86,95,137);
            this.SwitchHoverForeColor = Color.FromArgb(192,202,245);
        }
    }
}