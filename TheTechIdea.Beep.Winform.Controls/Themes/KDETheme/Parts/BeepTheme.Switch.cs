using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class KDETheme
    {
        private void ApplySwitch()
        {
            this.SwitchBackColor = Color.FromArgb(248,249,250);
            this.SwitchBorderColor = Color.FromArgb(222,226,230);
            this.SwitchForeColor = Color.FromArgb(33,37,41);
            this.SwitchSelectedBackColor = Color.FromArgb(248,249,250);
            this.SwitchSelectedBorderColor = Color.FromArgb(222,226,230);
            this.SwitchSelectedForeColor = Color.FromArgb(33,37,41);
            this.SwitchHoverBackColor = Color.FromArgb(248,249,250);
            this.SwitchHoverBorderColor = Color.FromArgb(222,226,230);
            this.SwitchHoverForeColor = Color.FromArgb(33,37,41);
        }
    }
}