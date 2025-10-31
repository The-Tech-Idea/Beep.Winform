using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class FluentTheme
    {
        private void ApplyCore()
        {
            // Fluent theme - Microsoft Fluent Design System
            this.BorderRadius = 6;  // Matching FormStyle.Fluent
            this.BorderSize = 1;
            this.ShadowOpacity = 0.12f;  // Soft shadows
            this.IsDarkTheme = false;  // Light theme
        }
    }
}