using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordTheme
    {
        private void ApplyLink()
        {
            this.LinkColor = Color.FromArgb(46,52,64);
            this.LinkHoverColor = Color.FromArgb(46,52,64);
            this.LinkIsUnderline = false;
        }
    }
}