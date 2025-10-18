using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class HolographicTheme
    {
        private void ApplyTextBox()
        {
            this.TextBoxBackColor = Color.FromArgb(15,16,32);
            this.TextBoxForeColor = Color.FromArgb(245,247,255);
            this.TextBoxBorderColor = Color.FromArgb(74,79,123);
            this.TextBoxHoverBorderColor = Color.FromArgb(74,79,123);
            this.TextBoxHoverBackColor = Color.FromArgb(15,16,32);
            this.TextBoxHoverForeColor = Color.FromArgb(245,247,255);
            this.TextBoxSelectedBorderColor = Color.FromArgb(74,79,123);
            this.TextBoxSelectedBackColor = Color.FromArgb(15,16,32);
            this.TextBoxSelectedForeColor = Color.FromArgb(245,247,255);
            this.TextBoxPlaceholderColor = Color.FromArgb(15,16,32);
            this.TextBoxErrorBorderColor = Color.FromArgb(74,79,123);
            this.TextBoxErrorBackColor = Color.FromArgb(15,16,32);
            this.TextBoxErrorForeColor = Color.FromArgb(245,247,255);
            this.TextBoxErrorTextColor = Color.FromArgb(245,247,255);
            this.TextBoxErrorPlaceholderColor = Color.FromArgb(15,16,32);
            this.TextBoxErrorTextBoxColor = Color.FromArgb(15,16,32);
            this.TextBoxErrorTextBoxBorderColor = Color.FromArgb(74,79,123);
            this.TextBoxErrorTextBoxHoverColor = Color.FromArgb(15,16,32);
        }
    }
}