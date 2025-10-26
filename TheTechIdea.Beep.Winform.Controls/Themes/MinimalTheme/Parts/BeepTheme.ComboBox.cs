using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MinimalTheme
    {
        private void ApplyComboBox()
        {
            this.ComboBoxBackColor = BackgroundColor;
            this.ComboBoxForeColor = ForeColor;
            this.ComboBoxBorderColor = BorderColor;
            this.ComboBoxHoverBackColor = SurfaceColor;
            this.ComboBoxHoverForeColor = ForeColor;
            this.ComboBoxHoverBorderColor = ActiveBorderColor;
            this.ComboBoxSelectedBackColor = SurfaceColor;
            this.ComboBoxSelectedForeColor = ForeColor;
            this.ComboBoxSelectedBorderColor = ActiveBorderColor;
            this.ComboBoxErrorBackColor = ErrorColor;
            this.ComboBoxErrorForeColor = OnPrimaryColor;
        }
    }
}
