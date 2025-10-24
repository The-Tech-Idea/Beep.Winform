using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CartoonTheme
    {
        private void ApplyCore()
        {
            this.BorderRadius = 18;
            this.BorderSize = 2;
            this.ShadowOpacity = 0.18f;
            this.IsDarkTheme = false;
        }
    }
}