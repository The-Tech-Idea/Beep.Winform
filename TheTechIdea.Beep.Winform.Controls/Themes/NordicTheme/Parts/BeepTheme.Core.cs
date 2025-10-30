using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordicTheme
    {
        private void ApplyCore()
        {
            // Nordic theme - Scandinavian minimalist aesthetic
            this.BorderRadius = 6;  // Matching FormStyle.Nordic
            this.BorderSize = 1;
            this.ShadowOpacity = 0.08f;  // Minimal shadows
            this.IsDarkTheme = false;  // Light theme
        }
    }
}