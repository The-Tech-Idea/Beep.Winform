using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeonTheme
    {
        private void ApplyCheckBox()
        {
            this.CheckBoxBackColor = PanelGradiantMiddleColor;
            this.CheckBoxForeColor = ForeColor;
            this.CheckBoxBorderColor = InactiveBorderColor;
            this.CheckBoxCheckedBackColor = PanelGradiantMiddleColor;
            this.CheckBoxCheckedForeColor = ForeColor;
            this.CheckBoxCheckedBorderColor = InactiveBorderColor;
            this.CheckBoxHoverBackColor = PanelGradiantMiddleColor;
            this.CheckBoxHoverForeColor = ForeColor;
            this.CheckBoxHoverBorderColor = InactiveBorderColor;
        }
    }
}