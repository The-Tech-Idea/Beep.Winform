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
            // ChatBubble theme - rounded, soft aesthetic
            this.BorderRadius = 12;  // Matching FormStyle.ChatBubble
            this.BorderSize = 2;
            this.ShadowOpacity = 0.16f;  // Soft shadows
            this.IsDarkTheme = false;  // Light theme with blue accents
        }
    }
}