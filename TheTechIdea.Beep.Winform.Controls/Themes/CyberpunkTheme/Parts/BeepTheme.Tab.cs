using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CyberpunkTheme
    {
        private void ApplyTab()
        {
            this.TabBackColor = Color.FromArgb(10,8,20);
            this.TabForeColor = Color.FromArgb(228,244,255);
            this.TabBorderColor = Color.FromArgb(90,20,110);
            this.TabHoverBackColor = Color.FromArgb(10,8,20);
            this.TabHoverForeColor = Color.FromArgb(228,244,255);
            this.TabSelectedBackColor = Color.FromArgb(10,8,20);
            this.TabSelectedForeColor = Color.FromArgb(228,244,255);
            this.TabSelectedBorderColor = Color.FromArgb(90,20,110);
            this.TabHoverBorderColor = Color.FromArgb(90,20,110);
        }
    }
}