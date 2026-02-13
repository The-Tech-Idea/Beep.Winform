using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class TokyoTheme
    {
        private void ApplyComboBox()
        {
            this.ComboBoxBackColor = BackgroundColor;
            this.ComboBoxForeColor = ForeColor;
            this.ComboBoxBorderColor = BorderColor;
            this.ComboBoxHoverBackColor = BackgroundColor;
            this.ComboBoxHoverForeColor = ForeColor;
            this.ComboBoxHoverBorderColor = BorderColor;
            this.ComboBoxSelectedBackColor = BackgroundColor;
            this.ComboBoxSelectedForeColor = ForeColor;
            this.ComboBoxSelectedBorderColor = BorderColor;
            this.ComboBoxErrorBackColor = ErrorColor;
            this.ComboBoxErrorForeColor = OnPrimaryColor;
        }
    }
}
