using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GNOMETheme
    {
        private void ApplyCheckBox()
        {
            this.CheckBoxBackColor = SurfaceColor;
            this.CheckBoxForeColor = ForeColor;
            this.CheckBoxBorderColor = InactiveBorderColor;
            this.CheckBoxCheckedBackColor = PrimaryColor;
            this.CheckBoxCheckedForeColor = OnPrimaryColor;
            this.CheckBoxCheckedBorderColor = PrimaryColor;
            this.CheckBoxHoverBackColor = PanelGradiantStartColor;
            this.CheckBoxHoverForeColor = ForeColor;
            this.CheckBoxHoverBorderColor = ActiveBorderColor;
        }
    }
}