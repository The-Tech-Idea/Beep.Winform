using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CartoonTheme
    {
        private void ApplyRadioButton()
        {
            this.RadioButtonBackColor = SurfaceColor;
            this.RadioButtonForeColor = ForeColor;
            this.RadioButtonBorderColor = BorderColor;
            this.RadioButtonCheckedBackColor = SecondaryColor;
            this.RadioButtonCheckedForeColor = ForeColor;
            this.RadioButtonCheckedBorderColor = BorderColor;
            this.RadioButtonHoverBackColor = SecondaryColor;
            this.RadioButtonHoverForeColor = ForeColor;
            this.RadioButtonHoverBorderColor = BorderColor;
            this.RadioButtonSelectedForeColor = ForeColor;
            this.RadioButtonSelectedBackColor = AccentColor;
        }
    }
}