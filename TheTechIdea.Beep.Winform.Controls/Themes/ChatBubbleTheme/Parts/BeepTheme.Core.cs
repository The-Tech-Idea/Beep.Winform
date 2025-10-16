using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ChatBubbleTheme
    {
        private void ApplyCore()
        {
            this.BorderRadius = 22;
            this.BorderSize = 2;
            this.ShadowOpacity = 0.16f;
            this.IsDarkTheme = false;
        }
    }
}