using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class TokyoTheme
    {
        private void ApplyCore()
        {
            // Tokyo Night theme - inspired by Tokyo Night VSCode theme
            this.BorderRadius = 8;  // Matching FormStyle.Tokyo
            this.BorderSize = 1;
            this.ShadowOpacity = 0.16f;  // Moderate shadows
            this.IsDarkTheme = true;  // Dark theme
        }
    }
}