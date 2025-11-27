using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class FluentTheme
    {
        private void ApplyRadioButton()
        {
            this.RadioButtonBackColor = SurfaceColor;
            this.RadioButtonForeColor = ForeColor;
            this.RadioButtonBorderColor = BorderColor;
            this.RadioButtonCheckedBackColor = PrimaryColor;
            this.RadioButtonCheckedForeColor = OnPrimaryColor;
            this.RadioButtonCheckedBorderColor = PrimaryColor;
            this.RadioButtonHoverBackColor = SecondaryColor;
            this.RadioButtonHoverForeColor = ForeColor;
            this.RadioButtonHoverBorderColor = ActiveBorderColor;
            this.RadioButtonSelectedForeColor = OnPrimaryColor;
            this.RadioButtonSelectedBackColor = PrimaryColor;
        }
    }
}