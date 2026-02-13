using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CyberpunkTheme
    {
        private void ApplyComboBox()
        {
            this.ComboBoxBackColor = BackgroundColor;
            this.ComboBoxForeColor = ForeColor;
            this.ComboBoxBorderColor = SecondaryColor;
            this.ComboBoxHoverBackColor = PanelBackColor;
            this.ComboBoxHoverForeColor = ForeColor;
            this.ComboBoxHoverBorderColor = SecondaryColor;
            this.ComboBoxSelectedBackColor = PanelBackColor;
            this.ComboBoxSelectedForeColor = ForeColor;
            this.ComboBoxSelectedBorderColor = SecondaryColor;
            this.ComboBoxErrorBackColor = ErrorColor;
            this.ComboBoxErrorForeColor = OnPrimaryColor;
        }
    }
}
