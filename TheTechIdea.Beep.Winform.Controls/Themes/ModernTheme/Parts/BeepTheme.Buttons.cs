using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ModernTheme
    {
        private void ApplyButtons()
        {
            this.ButtonHoverBackColor = Color.FromArgb(255,255,255);
            this.ButtonHoverForeColor = Color.FromArgb(17,24,39);
            this.ButtonHoverBorderColor = Color.FromArgb(203,213,225);
            this.ButtonSelectedBorderColor = Color.FromArgb(203,213,225);
            this.ButtonSelectedBackColor = Color.FromArgb(255,255,255);
            this.ButtonSelectedForeColor = Color.FromArgb(17,24,39);
            this.ButtonSelectedHoverBackColor = Color.FromArgb(255,255,255);
            this.ButtonSelectedHoverForeColor = Color.FromArgb(17,24,39);
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(203,213,225);
            this.ButtonBackColor = Color.FromArgb(255,255,255);
            this.ButtonForeColor = Color.FromArgb(17,24,39);
            this.ButtonBorderColor = Color.FromArgb(203,213,225);
            this.ButtonErrorBackColor = Color.FromArgb(239,68,68);
            this.ButtonErrorForeColor = Color.FromArgb(17,24,39);
            this.ButtonErrorBorderColor = Color.FromArgb(203,213,225);
            this.ButtonPressedBackColor = Color.FromArgb(255,255,255);
            this.ButtonPressedForeColor = Color.FromArgb(17,24,39);
            this.ButtonPressedBorderColor = Color.FromArgb(203,213,225);
        }
    }
}