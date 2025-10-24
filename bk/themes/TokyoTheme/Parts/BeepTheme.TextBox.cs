using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class TokyoTheme
    {
        private void ApplyTextBox()
        {
            this.TextBoxBackColor = Color.FromArgb(26,27,38);
            this.TextBoxForeColor = Color.FromArgb(192,202,245);
            this.TextBoxBorderColor = Color.FromArgb(86,95,137);
            this.TextBoxHoverBorderColor = Color.FromArgb(86,95,137);
            this.TextBoxHoverBackColor = Color.FromArgb(26,27,38);
            this.TextBoxHoverForeColor = Color.FromArgb(192,202,245);
            this.TextBoxSelectedBorderColor = Color.FromArgb(86,95,137);
            this.TextBoxSelectedBackColor = Color.FromArgb(26,27,38);
            this.TextBoxSelectedForeColor = Color.FromArgb(192,202,245);
            this.TextBoxPlaceholderColor = Color.FromArgb(26,27,38);
            this.TextBoxErrorBorderColor = Color.FromArgb(86,95,137);
            this.TextBoxErrorBackColor = Color.FromArgb(26,27,38);
            this.TextBoxErrorForeColor = Color.FromArgb(192,202,245);
            this.TextBoxErrorTextColor = Color.FromArgb(192,202,245);
            this.TextBoxErrorPlaceholderColor = Color.FromArgb(26,27,38);
            this.TextBoxErrorTextBoxColor = Color.FromArgb(26,27,38);
            this.TextBoxErrorTextBoxBorderColor = Color.FromArgb(86,95,137);
            this.TextBoxErrorTextBoxHoverColor = Color.FromArgb(26,27,38);
        }
    }
}