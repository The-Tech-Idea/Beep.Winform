using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class Metro2Theme
    {
        private void ApplyComboBox()
        {
            this.ComboBoxBackColor = Color.FromArgb(243,242,241);
            this.ComboBoxForeColor = Color.FromArgb(32,31,30);
            this.ComboBoxBorderColor = Color.FromArgb(220,220,220);
            this.ComboBoxHoverBackColor = Color.FromArgb(243,242,241);
            this.ComboBoxHoverForeColor = Color.FromArgb(32,31,30);
            this.ComboBoxHoverBorderColor = Color.FromArgb(220,220,220);
            this.ComboBoxSelectedBackColor = Color.FromArgb(243,242,241);
            this.ComboBoxSelectedForeColor = Color.FromArgb(32,31,30);
            this.ComboBoxSelectedBorderColor = Color.FromArgb(220,220,220);
            this.ComboBoxErrorBackColor = Color.FromArgb(232,17,35);
            this.ComboBoxErrorForeColor = Color.FromArgb(32,31,30);
        }
    }
}