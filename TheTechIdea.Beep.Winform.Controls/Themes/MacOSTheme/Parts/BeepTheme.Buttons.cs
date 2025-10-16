using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MacOSTheme
    {
        private void ApplyButtons()
        {
            this.ButtonHoverBackColor = Color.FromArgb(250,250,252);
            this.ButtonHoverForeColor = Color.FromArgb(28,28,30);
            this.ButtonHoverBorderColor = Color.FromArgb(229,229,234);
            this.ButtonSelectedBorderColor = Color.FromArgb(229,229,234);
            this.ButtonSelectedBackColor = Color.FromArgb(250,250,252);
            this.ButtonSelectedForeColor = Color.FromArgb(28,28,30);
            this.ButtonSelectedHoverBackColor = Color.FromArgb(250,250,252);
            this.ButtonSelectedHoverForeColor = Color.FromArgb(28,28,30);
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(229,229,234);
            this.ButtonBackColor = Color.FromArgb(250,250,252);
            this.ButtonForeColor = Color.FromArgb(28,28,30);
            this.ButtonBorderColor = Color.FromArgb(229,229,234);
            this.ButtonErrorBackColor = Color.FromArgb(255,69,58);
            this.ButtonErrorForeColor = Color.FromArgb(28,28,30);
            this.ButtonErrorBorderColor = Color.FromArgb(229,229,234);
            this.ButtonPressedBackColor = Color.FromArgb(250,250,252);
            this.ButtonPressedForeColor = Color.FromArgb(28,28,30);
            this.ButtonPressedBorderColor = Color.FromArgb(229,229,234);
        }
    }
}