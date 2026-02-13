using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class PaperTheme
    {
        private void ApplyComboBox()
        {
            this.ComboBoxBackColor = BackgroundColor;
            this.ComboBoxForeColor = ForeColor;
            this.ComboBoxBorderColor = InactiveBorderColor;
            this.ComboBoxHoverBackColor = BackgroundColor;
            this.ComboBoxHoverForeColor = ForeColor;
            this.ComboBoxHoverBorderColor = InactiveBorderColor;
            this.ComboBoxSelectedBackColor = BackgroundColor;
            this.ComboBoxSelectedForeColor = ForeColor;
            this.ComboBoxSelectedBorderColor = InactiveBorderColor;
            this.ComboBoxErrorBackColor = ErrorColor;
            this.ComboBoxErrorForeColor = OnPrimaryColor;
        }
    }
}
