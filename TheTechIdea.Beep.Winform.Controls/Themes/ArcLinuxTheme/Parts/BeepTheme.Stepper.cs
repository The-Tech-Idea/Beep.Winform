using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ArcLinuxTheme
    {
        private void ApplyStepper()
        {
            this.StepperBackColor = SurfaceColor;
            this.StepperForeColor = ForeColor;
            this.StepperBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
            this.StepperItemForeColor = ForeColor;
            this.StepperItemHoverForeColor = ForeColor;
            this.StepperItemHoverBackColor = SurfaceColor;
            this.StepperItemSelectedForeColor = ForeColor;
            this.StepperItemSelectedBackColor = SurfaceColor;
            this.StepperItemSelectedBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
            this.StepperItemBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
            this.StepperItemHoverBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
            this.StepperItemCheckedBoxForeColor = ForeColor;
            this.StepperItemCheckedBoxBackColor = SurfaceColor;
            this.StepperItemCheckedBoxBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
        }
    }
}
