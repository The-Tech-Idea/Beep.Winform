using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordTheme
    {
        private void ApplyTextBox()
        {
            this.TextBoxBackColor = Color.FromArgb(46,52,64);
            this.TextBoxForeColor = Color.FromArgb(216,222,233);
            this.TextBoxBorderColor = Color.FromArgb(76,86,106);
            this.TextBoxHoverBorderColor = Color.FromArgb(76,86,106);
            this.TextBoxHoverBackColor = Color.FromArgb(46,52,64);
            this.TextBoxHoverForeColor = Color.FromArgb(216,222,233);
            this.TextBoxSelectedBorderColor = Color.FromArgb(76,86,106);
            this.TextBoxSelectedBackColor = Color.FromArgb(46,52,64);
            this.TextBoxSelectedForeColor = Color.FromArgb(216,222,233);
            this.TextBoxPlaceholderColor = Color.FromArgb(46,52,64);
            this.TextBoxErrorBorderColor = Color.FromArgb(76,86,106);
            this.TextBoxErrorBackColor = Color.FromArgb(46,52,64);
            this.TextBoxErrorForeColor = Color.FromArgb(216,222,233);
            this.TextBoxErrorTextColor = Color.FromArgb(216,222,233);
            this.TextBoxErrorPlaceholderColor = Color.FromArgb(46,52,64);
            this.TextBoxErrorTextBoxColor = Color.FromArgb(46,52,64);
            this.TextBoxErrorTextBoxBorderColor = Color.FromArgb(76,86,106);
            this.TextBoxErrorTextBoxHoverColor = Color.FromArgb(46,52,64);
        }
    }
}