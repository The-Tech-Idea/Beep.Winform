using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class FluentTheme
    {
        private void ApplyComboBox()
        {
            this.ComboBoxBackColor = SurfaceColor;
            this.ComboBoxForeColor = ForeColor;
            this.ComboBoxBorderColor = BorderColor;
            this.ComboBoxHoverBackColor = SecondaryColor;
            this.ComboBoxHoverForeColor = ForeColor;
            this.ComboBoxHoverBorderColor = ActiveBorderColor;
            this.ComboBoxSelectedBackColor = PrimaryColor;
            this.ComboBoxSelectedForeColor = OnPrimaryColor;
            this.ComboBoxSelectedBorderColor = PrimaryColor;
            this.ComboBoxErrorBackColor = ErrorColor;
            this.ComboBoxErrorForeColor = OnPrimaryColor;
        }
    }
}