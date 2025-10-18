using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class UbuntuTheme
    {
        private void ApplyGradient()
        {
            this.GradientStartColor = Color.FromArgb(242,242,245);
            this.GradientEndColor = Color.FromArgb(242,242,245);
            this.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}