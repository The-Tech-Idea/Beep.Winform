using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CartoonTheme
    {
        private void ApplyTextBox()
        {
            this.TextBoxBackColor = Color.FromArgb(255,251,235);
            this.TextBoxForeColor = Color.FromArgb(33,37,41);
            this.TextBoxBorderColor = Color.FromArgb(247,208,136);
            this.TextBoxHoverBorderColor = Color.FromArgb(247,208,136);
            this.TextBoxHoverBackColor = Color.FromArgb(255,251,235);
            this.TextBoxHoverForeColor = Color.FromArgb(33,37,41);
            this.TextBoxSelectedBorderColor = Color.FromArgb(247,208,136);
            this.TextBoxSelectedBackColor = Color.FromArgb(255,251,235);
            this.TextBoxSelectedForeColor = Color.FromArgb(33,37,41);
            this.TextBoxPlaceholderColor = Color.FromArgb(255,251,235);
            this.TextBoxErrorBorderColor = Color.FromArgb(247,208,136);
            this.TextBoxErrorBackColor = Color.FromArgb(255,82,82);
            this.TextBoxErrorForeColor = Color.FromArgb(33,37,41);
            this.TextBoxErrorTextColor = Color.FromArgb(33,37,41);
            this.TextBoxErrorPlaceholderColor = Color.FromArgb(255,82,82);
            this.TextBoxErrorTextBoxColor = Color.FromArgb(255,82,82);
            this.TextBoxErrorTextBoxBorderColor = Color.FromArgb(247,208,136);
            this.TextBoxErrorTextBoxHoverColor = Color.FromArgb(255,82,82);
        }
    }
}