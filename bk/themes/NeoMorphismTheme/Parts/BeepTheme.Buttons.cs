using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeoMorphismTheme
    {
        private void ApplyButtons()
        {
            this.ButtonHoverBackColor = Color.FromArgb(236,240,243);
            this.ButtonHoverForeColor = Color.FromArgb(58,66,86);
            this.ButtonHoverBorderColor = Color.FromArgb(221,228,235);
            this.ButtonSelectedBorderColor = Color.FromArgb(221,228,235);
            this.ButtonSelectedBackColor = Color.FromArgb(236,240,243);
            this.ButtonSelectedForeColor = Color.FromArgb(58,66,86);
            this.ButtonSelectedHoverBackColor = Color.FromArgb(236,240,243);
            this.ButtonSelectedHoverForeColor = Color.FromArgb(58,66,86);
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(221,228,235);
            this.ButtonBackColor = Color.FromArgb(236,240,243);
            this.ButtonForeColor = Color.FromArgb(58,66,86);
            this.ButtonBorderColor = Color.FromArgb(221,228,235);
            this.ButtonErrorBackColor = Color.FromArgb(231,76,60);
            this.ButtonErrorForeColor = Color.FromArgb(58,66,86);
            this.ButtonErrorBorderColor = Color.FromArgb(221,228,235);
            this.ButtonPressedBackColor = Color.FromArgb(236,240,243);
            this.ButtonPressedForeColor = Color.FromArgb(58,66,86);
            this.ButtonPressedBorderColor = Color.FromArgb(221,228,235);
        }
    }
}