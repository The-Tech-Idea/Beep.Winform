using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GlassTheme
    {
        private void ApplyButtons()
        {
            this.ButtonHoverBackColor = Color.FromArgb(236,244,255);
            this.ButtonHoverForeColor = Color.FromArgb(17,24,39);
            this.ButtonHoverBorderColor = Color.FromArgb(140, 255, 255, 255);
            this.ButtonSelectedBorderColor = Color.FromArgb(140, 255, 255, 255);
            this.ButtonSelectedBackColor = Color.FromArgb(236,244,255);
            this.ButtonSelectedForeColor = Color.FromArgb(17,24,39);
            this.ButtonSelectedHoverBackColor = Color.FromArgb(236,244,255);
            this.ButtonSelectedHoverForeColor = Color.FromArgb(17,24,39);
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(140, 255, 255, 255);
            this.ButtonBackColor = Color.FromArgb(236,244,255);
            this.ButtonForeColor = Color.FromArgb(17,24,39);
            this.ButtonBorderColor = Color.FromArgb(140, 255, 255, 255);
            this.ButtonErrorBackColor = Color.FromArgb(239,68,68);
            this.ButtonErrorForeColor = Color.FromArgb(17,24,39);
            this.ButtonErrorBorderColor = Color.FromArgb(140, 255, 255, 255);
            this.ButtonPressedBackColor = Color.FromArgb(236,244,255);
            this.ButtonPressedForeColor = Color.FromArgb(17,24,39);
            this.ButtonPressedBorderColor = Color.FromArgb(140, 255, 255, 255);
        }
    }
}