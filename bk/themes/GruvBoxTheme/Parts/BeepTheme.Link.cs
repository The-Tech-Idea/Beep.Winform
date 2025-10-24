using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GruvBoxTheme
    {
        private void ApplyLink()
        {
            this.LinkColor = Color.FromArgb(40,40,40);
            this.LinkHoverColor = Color.FromArgb(40,40,40);
            this.LinkIsUnderline = false;
        }
    }
}