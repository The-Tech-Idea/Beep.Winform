using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MinimalTheme
    {
        private void ApplyTextBox()
        {
            this.TextBoxBackColor = BackgroundColor;
            this.TextBoxForeColor = ForeColor;
            this.TextBoxBorderColor = BorderColor;
            this.TextBoxHoverBorderColor = ActiveBorderColor;
            this.TextBoxHoverBackColor = SurfaceColor;
            this.TextBoxHoverForeColor = ForeColor;
            this.TextBoxSelectedBorderColor = ActiveBorderColor;
            this.TextBoxSelectedBackColor = SurfaceColor;
            this.TextBoxSelectedForeColor = ForeColor;
            this.TextBoxPlaceholderColor = SecondaryColor;
            this.TextBoxErrorBorderColor = ErrorColor;
            this.TextBoxErrorBackColor = ThemeUtil.Lighten(BackgroundColor, 0.02);
            this.TextBoxErrorForeColor = ForeColor;
            this.TextBoxErrorTextColor = ForeColor;
            this.TextBoxErrorPlaceholderColor = ErrorColor;
            this.TextBoxErrorTextBoxColor = ErrorColor;
            this.TextBoxErrorTextBoxBorderColor = ErrorColor;
            this.TextBoxErrorTextBoxHoverColor = ErrorColor;
        }
    }
}
