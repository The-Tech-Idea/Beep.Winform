using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class SolarizedTheme
    {
        private void ApplyTextBox()
        {
            this.TextBoxBackColor = Color.FromArgb(0,43,54);
            this.TextBoxForeColor = Color.FromArgb(147,161,161);
            this.TextBoxBorderColor = Color.FromArgb(88,110,117);
            this.TextBoxHoverBorderColor = Color.FromArgb(88,110,117);
            this.TextBoxHoverBackColor = Color.FromArgb(0,43,54);
            this.TextBoxHoverForeColor = Color.FromArgb(147,161,161);
            this.TextBoxSelectedBorderColor = Color.FromArgb(88,110,117);
            this.TextBoxSelectedBackColor = Color.FromArgb(0,43,54);
            this.TextBoxSelectedForeColor = Color.FromArgb(147,161,161);
            this.TextBoxPlaceholderColor = Color.FromArgb(0,43,54);
            this.TextBoxErrorBorderColor = Color.FromArgb(88,110,117);
            this.TextBoxErrorBackColor = Color.FromArgb(0,43,54);
            this.TextBoxErrorForeColor = Color.FromArgb(147,161,161);
            this.TextBoxErrorTextColor = Color.FromArgb(147,161,161);
            this.TextBoxErrorPlaceholderColor = Color.FromArgb(0,43,54);
            this.TextBoxErrorTextBoxColor = Color.FromArgb(0,43,54);
            this.TextBoxErrorTextBoxBorderColor = Color.FromArgb(88,110,117);
            this.TextBoxErrorTextBoxHoverColor = Color.FromArgb(0,43,54);
        }
    }
}