using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeonTheme
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
            this.TextBoxPlaceholderColor = InactiveBorderColor;
            this.TextBoxErrorBorderColor = ErrorColor;
            this.TextBoxErrorBackColor = BackgroundColor;
            this.TextBoxErrorForeColor = OnPrimaryColor;
            this.TextBoxErrorTextColor = OnPrimaryColor;
            this.TextBoxErrorPlaceholderColor = ErrorColor;
            this.TextBoxErrorTextBoxColor = BackgroundColor;
            this.TextBoxErrorTextBoxBorderColor = ErrorColor;
            this.TextBoxErrorTextBoxHoverColor = BackgroundColor;
        }
    }
}
