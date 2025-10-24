using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class KDETheme
    {
        private void ApplyTextBox()
        {
            this.TextBoxBackColor = Color.FromArgb(248,249,250);
            this.TextBoxForeColor = Color.FromArgb(33,37,41);
            this.TextBoxBorderColor = Color.FromArgb(222,226,230);
            this.TextBoxHoverBorderColor = Color.FromArgb(222,226,230);
            this.TextBoxHoverBackColor = Color.FromArgb(248,249,250);
            this.TextBoxHoverForeColor = Color.FromArgb(33,37,41);
            this.TextBoxSelectedBorderColor = Color.FromArgb(222,226,230);
            this.TextBoxSelectedBackColor = Color.FromArgb(248,249,250);
            this.TextBoxSelectedForeColor = Color.FromArgb(33,37,41);
            this.TextBoxPlaceholderColor = Color.FromArgb(248,249,250);
            this.TextBoxErrorBorderColor = Color.FromArgb(222,226,230);
            this.TextBoxErrorBackColor = Color.FromArgb(220,53,69);
            this.TextBoxErrorForeColor = Color.FromArgb(33,37,41);
            this.TextBoxErrorTextColor = Color.FromArgb(33,37,41);
            this.TextBoxErrorPlaceholderColor = Color.FromArgb(220,53,69);
            this.TextBoxErrorTextBoxColor = Color.FromArgb(220,53,69);
            this.TextBoxErrorTextBoxBorderColor = Color.FromArgb(222,226,230);
            this.TextBoxErrorTextBoxHoverColor = Color.FromArgb(220,53,69);
        }
    }
}