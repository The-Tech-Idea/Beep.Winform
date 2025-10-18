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
            this.TextBoxBackColor = Color.FromArgb(242,242,245);
            this.TextBoxForeColor = Color.FromArgb(44,44,44);
            this.TextBoxBorderColor = Color.FromArgb(218,218,222);
            this.TextBoxHoverBorderColor = Color.FromArgb(218,218,222);
            this.TextBoxHoverBackColor = Color.FromArgb(242,242,245);
            this.TextBoxHoverForeColor = Color.FromArgb(44,44,44);
            this.TextBoxSelectedBorderColor = Color.FromArgb(218,218,222);
            this.TextBoxSelectedBackColor = Color.FromArgb(242,242,245);
            this.TextBoxSelectedForeColor = Color.FromArgb(44,44,44);
            this.TextBoxPlaceholderColor = Color.FromArgb(242,242,245);
            this.TextBoxErrorBorderColor = Color.FromArgb(218,218,222);
            this.TextBoxErrorBackColor = Color.FromArgb(192,28,40);
            this.TextBoxErrorForeColor = Color.FromArgb(44,44,44);
            this.TextBoxErrorTextColor = Color.FromArgb(44,44,44);
            this.TextBoxErrorPlaceholderColor = Color.FromArgb(192,28,40);
            this.TextBoxErrorTextBoxColor = Color.FromArgb(192,28,40);
            this.TextBoxErrorTextBoxBorderColor = Color.FromArgb(218,218,222);
            this.TextBoxErrorTextBoxHoverColor = Color.FromArgb(192,28,40);
        }
    }
}