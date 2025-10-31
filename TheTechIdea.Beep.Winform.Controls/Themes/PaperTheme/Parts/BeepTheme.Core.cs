using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class PaperTheme
    {
        private void ApplyCore()
        {
            // Paper theme - Material Design paper aesthetic
            this.BorderRadius = 4;  // Matching FormStyle.Paper
            this.BorderSize = 1;
            this.ShadowOpacity = 0.12f;  // Material Design shadows
            this.IsDarkTheme = false;  // Light theme
        }
    }
}