using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class KDETheme
    {
        private void ApplyLink()
        {
            this.LinkColor = BackgroundColor;
            this.LinkHoverColor = BackgroundColor;
            this.LinkIsUnderline = false;
        }
    }
}