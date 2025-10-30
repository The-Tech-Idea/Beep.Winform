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
            // Cartoon theme - playful rounded corners, thicker borders
            this.BorderRadius = 16;  // Matching FormStyle.Cartoon
            this.BorderSize = 3;  // Thick borders for cartoon look
            this.ShadowOpacity = 0.18f;  // Playful shadows
            this.IsDarkTheme = false;  // Light theme with purple accents
        }
    }
}