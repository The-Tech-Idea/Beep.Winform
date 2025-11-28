using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GNOMETheme
    {
        private void ApplyComboBox()
        {
            this.ComboBoxBackColor = SurfaceColor;
            this.ComboBoxForeColor = ForeColor;
            this.ComboBoxBorderColor = BorderColor;
            this.ComboBoxHoverBackColor = PanelGradiantMiddleColor;
            this.ComboBoxHoverForeColor = ForeColor;
            this.ComboBoxHoverBorderColor = BorderColor;
            this.ComboBoxSelectedBackColor = SurfaceColor;
            this.ComboBoxSelectedForeColor = ForeColor;
            this.ComboBoxSelectedBorderColor = BorderColor;
            this.ComboBoxErrorBackColor = ErrorColor;
            this.ComboBoxErrorForeColor = OnPrimaryColor;
        }
    }
}