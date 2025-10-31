using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class SolarizedTheme
    {
        private void ApplyCore()
        {
            // Solarized theme - scientifically crafted color palette
            this.BorderRadius = 6;  // Matching FormStyle.Solarized
            this.BorderSize = 1;
            this.ShadowOpacity = 0.16f;  // Moderate shadows
            this.IsDarkTheme = true;  // Dark theme
        }
    }
}