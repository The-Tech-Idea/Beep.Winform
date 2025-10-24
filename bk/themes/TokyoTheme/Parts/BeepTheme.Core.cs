using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class TokyoTheme
    {
        private void ApplyCore()
        {
            this.BorderRadius = 8;
            this.BorderSize = 1;
            this.ShadowOpacity = 0.16f;
            this.IsDarkTheme = true;
        }
    }
}