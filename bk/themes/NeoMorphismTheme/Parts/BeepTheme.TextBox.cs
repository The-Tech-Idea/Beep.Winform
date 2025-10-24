using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeoMorphismTheme
    {
        private void ApplyTextBox()
        {
            this.TextBoxBackColor = Color.FromArgb(236,240,243);
            this.TextBoxForeColor = Color.FromArgb(58,66,86);
            this.TextBoxBorderColor = Color.FromArgb(221,228,235);
            this.TextBoxHoverBorderColor = Color.FromArgb(221,228,235);
            this.TextBoxHoverBackColor = Color.FromArgb(236,240,243);
            this.TextBoxHoverForeColor = Color.FromArgb(58,66,86);
            this.TextBoxSelectedBorderColor = Color.FromArgb(221,228,235);
            this.TextBoxSelectedBackColor = Color.FromArgb(236,240,243);
            this.TextBoxSelectedForeColor = Color.FromArgb(58,66,86);
            this.TextBoxPlaceholderColor = Color.FromArgb(236,240,243);
            this.TextBoxErrorBorderColor = Color.FromArgb(221,228,235);
            this.TextBoxErrorBackColor = Color.FromArgb(231,76,60);
            this.TextBoxErrorForeColor = Color.FromArgb(58,66,86);
            this.TextBoxErrorTextColor = Color.FromArgb(58,66,86);
            this.TextBoxErrorPlaceholderColor = Color.FromArgb(231,76,60);
            this.TextBoxErrorTextBoxColor = Color.FromArgb(231,76,60);
            this.TextBoxErrorTextBoxBorderColor = Color.FromArgb(221,228,235);
            this.TextBoxErrorTextBoxHoverColor = Color.FromArgb(231,76,60);
        }
    }
}