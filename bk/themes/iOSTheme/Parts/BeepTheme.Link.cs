using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class iOSTheme
    {
        private void ApplyLink()
        {
            this.LinkColor = Color.FromArgb(242,242,247);
            this.LinkHoverColor = Color.FromArgb(242,242,247);
            this.LinkIsUnderline = false;
        }
    }
}