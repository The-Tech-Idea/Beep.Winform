using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GruvBoxTheme
    {
        private void ApplyCore()
        {
            // GruvBox theme - retro groove aesthetic
            this.BorderRadius = 4;  // Matching FormStyle.GruvBox
            this.BorderSize = 1;
            this.ShadowOpacity = 0.16f;  // Moderate shadows
            this.IsDarkTheme = true;  // Dark theme
        }
    }
}