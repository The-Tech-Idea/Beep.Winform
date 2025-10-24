using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MacOSTheme
    {
        private void ApplyCore()
        {
            this.BorderRadius = 12;
            this.BorderSize = 0;
            this.ShadowOpacity = 0.10f;
            this.IsDarkTheme = false;
        }
    }
}