using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class BrutalistTheme
    {
        private void ApplyCore()
        {
            this.BorderRadius = 0;
            this.BorderSize = 3;
            this.ShadowOpacity = 0.10f;
            this.IsDarkTheme = false;
        }
    }
}