using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordTheme
    {
        private void ApplyCore()
        {
            // Nord theme - Arctic-inspired dark theme
            this.BorderRadius = 4;  // Matching FormStyle.Nord
            this.BorderSize = 1;
            this.ShadowOpacity = 0.16f;  // Moderate shadows
            this.IsDarkTheme = true;  // Dark theme
        }
    }
}