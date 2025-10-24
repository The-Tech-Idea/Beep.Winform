using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeonTheme
    {
        private void ApplyTextBox()
        {
            this.TextBoxBackColor = Color.FromArgb(10,12,20);
            this.TextBoxForeColor = Color.FromArgb(235,245,255);
            this.TextBoxBorderColor = Color.FromArgb(60,70,100);
            this.TextBoxHoverBorderColor = Color.FromArgb(60,70,100);
            this.TextBoxHoverBackColor = Color.FromArgb(10,12,20);
            this.TextBoxHoverForeColor = Color.FromArgb(235,245,255);
            this.TextBoxSelectedBorderColor = Color.FromArgb(60,70,100);
            this.TextBoxSelectedBackColor = Color.FromArgb(10,12,20);
            this.TextBoxSelectedForeColor = Color.FromArgb(235,245,255);
            this.TextBoxPlaceholderColor = Color.FromArgb(10,12,20);
            this.TextBoxErrorBorderColor = Color.FromArgb(60,70,100);
            this.TextBoxErrorBackColor = Color.FromArgb(10,12,20);
            this.TextBoxErrorForeColor = Color.FromArgb(235,245,255);
            this.TextBoxErrorTextColor = Color.FromArgb(235,245,255);
            this.TextBoxErrorPlaceholderColor = Color.FromArgb(10,12,20);
            this.TextBoxErrorTextBoxColor = Color.FromArgb(10,12,20);
            this.TextBoxErrorTextBoxBorderColor = Color.FromArgb(60,70,100);
            this.TextBoxErrorTextBoxHoverColor = Color.FromArgb(10,12,20);
        }
    }
}