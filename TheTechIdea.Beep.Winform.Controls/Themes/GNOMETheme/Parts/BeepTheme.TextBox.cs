using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GNOMETheme
    {
        private void ApplyTextBox()
        {
            this.TextBoxBackColor = Color.FromArgb(246,245,244);
            this.TextBoxForeColor = Color.FromArgb(46,52,54);
            this.TextBoxBorderColor = Color.FromArgb(205,207,212);
            this.TextBoxHoverBorderColor = Color.FromArgb(205,207,212);
            this.TextBoxHoverBackColor = Color.FromArgb(246,245,244);
            this.TextBoxHoverForeColor = Color.FromArgb(46,52,54);
            this.TextBoxSelectedBorderColor = Color.FromArgb(205,207,212);
            this.TextBoxSelectedBackColor = Color.FromArgb(246,245,244);
            this.TextBoxSelectedForeColor = Color.FromArgb(46,52,54);
            this.TextBoxPlaceholderColor = Color.FromArgb(246,245,244);
            this.TextBoxErrorBorderColor = Color.FromArgb(205,207,212);
            this.TextBoxErrorBackColor = Color.FromArgb(224,27,36);
            this.TextBoxErrorForeColor = Color.FromArgb(46,52,54);
            this.TextBoxErrorTextColor = Color.FromArgb(46,52,54);
            this.TextBoxErrorPlaceholderColor = Color.FromArgb(224,27,36);
            this.TextBoxErrorTextBoxColor = Color.FromArgb(224,27,36);
            this.TextBoxErrorTextBoxBorderColor = Color.FromArgb(205,207,212);
            this.TextBoxErrorTextBoxHoverColor = Color.FromArgb(224,27,36);
        }
    }
}