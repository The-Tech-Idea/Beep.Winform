using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class UbuntuTheme
    {
        private void ApplyTextBox()
        {
            this.TextBoxBackColor = SurfaceColor;
            this.TextBoxForeColor = ForeColor;
            this.TextBoxBorderColor = BorderColor;
            this.TextBoxHoverBorderColor = ActiveBorderColor;
            this.TextBoxHoverBackColor = SecondaryColor;
            this.TextBoxHoverForeColor = ForeColor;
            this.TextBoxSelectedBorderColor = PrimaryColor;
            this.TextBoxSelectedBackColor = PrimaryColor;
            this.TextBoxSelectedForeColor = OnPrimaryColor;
            this.TextBoxPlaceholderColor = ThemeUtil.Lighten(ForeColor, 0.6);
            this.TextBoxErrorBorderColor = ErrorColor;
            this.TextBoxErrorBackColor = SurfaceColor;
            this.TextBoxErrorForeColor = OnPrimaryColor;
            this.TextBoxErrorTextColor = OnPrimaryColor;
            this.TextBoxErrorPlaceholderColor = ErrorColor;
            this.TextBoxErrorTextBoxColor = SurfaceColor;
            this.TextBoxErrorTextBoxBorderColor = ErrorColor;
            this.TextBoxErrorTextBoxHoverColor = SurfaceColor;
        }
    }
}

