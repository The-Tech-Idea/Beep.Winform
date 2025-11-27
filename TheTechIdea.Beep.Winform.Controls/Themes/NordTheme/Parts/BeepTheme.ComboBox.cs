using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordTheme
    {
        private void ApplyComboBox()
        {
            this.ComboBoxBackColor = PanelBackColor;
            this.ComboBoxForeColor = ForeColor;
            this.ComboBoxBorderColor = BorderColor;
            this.ComboBoxHoverBackColor = PanelGradiantMiddleColor;
            this.ComboBoxHoverForeColor = ForeColor;
            this.ComboBoxHoverBorderColor = BorderColor;
            this.ComboBoxSelectedBackColor = PanelBackColor;
            this.ComboBoxSelectedForeColor = ForeColor;
            this.ComboBoxSelectedBorderColor = BorderColor;
            this.ComboBoxErrorBackColor = PanelBackColor;
            this.ComboBoxErrorForeColor = ForeColor;
        }
    }
}