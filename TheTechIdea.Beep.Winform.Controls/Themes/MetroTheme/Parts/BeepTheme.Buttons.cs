using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MetroTheme
    {
        private void ApplyButtons()
        {
            this.ButtonHoverBackColor = Color.FromArgb(243,242,241);
            this.ButtonHoverForeColor = Color.FromArgb(32,31,30);
            this.ButtonHoverBorderColor = Color.FromArgb(225,225,225);
            this.ButtonSelectedBorderColor = Color.FromArgb(225,225,225);
            this.ButtonSelectedBackColor = Color.FromArgb(243,242,241);
            this.ButtonSelectedForeColor = Color.FromArgb(32,31,30);
            this.ButtonSelectedHoverBackColor = Color.FromArgb(243,242,241);
            this.ButtonSelectedHoverForeColor = Color.FromArgb(32,31,30);
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(225,225,225);
            this.ButtonBackColor = Color.FromArgb(243,242,241);
            this.ButtonForeColor = Color.FromArgb(32,31,30);
            this.ButtonBorderColor = Color.FromArgb(225,225,225);
            this.ButtonErrorBackColor = Color.FromArgb(196,30,58);
            this.ButtonErrorForeColor = Color.FromArgb(32,31,30);
            this.ButtonErrorBorderColor = Color.FromArgb(225,225,225);
            this.ButtonPressedBackColor = Color.FromArgb(243,242,241);
            this.ButtonPressedForeColor = Color.FromArgb(32,31,30);
            this.ButtonPressedBorderColor = Color.FromArgb(225,225,225);
        }
    }
}