using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ArcLinuxTheme
    {
        private void ApplyComboBox()
        {
            this.ComboBoxBackColor = SurfaceColor;
            this.ComboBoxForeColor = ForeColor;
            this.ComboBoxBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.2);
            this.ComboBoxHoverBackColor = ThemeUtil.Darken(SurfaceColor, 0.03);
            this.ComboBoxHoverForeColor = ForeColor;
            this.ComboBoxHoverBorderColor = ActiveBorderColor;
            this.ComboBoxSelectedBackColor = ThemeUtil.Darken(SurfaceColor, 0.06);
            this.ComboBoxSelectedForeColor = ForeColor;
            this.ComboBoxSelectedBorderColor = ActiveBorderColor;
            this.ComboBoxErrorBackColor = ErrorColor;
            this.ComboBoxErrorForeColor = OnPrimaryColor;
        }
    }
}
