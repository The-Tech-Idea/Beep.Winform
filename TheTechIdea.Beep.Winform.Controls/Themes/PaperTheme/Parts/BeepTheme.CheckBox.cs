using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class PaperTheme
    {
        private void ApplyCheckBox()
        {
            this.CheckBoxBackColor = BackgroundColor;
            this.CheckBoxForeColor = ForeColor;
            this.CheckBoxBorderColor = InactiveBorderColor;
            this.CheckBoxCheckedBackColor = BackgroundColor;
            this.CheckBoxCheckedForeColor = ForeColor;
            this.CheckBoxCheckedBorderColor = InactiveBorderColor;
            this.CheckBoxHoverBackColor = BackgroundColor;
            this.CheckBoxHoverForeColor = ForeColor;
            this.CheckBoxHoverBorderColor = InactiveBorderColor;
        }
    }
}