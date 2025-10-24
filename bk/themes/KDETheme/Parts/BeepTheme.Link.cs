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
            this.LinkColor = Color.FromArgb(248,249,250);
            this.LinkHoverColor = Color.FromArgb(248,249,250);
            this.LinkIsUnderline = false;
        }
    }
}