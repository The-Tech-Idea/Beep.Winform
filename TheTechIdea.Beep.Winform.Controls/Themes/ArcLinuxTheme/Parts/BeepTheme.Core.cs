using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ArcLinuxTheme
    {
        private void ApplyCore()
        {
            // Arc Linux aesthetic - subtle borders, moderate shadows
            this.BorderRadius = 4;  // Slightly more squared, matching Linux aesthetic
            this.BorderSize = 1;
            this.ShadowOpacity = 0.10f;
            this.IsDarkTheme = true;  // Arc is a dark theme
        }
    }
}
