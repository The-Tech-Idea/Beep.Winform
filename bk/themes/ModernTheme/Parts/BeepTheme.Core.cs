using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ModernTheme
    {
        private void ApplyCore()
        {
            this.BorderRadius = 12;
            this.BorderSize = 0;
            this.ShadowOpacity = 0.14f;
            this.IsDarkTheme = false;
        }
    }
}