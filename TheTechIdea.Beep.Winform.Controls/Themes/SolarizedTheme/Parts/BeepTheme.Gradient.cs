using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class SolarizedTheme
    {
        private void ApplyGradient()
        {
            this.GradientStartColor = Color.FromArgb(0,43,54);
            this.GradientEndColor = Color.FromArgb(0,43,54);
            this.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}