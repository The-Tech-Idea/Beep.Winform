using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class PaperTheme
    {
        private void ApplyRadioButton()
        {
            this.RadioButtonBackColor = BackgroundColor;
            this.RadioButtonForeColor = ForeColor;
            this.RadioButtonBorderColor = InactiveBorderColor;
            this.RadioButtonCheckedBackColor = BackgroundColor;
            this.RadioButtonCheckedForeColor = ForeColor;
            this.RadioButtonCheckedBorderColor = InactiveBorderColor;
            this.RadioButtonHoverBackColor = BackgroundColor;
            this.RadioButtonHoverForeColor = ForeColor;
            this.RadioButtonHoverBorderColor = InactiveBorderColor;
            this.RadioButtonSelectedForeColor = ForeColor;
            this.RadioButtonSelectedBackColor = BackgroundColor;
        }
    }
}