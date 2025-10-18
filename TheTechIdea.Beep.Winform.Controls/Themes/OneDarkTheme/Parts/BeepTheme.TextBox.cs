using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class OneDarkTheme
    {
        private void ApplyTextBox()
        {
            this.TextBoxBackColor = Color.FromArgb(40,44,52);
            this.TextBoxForeColor = Color.FromArgb(171,178,191);
            this.TextBoxBorderColor = Color.FromArgb(92,99,112);
            this.TextBoxHoverBorderColor = Color.FromArgb(92,99,112);
            this.TextBoxHoverBackColor = Color.FromArgb(40,44,52);
            this.TextBoxHoverForeColor = Color.FromArgb(171,178,191);
            this.TextBoxSelectedBorderColor = Color.FromArgb(92,99,112);
            this.TextBoxSelectedBackColor = Color.FromArgb(40,44,52);
            this.TextBoxSelectedForeColor = Color.FromArgb(171,178,191);
            this.TextBoxPlaceholderColor = Color.FromArgb(40,44,52);
            this.TextBoxErrorBorderColor = Color.FromArgb(92,99,112);
            this.TextBoxErrorBackColor = Color.FromArgb(40,44,52);
            this.TextBoxErrorForeColor = Color.FromArgb(171,178,191);
            this.TextBoxErrorTextColor = Color.FromArgb(171,178,191);
            this.TextBoxErrorPlaceholderColor = Color.FromArgb(40,44,52);
            this.TextBoxErrorTextBoxColor = Color.FromArgb(40,44,52);
            this.TextBoxErrorTextBoxBorderColor = Color.FromArgb(92,99,112);
            this.TextBoxErrorTextBoxHoverColor = Color.FromArgb(40,44,52);
        }
    }
}