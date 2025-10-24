using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MinimalTheme
    {
        private void ApplyTextBox()
        {
            this.TextBoxBackColor = Color.FromArgb(255,255,255);
            this.TextBoxForeColor = Color.FromArgb(31,41,55);
            this.TextBoxBorderColor = Color.FromArgb(209,213,219);
            this.TextBoxHoverBorderColor = Color.FromArgb(209,213,219);
            this.TextBoxHoverBackColor = Color.FromArgb(255,255,255);
            this.TextBoxHoverForeColor = Color.FromArgb(31,41,55);
            this.TextBoxSelectedBorderColor = Color.FromArgb(209,213,219);
            this.TextBoxSelectedBackColor = Color.FromArgb(255,255,255);
            this.TextBoxSelectedForeColor = Color.FromArgb(31,41,55);
            this.TextBoxPlaceholderColor = Color.FromArgb(255,255,255);
            this.TextBoxErrorBorderColor = Color.FromArgb(209,213,219);
            this.TextBoxErrorBackColor = Color.FromArgb(239,68,68);
            this.TextBoxErrorForeColor = Color.FromArgb(31,41,55);
            this.TextBoxErrorTextColor = Color.FromArgb(31,41,55);
            this.TextBoxErrorPlaceholderColor = Color.FromArgb(239,68,68);
            this.TextBoxErrorTextBoxColor = Color.FromArgb(239,68,68);
            this.TextBoxErrorTextBoxBorderColor = Color.FromArgb(209,213,219);
            this.TextBoxErrorTextBoxHoverColor = Color.FromArgb(239,68,68);
        }
    }
}