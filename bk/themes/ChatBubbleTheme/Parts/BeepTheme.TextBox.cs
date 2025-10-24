using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ChatBubbleTheme
    {
        private void ApplyTextBox()
        {
            this.TextBoxBackColor = Color.FromArgb(245,248,255);
            this.TextBoxForeColor = Color.FromArgb(24,28,35);
            this.TextBoxBorderColor = Color.FromArgb(210,220,235);
            this.TextBoxHoverBorderColor = Color.FromArgb(210,220,235);
            this.TextBoxHoverBackColor = Color.FromArgb(245,248,255);
            this.TextBoxHoverForeColor = Color.FromArgb(24,28,35);
            this.TextBoxSelectedBorderColor = Color.FromArgb(210,220,235);
            this.TextBoxSelectedBackColor = Color.FromArgb(245,248,255);
            this.TextBoxSelectedForeColor = Color.FromArgb(24,28,35);
            this.TextBoxPlaceholderColor = Color.FromArgb(245,248,255);
            this.TextBoxErrorBorderColor = Color.FromArgb(210,220,235);
            this.TextBoxErrorBackColor = Color.FromArgb(245,80,100);
            this.TextBoxErrorForeColor = Color.FromArgb(24,28,35);
            this.TextBoxErrorTextColor = Color.FromArgb(24,28,35);
            this.TextBoxErrorPlaceholderColor = Color.FromArgb(245,80,100);
            this.TextBoxErrorTextBoxColor = Color.FromArgb(245,80,100);
            this.TextBoxErrorTextBoxBorderColor = Color.FromArgb(210,220,235);
            this.TextBoxErrorTextBoxHoverColor = Color.FromArgb(245,80,100);
        }
    }
}