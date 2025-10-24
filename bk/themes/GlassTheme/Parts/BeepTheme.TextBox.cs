using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GlassTheme
    {
        private void ApplyTextBox()
        {
            this.TextBoxBackColor = Color.FromArgb(236,244,255);
            this.TextBoxForeColor = Color.FromArgb(17,24,39);
            this.TextBoxBorderColor = Color.FromArgb(140, 255, 255, 255);
            this.TextBoxHoverBorderColor = Color.FromArgb(140, 255, 255, 255);
            this.TextBoxHoverBackColor = Color.FromArgb(236,244,255);
            this.TextBoxHoverForeColor = Color.FromArgb(17,24,39);
            this.TextBoxSelectedBorderColor = Color.FromArgb(140, 255, 255, 255);
            this.TextBoxSelectedBackColor = Color.FromArgb(236,244,255);
            this.TextBoxSelectedForeColor = Color.FromArgb(17,24,39);
            this.TextBoxPlaceholderColor = Color.FromArgb(236,244,255);
            this.TextBoxErrorBorderColor = Color.FromArgb(140, 255, 255, 255);
            this.TextBoxErrorBackColor = Color.FromArgb(239,68,68);
            this.TextBoxErrorForeColor = Color.FromArgb(17,24,39);
            this.TextBoxErrorTextColor = Color.FromArgb(17,24,39);
            this.TextBoxErrorPlaceholderColor = Color.FromArgb(239,68,68);
            this.TextBoxErrorTextBoxColor = Color.FromArgb(239,68,68);
            this.TextBoxErrorTextBoxBorderColor = Color.FromArgb(140, 255, 255, 255);
            this.TextBoxErrorTextBoxHoverColor = Color.FromArgb(239,68,68);
        }
    }
}