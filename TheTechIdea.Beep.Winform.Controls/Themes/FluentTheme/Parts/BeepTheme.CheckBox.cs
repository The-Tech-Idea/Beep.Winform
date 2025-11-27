using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class FluentTheme
    {
        private void ApplyCheckBox()
        {
            this.CheckBoxBackColor = SurfaceColor;
            this.CheckBoxForeColor = ForeColor;
            this.CheckBoxBorderColor = BorderColor;
            this.CheckBoxCheckedBackColor = PrimaryColor;
            this.CheckBoxCheckedForeColor = OnPrimaryColor;
            this.CheckBoxCheckedBorderColor = PrimaryColor;
            this.CheckBoxHoverBackColor = SecondaryColor;
            this.CheckBoxHoverForeColor = ForeColor;
            this.CheckBoxHoverBorderColor = ActiveBorderColor;
        }
    }
}