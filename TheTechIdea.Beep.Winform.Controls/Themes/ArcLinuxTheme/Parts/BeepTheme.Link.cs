using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ArcLinuxTheme
    {
        private void ApplyLink()
        {
            this.LinkColor = SurfaceColor;
            this.LinkHoverColor = SurfaceColor;
            this.LinkIsUnderline = false;
        }
    }
}
