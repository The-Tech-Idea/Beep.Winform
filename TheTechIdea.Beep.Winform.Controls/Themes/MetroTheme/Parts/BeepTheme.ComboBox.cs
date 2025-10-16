using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MetroTheme
    {
        private void ApplyComboBox()
        {
            this.ComboBoxBackColor = Color.FromArgb(243,242,241);
            this.ComboBoxForeColor = Color.FromArgb(32,31,30);
            this.ComboBoxBorderColor = Color.FromArgb(225,225,225);
            this.ComboBoxHoverBackColor = Color.FromArgb(243,242,241);
            this.ComboBoxHoverForeColor = Color.FromArgb(32,31,30);
            this.ComboBoxHoverBorderColor = Color.FromArgb(225,225,225);
            this.ComboBoxSelectedBackColor = Color.FromArgb(243,242,241);
            this.ComboBoxSelectedForeColor = Color.FromArgb(32,31,30);
            this.ComboBoxSelectedBorderColor = Color.FromArgb(225,225,225);
            this.ComboBoxErrorBackColor = Color.FromArgb(196,30,58);
            this.ComboBoxErrorForeColor = Color.FromArgb(32,31,30);
        }
    }
}