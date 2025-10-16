using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class Metro2Theme
    {
        private void ApplyTextBox()
        {
            this.TextBoxBackColor = Color.FromArgb(243,242,241);
            this.TextBoxForeColor = Color.FromArgb(32,31,30);
            this.TextBoxBorderColor = Color.FromArgb(220,220,220);
            this.TextBoxHoverBorderColor = Color.FromArgb(220,220,220);
            this.TextBoxHoverBackColor = Color.FromArgb(243,242,241);
            this.TextBoxHoverForeColor = Color.FromArgb(32,31,30);
            this.TextBoxSelectedBorderColor = Color.FromArgb(220,220,220);
            this.TextBoxSelectedBackColor = Color.FromArgb(243,242,241);
            this.TextBoxSelectedForeColor = Color.FromArgb(32,31,30);
            this.TextBoxPlaceholderColor = Color.FromArgb(243,242,241);
            this.TextBoxErrorBorderColor = Color.FromArgb(220,220,220);
            this.TextBoxErrorBackColor = Color.FromArgb(232,17,35);
            this.TextBoxErrorForeColor = Color.FromArgb(32,31,30);
            this.TextBoxErrorTextColor = Color.FromArgb(32,31,30);
            this.TextBoxErrorPlaceholderColor = Color.FromArgb(232,17,35);
            this.TextBoxErrorTextBoxColor = Color.FromArgb(232,17,35);
            this.TextBoxErrorTextBoxBorderColor = Color.FromArgb(220,220,220);
            this.TextBoxErrorTextBoxHoverColor = Color.FromArgb(232,17,35);
        }
    }
}