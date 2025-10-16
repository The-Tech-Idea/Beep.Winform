using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CyberpunkTheme
    {
        private void ApplyButtons()
        {
            this.ButtonHoverBackColor = Color.FromArgb(10,8,20);
            this.ButtonHoverForeColor = Color.FromArgb(228,244,255);
            this.ButtonHoverBorderColor = Color.FromArgb(90,20,110);
            this.ButtonSelectedBorderColor = Color.FromArgb(90,20,110);
            this.ButtonSelectedBackColor = Color.FromArgb(10,8,20);
            this.ButtonSelectedForeColor = Color.FromArgb(228,244,255);
            this.ButtonSelectedHoverBackColor = Color.FromArgb(10,8,20);
            this.ButtonSelectedHoverForeColor = Color.FromArgb(228,244,255);
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(90,20,110);
            this.ButtonBackColor = Color.FromArgb(10,8,20);
            this.ButtonForeColor = Color.FromArgb(228,244,255);
            this.ButtonBorderColor = Color.FromArgb(90,20,110);
            this.ButtonErrorBackColor = Color.FromArgb(10,8,20);
            this.ButtonErrorForeColor = Color.FromArgb(228,244,255);
            this.ButtonErrorBorderColor = Color.FromArgb(90,20,110);
            this.ButtonPressedBackColor = Color.FromArgb(10,8,20);
            this.ButtonPressedForeColor = Color.FromArgb(228,244,255);
            this.ButtonPressedBorderColor = Color.FromArgb(90,20,110);
        }
    }
}