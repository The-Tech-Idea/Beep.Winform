using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeonTheme
    {
        private void ApplyComboBox()
        {
            this.ComboBoxBackColor = PanelGradiantMiddleColor;
            this.ComboBoxForeColor = ForeColor;
            this.ComboBoxBorderColor = InactiveBorderColor;
            this.ComboBoxHoverBackColor = PanelGradiantMiddleColor;
            this.ComboBoxHoverForeColor = ForeColor;
            this.ComboBoxHoverBorderColor = InactiveBorderColor;
            this.ComboBoxSelectedBackColor = PanelGradiantMiddleColor;
            this.ComboBoxSelectedForeColor = ForeColor;
            this.ComboBoxSelectedBorderColor = InactiveBorderColor;
            this.ComboBoxErrorBackColor = PanelGradiantMiddleColor;
            this.ComboBoxErrorForeColor = ForeColor;
        }
    }
}