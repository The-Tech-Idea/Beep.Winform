using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class FluentTheme
    {
        private void ApplyButtons()
        {
            this.ButtonHoverBackColor = Color.FromArgb(245,246,248);
            this.ButtonHoverForeColor = Color.FromArgb(32,32,32);
            this.ButtonHoverBorderColor = Color.FromArgb(218,223,230);
            this.ButtonSelectedBorderColor = Color.FromArgb(218,223,230);
            this.ButtonSelectedBackColor = Color.FromArgb(245,246,248);
            this.ButtonSelectedForeColor = Color.FromArgb(32,32,32);
            this.ButtonSelectedHoverBackColor = Color.FromArgb(245,246,248);
            this.ButtonSelectedHoverForeColor = Color.FromArgb(32,32,32);
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(218,223,230);
            this.ButtonBackColor = Color.FromArgb(245,246,248);
            this.ButtonForeColor = Color.FromArgb(32,32,32);
            this.ButtonBorderColor = Color.FromArgb(218,223,230);
            this.ButtonErrorBackColor = Color.FromArgb(196,30,58);
            this.ButtonErrorForeColor = Color.FromArgb(32,32,32);
            this.ButtonErrorBorderColor = Color.FromArgb(218,223,230);
            this.ButtonPressedBackColor = Color.FromArgb(245,246,248);
            this.ButtonPressedForeColor = Color.FromArgb(32,32,32);
            this.ButtonPressedBorderColor = Color.FromArgb(218,223,230);
        }
    }
}