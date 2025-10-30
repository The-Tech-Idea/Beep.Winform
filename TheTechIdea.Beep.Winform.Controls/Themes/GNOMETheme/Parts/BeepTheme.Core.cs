using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GNOMETheme
    {
        private void ApplyCore()
        {
            // GNOME theme - clean modern aesthetic
            this.BorderRadius = 8;  // Matching FormStyle.GNOME
            this.BorderSize = 1;
            this.ShadowOpacity = 0.10f;  // Subtle shadows
            this.IsDarkTheme = false;  // Light theme
        }
    }
}