using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ChatBubbleTheme
    {
        private void ApplyTextBox()
        {
            this.TextBoxBackColor = BackgroundColor;
            this.TextBoxForeColor = ForeColor;
            this.TextBoxBorderColor = BorderColor;
            this.TextBoxHoverBorderColor = BorderColor;
            this.TextBoxHoverBackColor = BackgroundColor;
            this.TextBoxHoverForeColor = ForeColor;
            this.TextBoxSelectedBorderColor = BorderColor;
            this.TextBoxSelectedBackColor = BackgroundColor;
            this.TextBoxSelectedForeColor = ForeColor;
            this.TextBoxPlaceholderColor = ThemeUtil.Lighten(ForeColor, 0.6);
            this.TextBoxErrorBorderColor = BorderColor;
            this.TextBoxErrorBackColor = ErrorColor;
            this.TextBoxErrorForeColor = ForeColor;
            this.TextBoxErrorTextColor = ForeColor;
            this.TextBoxErrorPlaceholderColor = ErrorColor;
            this.TextBoxErrorTextBoxColor = ErrorColor;
            this.TextBoxErrorTextBoxBorderColor = BorderColor;
            this.TextBoxErrorTextBoxHoverColor = ErrorColor;
        }
    }
}
