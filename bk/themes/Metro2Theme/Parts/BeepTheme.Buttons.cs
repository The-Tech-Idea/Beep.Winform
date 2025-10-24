using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class Metro2Theme
    {
        private void ApplyButtons()
        {
            this.ButtonHoverBackColor = Color.FromArgb(0,120,215);
            this.ButtonHoverForeColor = Color.FromArgb(32,31,30);
            this.ButtonHoverBorderColor = Color.FromArgb(220,220,220);
            this.ButtonSelectedBorderColor = Color.FromArgb(220,220,220);
            this.ButtonSelectedBackColor = Color.FromArgb(0,120,215);
            this.ButtonSelectedForeColor = Color.FromArgb(32,31,30);
            this.ButtonSelectedHoverBackColor = Color.FromArgb(0,120,215);
            this.ButtonSelectedHoverForeColor = Color.FromArgb(32,31,30);
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(220,220,220);
            this.ButtonBackColor = Color.FromArgb(0,120,215);
            this.ButtonForeColor = Color.FromArgb(32,31,30);
            this.ButtonBorderColor = Color.FromArgb(220,220,220);
            this.ButtonErrorBackColor = Color.FromArgb(0,120,215);
            this.ButtonErrorForeColor = Color.FromArgb(32,31,30);
            this.ButtonErrorBorderColor = Color.FromArgb(220,220,220);
            this.ButtonPressedBackColor = Color.FromArgb(0,120,215);
            this.ButtonPressedForeColor = Color.FromArgb(32,31,30);
            this.ButtonPressedBorderColor = Color.FromArgb(220,220,220);
        }
    }
}