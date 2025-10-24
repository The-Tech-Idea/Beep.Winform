using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class Metro2Theme
    {
        private void ApplyCore()
        {
            this.BorderRadius = 3;
            this.BorderSize = 1;
            this.ShadowOpacity = 0.08f;
            this.IsDarkTheme = false;
        }
    }
}