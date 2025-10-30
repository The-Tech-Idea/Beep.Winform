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
            // Metro2 theme - Windows Metro with accent stripe
            this.BorderRadius = 0;  // Matching FormStyle.Metro2 (sharp corners)
            this.BorderSize = 1;
            this.ShadowOpacity = 0.08f;  // Minimal shadows
            this.IsDarkTheme = false;  // Light theme
        }
    }
}