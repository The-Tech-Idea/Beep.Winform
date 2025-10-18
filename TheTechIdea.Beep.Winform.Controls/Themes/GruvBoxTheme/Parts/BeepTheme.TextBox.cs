using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GruvBoxTheme
    {
        private void ApplyTextBox()
        {
            this.TextBoxBackColor = Color.FromArgb(40,40,40);
            this.TextBoxForeColor = Color.FromArgb(235,219,178);
            this.TextBoxBorderColor = Color.FromArgb(168,153,132);
            this.TextBoxHoverBorderColor = Color.FromArgb(168,153,132);
            this.TextBoxHoverBackColor = Color.FromArgb(40,40,40);
            this.TextBoxHoverForeColor = Color.FromArgb(235,219,178);
            this.TextBoxSelectedBorderColor = Color.FromArgb(168,153,132);
            this.TextBoxSelectedBackColor = Color.FromArgb(40,40,40);
            this.TextBoxSelectedForeColor = Color.FromArgb(235,219,178);
            this.TextBoxPlaceholderColor = Color.FromArgb(40,40,40);
            this.TextBoxErrorBorderColor = Color.FromArgb(168,153,132);
            this.TextBoxErrorBackColor = Color.FromArgb(40,40,40);
            this.TextBoxErrorForeColor = Color.FromArgb(235,219,178);
            this.TextBoxErrorTextColor = Color.FromArgb(235,219,178);
            this.TextBoxErrorPlaceholderColor = Color.FromArgb(40,40,40);
            this.TextBoxErrorTextBoxColor = Color.FromArgb(40,40,40);
            this.TextBoxErrorTextBoxBorderColor = Color.FromArgb(168,153,132);
            this.TextBoxErrorTextBoxHoverColor = Color.FromArgb(40,40,40);
        }
    }
}