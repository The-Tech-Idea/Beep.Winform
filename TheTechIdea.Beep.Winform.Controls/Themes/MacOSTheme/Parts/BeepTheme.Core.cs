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
            // MacOS theme - clean macOS aesthetic
            this.BorderRadius = 12;  // Matching FormStyle.MacOS
            this.BorderSize = 0;
            this.ShadowOpacity = 0.10f;  // Subtle shadows
            this.IsDarkTheme = false;  // Light theme
        }
    }
}