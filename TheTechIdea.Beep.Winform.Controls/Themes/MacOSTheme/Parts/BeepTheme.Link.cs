using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MacOSTheme
    {
        private void ApplyLink()
        {
            this.LinkColor = PrimaryColor;
            this.LinkHoverColor = ThemeUtil.Lighten(PrimaryColor, 0.15);
            this.LinkIsUnderline = false;
        }
    }
}