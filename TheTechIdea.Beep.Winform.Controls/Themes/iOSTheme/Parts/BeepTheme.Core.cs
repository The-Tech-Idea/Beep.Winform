using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class iOSTheme
    {
        private void ApplyCore()
        {
            // iOS theme - clean, modern aesthetic
            this.BorderRadius = 12;  // Matching FormStyle.iOS
            this.BorderSize = 1;
            this.ShadowOpacity = 0.10f;  // Subtle shadows
            this.IsDarkTheme = false;  // Light theme
        }
    }
}