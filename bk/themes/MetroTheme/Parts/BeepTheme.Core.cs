using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MetroTheme
    {
        private void ApplyCore()
        {
            this.BorderRadius = 2;
            this.BorderSize = 1;
            this.ShadowOpacity = 0.06f;
            this.IsDarkTheme = false;
        }
    }
}