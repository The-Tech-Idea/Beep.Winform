using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MetroTheme
    {
        private void ApplyLink()
        {
            this.LinkColor = Color.FromArgb(243,242,241);
            this.LinkHoverColor = Color.FromArgb(243,242,241);
            this.LinkIsUnderline = false;
        }
    }
}