using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class FluentTheme
    {
        private void ApplyTextBox()
        {
            this.TextBoxBackColor = Color.FromArgb(245,246,248);
            this.TextBoxForeColor = Color.FromArgb(32,32,32);
            this.TextBoxBorderColor = Color.FromArgb(218,223,230);
            this.TextBoxHoverBorderColor = Color.FromArgb(218,223,230);
            this.TextBoxHoverBackColor = Color.FromArgb(245,246,248);
            this.TextBoxHoverForeColor = Color.FromArgb(32,32,32);
            this.TextBoxSelectedBorderColor = Color.FromArgb(218,223,230);
            this.TextBoxSelectedBackColor = Color.FromArgb(245,246,248);
            this.TextBoxSelectedForeColor = Color.FromArgb(32,32,32);
            this.TextBoxPlaceholderColor = Color.FromArgb(245,246,248);
            this.TextBoxErrorBorderColor = Color.FromArgb(218,223,230);
            this.TextBoxErrorBackColor = Color.FromArgb(196,30,58);
            this.TextBoxErrorForeColor = Color.FromArgb(32,32,32);
            this.TextBoxErrorTextColor = Color.FromArgb(32,32,32);
            this.TextBoxErrorPlaceholderColor = Color.FromArgb(196,30,58);
            this.TextBoxErrorTextBoxColor = Color.FromArgb(196,30,58);
            this.TextBoxErrorTextBoxBorderColor = Color.FromArgb(218,223,230);
            this.TextBoxErrorTextBoxHoverColor = Color.FromArgb(196,30,58);
        }
    }
}