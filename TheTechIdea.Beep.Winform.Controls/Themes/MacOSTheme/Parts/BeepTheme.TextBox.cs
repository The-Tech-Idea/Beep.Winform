using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MacOSTheme
    {
        private void ApplyTextBox()
        {
            this.TextBoxBackColor = Color.FromArgb(250,250,252);
            this.TextBoxForeColor = Color.FromArgb(28,28,30);
            this.TextBoxBorderColor = Color.FromArgb(229,229,234);
            this.TextBoxHoverBorderColor = Color.FromArgb(229,229,234);
            this.TextBoxHoverBackColor = Color.FromArgb(250,250,252);
            this.TextBoxHoverForeColor = Color.FromArgb(28,28,30);
            this.TextBoxSelectedBorderColor = Color.FromArgb(229,229,234);
            this.TextBoxSelectedBackColor = Color.FromArgb(250,250,252);
            this.TextBoxSelectedForeColor = Color.FromArgb(28,28,30);
            this.TextBoxPlaceholderColor = Color.FromArgb(250,250,252);
            this.TextBoxErrorBorderColor = Color.FromArgb(229,229,234);
            this.TextBoxErrorBackColor = Color.FromArgb(255,69,58);
            this.TextBoxErrorForeColor = Color.FromArgb(28,28,30);
            this.TextBoxErrorTextColor = Color.FromArgb(28,28,30);
            this.TextBoxErrorPlaceholderColor = Color.FromArgb(255,69,58);
            this.TextBoxErrorTextBoxColor = Color.FromArgb(255,69,58);
            this.TextBoxErrorTextBoxBorderColor = Color.FromArgb(229,229,234);
            this.TextBoxErrorTextBoxHoverColor = Color.FromArgb(255,69,58);
        }
    }
}