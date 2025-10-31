using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GlassTheme
    {
        private void ApplyCore()
        {
            // Glass theme - frosted glass aesthetic
            this.BorderRadius = 8;  // Matching FormStyle.Glass
            this.BorderSize = 1;
            this.ShadowOpacity = 0.14f;  // Soft shadows for depth
            this.IsDarkTheme = false;  // Light theme with transparency
        }
    }
}