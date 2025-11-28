using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class KDETheme
    {
        private void ApplyCheckBox()
        {
            this.CheckBoxBackColor = PanelBackColor;
            this.CheckBoxForeColor = ForeColor;
            this.CheckBoxBorderColor = BorderColor;
            this.CheckBoxCheckedBackColor = PanelBackColor;
            this.CheckBoxCheckedForeColor = ForeColor;
            this.CheckBoxCheckedBorderColor = BorderColor;
            this.CheckBoxHoverBackColor = PanelBackColor;
            this.CheckBoxHoverForeColor = ForeColor;
            this.CheckBoxHoverBorderColor = BorderColor;
        }
    }
}