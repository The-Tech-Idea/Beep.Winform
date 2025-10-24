using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class iOSTheme
    {
        private void ApplyTextBox()
        {
            this.TextBoxBackColor = Color.FromArgb(242,242,247);
            this.TextBoxForeColor = Color.FromArgb(28,28,30);
            this.TextBoxBorderColor = Color.FromArgb(198,198,207);
            this.TextBoxHoverBorderColor = Color.FromArgb(198,198,207);
            this.TextBoxHoverBackColor = Color.FromArgb(242,242,247);
            this.TextBoxHoverForeColor = Color.FromArgb(28,28,30);
            this.TextBoxSelectedBorderColor = Color.FromArgb(198,198,207);
            this.TextBoxSelectedBackColor = Color.FromArgb(242,242,247);
            this.TextBoxSelectedForeColor = Color.FromArgb(28,28,30);
            this.TextBoxPlaceholderColor = Color.FromArgb(242,242,247);
            this.TextBoxErrorBorderColor = Color.FromArgb(198,198,207);
            this.TextBoxErrorBackColor = Color.FromArgb(242,242,247);
            this.TextBoxErrorForeColor = Color.FromArgb(28,28,30);
            this.TextBoxErrorTextColor = Color.FromArgb(28,28,30);
            this.TextBoxErrorPlaceholderColor = Color.FromArgb(242,242,247);
            this.TextBoxErrorTextBoxColor = Color.FromArgb(242,242,247);
            this.TextBoxErrorTextBoxBorderColor = Color.FromArgb(198,198,207);
            this.TextBoxErrorTextBoxHoverColor = Color.FromArgb(242,242,247);
        }
    }
}