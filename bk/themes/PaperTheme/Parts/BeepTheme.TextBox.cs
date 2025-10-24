using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class PaperTheme
    {
        private void ApplyTextBox()
        {
            this.TextBoxBackColor = Color.FromArgb(250,250,250);
            this.TextBoxForeColor = Color.FromArgb(33,33,33);
            this.TextBoxBorderColor = Color.FromArgb(224,224,224);
            this.TextBoxHoverBorderColor = Color.FromArgb(224,224,224);
            this.TextBoxHoverBackColor = Color.FromArgb(250,250,250);
            this.TextBoxHoverForeColor = Color.FromArgb(33,33,33);
            this.TextBoxSelectedBorderColor = Color.FromArgb(224,224,224);
            this.TextBoxSelectedBackColor = Color.FromArgb(250,250,250);
            this.TextBoxSelectedForeColor = Color.FromArgb(33,33,33);
            this.TextBoxPlaceholderColor = Color.FromArgb(250,250,250);
            this.TextBoxErrorBorderColor = Color.FromArgb(224,224,224);
            this.TextBoxErrorBackColor = Color.FromArgb(250,250,250);
            this.TextBoxErrorForeColor = Color.FromArgb(33,33,33);
            this.TextBoxErrorTextColor = Color.FromArgb(33,33,33);
            this.TextBoxErrorPlaceholderColor = Color.FromArgb(250,250,250);
            this.TextBoxErrorTextBoxColor = Color.FromArgb(250,250,250);
            this.TextBoxErrorTextBoxBorderColor = Color.FromArgb(224,224,224);
            this.TextBoxErrorTextBoxHoverColor = Color.FromArgb(250,250,250);
        }
    }
}