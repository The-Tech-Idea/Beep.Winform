using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class DraculaTheme
    {
        private void ApplyTextBox()
        {
            this.TextBoxBackColor = Color.FromArgb(40,42,54);
            this.TextBoxForeColor = Color.FromArgb(248,248,242);
            this.TextBoxBorderColor = Color.FromArgb(98,114,164);
            this.TextBoxHoverBorderColor = Color.FromArgb(98,114,164);
            this.TextBoxHoverBackColor = Color.FromArgb(40,42,54);
            this.TextBoxHoverForeColor = Color.FromArgb(248,248,242);
            this.TextBoxSelectedBorderColor = Color.FromArgb(98,114,164);
            this.TextBoxSelectedBackColor = Color.FromArgb(40,42,54);
            this.TextBoxSelectedForeColor = Color.FromArgb(248,248,242);
            this.TextBoxPlaceholderColor = Color.FromArgb(40,42,54);
            this.TextBoxErrorBorderColor = Color.FromArgb(98,114,164);
            this.TextBoxErrorBackColor = Color.FromArgb(40,42,54);
            this.TextBoxErrorForeColor = Color.FromArgb(248,248,242);
            this.TextBoxErrorTextColor = Color.FromArgb(248,248,242);
            this.TextBoxErrorPlaceholderColor = Color.FromArgb(40,42,54);
            this.TextBoxErrorTextBoxColor = Color.FromArgb(40,42,54);
            this.TextBoxErrorTextBoxBorderColor = Color.FromArgb(98,114,164);
            this.TextBoxErrorTextBoxHoverColor = Color.FromArgb(40,42,54);
        }
    }
}