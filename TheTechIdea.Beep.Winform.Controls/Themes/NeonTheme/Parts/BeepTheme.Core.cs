using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeonTheme
    {
        private void ApplyCore()
        {
            // Neon theme - vibrant neon aesthetic
            this.BorderRadius = 6;  // Matching FormStyle.Neon
            this.BorderSize = 2;
            this.ShadowOpacity = 0.20f;  // Strong shadows for neon glow
            this.IsDarkTheme = true;  // Dark theme with neon accents
        }
    }
}