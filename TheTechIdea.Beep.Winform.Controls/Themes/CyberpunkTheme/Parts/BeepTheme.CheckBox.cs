using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CyberpunkTheme
    {
        private void ApplyCheckBox()
        {
            this.CheckBoxBackColor = BackgroundColor;
            this.CheckBoxForeColor = ForeColor;
            this.CheckBoxBorderColor = SecondaryColor;
            this.CheckBoxCheckedBackColor = PanelBackColor;
            this.CheckBoxCheckedForeColor = ForeColor;
            this.CheckBoxCheckedBorderColor = SecondaryColor;
            this.CheckBoxHoverBackColor = PanelGradiantMiddleColor;
            this.CheckBoxHoverForeColor = ForeColor;
            this.CheckBoxHoverBorderColor = SecondaryColor;
        }
    }
}