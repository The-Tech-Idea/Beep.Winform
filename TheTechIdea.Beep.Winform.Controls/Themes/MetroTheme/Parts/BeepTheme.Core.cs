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
            // Metro theme - Windows Metro design
            this.BorderRadius = 0;  // Matching FormStyle.Metro (sharp corners)
            this.BorderSize = 1;
            this.ShadowOpacity = 0.06f;  // Minimal shadows
            this.IsDarkTheme = false;  // Light theme
        }
    }
}