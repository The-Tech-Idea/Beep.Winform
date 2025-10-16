using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CyberpunkTheme
    {
        private void ApplyLink()
        {
            this.LinkColor = Color.FromArgb(10,8,20);
            this.LinkHoverColor = Color.FromArgb(10,8,20);
            this.LinkIsUnderline = false;
        }
    }
}