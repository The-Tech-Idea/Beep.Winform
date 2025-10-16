using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordicTheme
    {
        private void ApplyTextBox()
        {
            this.TextBoxBackColor = Color.FromArgb(250,250,251);
            this.TextBoxForeColor = Color.FromArgb(31,41,55);
            this.TextBoxBorderColor = Color.FromArgb(229,231,235);
            this.TextBoxHoverBorderColor = Color.FromArgb(229,231,235);
            this.TextBoxHoverBackColor = Color.FromArgb(250,250,251);
            this.TextBoxHoverForeColor = Color.FromArgb(31,41,55);
            this.TextBoxSelectedBorderColor = Color.FromArgb(229,231,235);
            this.TextBoxSelectedBackColor = Color.FromArgb(250,250,251);
            this.TextBoxSelectedForeColor = Color.FromArgb(31,41,55);
            this.TextBoxPlaceholderColor = Color.FromArgb(250,250,251);
            this.TextBoxErrorBorderColor = Color.FromArgb(229,231,235);
            this.TextBoxErrorBackColor = Color.FromArgb(220,38,38);
            this.TextBoxErrorForeColor = Color.FromArgb(31,41,55);
            this.TextBoxErrorTextColor = Color.FromArgb(31,41,55);
            this.TextBoxErrorPlaceholderColor = Color.FromArgb(220,38,38);
            this.TextBoxErrorTextBoxColor = Color.FromArgb(220,38,38);
            this.TextBoxErrorTextBoxBorderColor = Color.FromArgb(229,231,235);
            this.TextBoxErrorTextBoxHoverColor = Color.FromArgb(220,38,38);
        }
    }
}