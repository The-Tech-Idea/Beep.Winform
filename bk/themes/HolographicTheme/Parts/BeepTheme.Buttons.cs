using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class HolographicTheme
    {
        private void ApplyButtons()
        {
            this.ButtonHoverBackColor = Color.FromArgb(15,16,32);
            this.ButtonHoverForeColor = Color.FromArgb(245,247,255);
            this.ButtonHoverBorderColor = Color.FromArgb(74,79,123);
            this.ButtonSelectedBorderColor = Color.FromArgb(74,79,123);
            this.ButtonSelectedBackColor = Color.FromArgb(15,16,32);
            this.ButtonSelectedForeColor = Color.FromArgb(245,247,255);
            this.ButtonSelectedHoverBackColor = Color.FromArgb(15,16,32);
            this.ButtonSelectedHoverForeColor = Color.FromArgb(245,247,255);
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(74,79,123);
            this.ButtonBackColor = Color.FromArgb(15,16,32);
            this.ButtonForeColor = Color.FromArgb(245,247,255);
            this.ButtonBorderColor = Color.FromArgb(74,79,123);
            this.ButtonErrorBackColor = Color.FromArgb(15,16,32);
            this.ButtonErrorForeColor = Color.FromArgb(245,247,255);
            this.ButtonErrorBorderColor = Color.FromArgb(74,79,123);
            this.ButtonPressedBackColor = Color.FromArgb(15,16,32);
            this.ButtonPressedForeColor = Color.FromArgb(245,247,255);
            this.ButtonPressedBorderColor = Color.FromArgb(74,79,123);
        }
    }
}