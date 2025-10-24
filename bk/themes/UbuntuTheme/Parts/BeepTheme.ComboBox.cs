using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class UbuntuTheme
    {
        private void ApplyComboBox()
        {
            this.ComboBoxBackColor = Color.FromArgb(242,242,245);
            this.ComboBoxForeColor = Color.FromArgb(44,44,44);
            this.ComboBoxBorderColor = Color.FromArgb(218,218,222);
            this.ComboBoxHoverBackColor = Color.FromArgb(242,242,245);
            this.ComboBoxHoverForeColor = Color.FromArgb(44,44,44);
            this.ComboBoxHoverBorderColor = Color.FromArgb(218,218,222);
            this.ComboBoxSelectedBackColor = Color.FromArgb(242,242,245);
            this.ComboBoxSelectedForeColor = Color.FromArgb(44,44,44);
            this.ComboBoxSelectedBorderColor = Color.FromArgb(218,218,222);
            this.ComboBoxErrorBackColor = Color.FromArgb(192,28,40);
            this.ComboBoxErrorForeColor = Color.FromArgb(44,44,44);
        }
    }
}