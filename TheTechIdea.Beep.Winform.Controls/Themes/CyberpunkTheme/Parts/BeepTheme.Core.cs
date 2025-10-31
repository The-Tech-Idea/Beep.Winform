using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CyberpunkTheme
    {
        private void ApplyCore()
        {
            // Cyberpunk theme - angular, neon aesthetic
            this.BorderRadius = 4;  // Matching FormStyle.Cyberpunk
            this.BorderSize = 2;  // Thicker borders for neon effect
            this.ShadowOpacity = 0.22f;  // Glowing shadows
            this.IsDarkTheme = true;  // Dark theme with neon accents
        }
    }
}