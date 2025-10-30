using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class UbuntuTheme
    {
        private void ApplyCore()
        {
            // Ubuntu theme - Ubuntu Linux desktop aesthetic
            this.BorderRadius = 8;  // Matching FormStyle.Ubuntu
            this.BorderSize = 1;
            this.ShadowOpacity = 0.10f;  // Subtle shadows
            this.IsDarkTheme = false;  // Light theme
        }
    }
}