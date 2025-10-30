using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class DraculaTheme
    {
        private void ApplyCore()
        {
            // Dracula theme - smooth dark aesthetic
            this.BorderRadius = 6;  // Matching FormStyle.Dracula
            this.BorderSize = 1;
            this.ShadowOpacity = 0.18f;  // Subtle shadows
            this.IsDarkTheme = true;  // Dark theme
        }
    }
}