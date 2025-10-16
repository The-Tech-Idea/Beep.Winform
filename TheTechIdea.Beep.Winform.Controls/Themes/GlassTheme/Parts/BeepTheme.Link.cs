using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GlassTheme
    {
        private void ApplyLink()
        {
            this.LinkColor = Color.FromArgb(236,244,255);
            this.LinkHoverColor = Color.FromArgb(236,244,255);
            this.LinkIsUnderline = false;
        }
    }
}