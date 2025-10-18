using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ArcLinuxTheme
    {
        private void ApplyButtons()
        {
            this.ButtonHoverBackColor = Color.FromArgb(245,246,247);
            this.ButtonHoverForeColor = Color.FromArgb(43,45,48);
            this.ButtonHoverBorderColor = Color.FromArgb(220,223,230);
            this.ButtonSelectedBorderColor = Color.FromArgb(220,223,230);
            this.ButtonSelectedBackColor = Color.FromArgb(245,246,247);
            this.ButtonSelectedForeColor = Color.FromArgb(43,45,48);
            this.ButtonSelectedHoverBackColor = Color.FromArgb(245,246,247);
            this.ButtonSelectedHoverForeColor = Color.FromArgb(43,45,48);
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(220,223,230);
            this.ButtonBackColor = Color.FromArgb(245,246,247);
            this.ButtonForeColor = Color.FromArgb(43,45,48);
            this.ButtonBorderColor = Color.FromArgb(220,223,230);
            this.ButtonErrorBackColor = Color.FromArgb(244,67,54);
            this.ButtonErrorForeColor = Color.FromArgb(43,45,48);
            this.ButtonErrorBorderColor = Color.FromArgb(220,223,230);
            this.ButtonPressedBackColor = Color.FromArgb(245,246,247);
            this.ButtonPressedForeColor = Color.FromArgb(43,45,48);
            this.ButtonPressedBorderColor = Color.FromArgb(220,223,230);
        }
    }
}