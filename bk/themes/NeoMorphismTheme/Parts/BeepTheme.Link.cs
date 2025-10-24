using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeoMorphismTheme
    {
        private void ApplyLink()
        {
            this.LinkColor = Color.FromArgb(236,240,243);
            this.LinkHoverColor = Color.FromArgb(236,240,243);
            this.LinkIsUnderline = false;
        }
    }
}