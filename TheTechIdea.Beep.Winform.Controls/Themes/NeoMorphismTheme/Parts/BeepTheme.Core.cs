using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeoMorphismTheme
    {
        private void ApplyCore()
        {
            // NeoMorphism theme - soft shadows aesthetic
            this.BorderRadius = 12;  // Matching FormStyle.NeoMorphism
            this.BorderSize = 0;
            this.ShadowOpacity = 0.14f;  // Soft neomorphic shadows
            this.IsDarkTheme = false;  // Light theme
        }
    }
}