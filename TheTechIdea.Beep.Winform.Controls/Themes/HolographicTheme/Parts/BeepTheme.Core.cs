using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class HolographicTheme
    {
        private void ApplyCore()
        {
            this.BorderRadius = 12;
            this.BorderSize = 1;
            this.ShadowOpacity = 0.20f;
            this.IsDarkTheme = true;
        }
    }
}