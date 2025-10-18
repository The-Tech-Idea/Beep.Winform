using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ArcLinuxTheme
    {
        private void ApplyTextBox()
        {
            this.TextBoxBackColor = Color.FromArgb(245,246,247);
            this.TextBoxForeColor = Color.FromArgb(43,45,48);
            this.TextBoxBorderColor = Color.FromArgb(220,223,230);
            this.TextBoxHoverBorderColor = Color.FromArgb(220,223,230);
            this.TextBoxHoverBackColor = Color.FromArgb(245,246,247);
            this.TextBoxHoverForeColor = Color.FromArgb(43,45,48);
            this.TextBoxSelectedBorderColor = Color.FromArgb(220,223,230);
            this.TextBoxSelectedBackColor = Color.FromArgb(245,246,247);
            this.TextBoxSelectedForeColor = Color.FromArgb(43,45,48);
            this.TextBoxPlaceholderColor = Color.FromArgb(245,246,247);
            this.TextBoxErrorBorderColor = Color.FromArgb(220,223,230);
            this.TextBoxErrorBackColor = Color.FromArgb(244,67,54);
            this.TextBoxErrorForeColor = Color.FromArgb(43,45,48);
            this.TextBoxErrorTextColor = Color.FromArgb(43,45,48);
            this.TextBoxErrorPlaceholderColor = Color.FromArgb(244,67,54);
            this.TextBoxErrorTextBoxColor = Color.FromArgb(244,67,54);
            this.TextBoxErrorTextBoxBorderColor = Color.FromArgb(220,223,230);
            this.TextBoxErrorTextBoxHoverColor = Color.FromArgb(244,67,54);
        }
    }
}