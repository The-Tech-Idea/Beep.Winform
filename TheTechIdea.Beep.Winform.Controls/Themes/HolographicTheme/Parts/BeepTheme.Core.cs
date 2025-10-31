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
            // Holographic theme - futuristic aesthetic
            this.BorderRadius = 8;  // Matching FormStyle.Holographic
            this.BorderSize = 1;
            this.ShadowOpacity = 0.20f;  // Strong shadows for depth
            this.IsDarkTheme = true;  // Dark theme with neon accents
        }
    }
}