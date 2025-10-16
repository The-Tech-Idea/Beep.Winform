using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class BrutalistTheme
    {
        private void ApplyTextBox()
        {
            this.TextBoxBackColor = Color.FromArgb(250,250,250);
            this.TextBoxForeColor = Color.FromArgb(20,20,20);
            this.TextBoxBorderColor = Color.FromArgb(0,0,0);
            this.TextBoxHoverBorderColor = Color.FromArgb(0,0,0);
            this.TextBoxHoverBackColor = Color.FromArgb(250,250,250);
            this.TextBoxHoverForeColor = Color.FromArgb(20,20,20);
            this.TextBoxSelectedBorderColor = Color.FromArgb(0,0,0);
            this.TextBoxSelectedBackColor = Color.FromArgb(250,250,250);
            this.TextBoxSelectedForeColor = Color.FromArgb(20,20,20);
            this.TextBoxPlaceholderColor = Color.FromArgb(250,250,250);
            this.TextBoxErrorBorderColor = Color.FromArgb(0,0,0);
            this.TextBoxErrorBackColor = Color.FromArgb(220,0,0);
            this.TextBoxErrorForeColor = Color.FromArgb(20,20,20);
            this.TextBoxErrorTextColor = Color.FromArgb(20,20,20);
            this.TextBoxErrorPlaceholderColor = Color.FromArgb(220,0,0);
            this.TextBoxErrorTextBoxColor = Color.FromArgb(220,0,0);
            this.TextBoxErrorTextBoxBorderColor = Color.FromArgb(0,0,0);
            this.TextBoxErrorTextBoxHoverColor = Color.FromArgb(220,0,0);
        }
    }
}