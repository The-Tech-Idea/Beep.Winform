using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class HolographicTheme
    {
        private void ApplyComboBox()
        {
            this.ComboBoxBackColor = Color.FromArgb(15,16,32);
            this.ComboBoxForeColor = Color.FromArgb(245,247,255);
            this.ComboBoxBorderColor = Color.FromArgb(74,79,123);
            this.ComboBoxHoverBackColor = Color.FromArgb(15,16,32);
            this.ComboBoxHoverForeColor = Color.FromArgb(245,247,255);
            this.ComboBoxHoverBorderColor = Color.FromArgb(74,79,123);
            this.ComboBoxSelectedBackColor = Color.FromArgb(15,16,32);
            this.ComboBoxSelectedForeColor = Color.FromArgb(245,247,255);
            this.ComboBoxSelectedBorderColor = Color.FromArgb(74,79,123);
            this.ComboBoxErrorBackColor = Color.FromArgb(15,16,32);
            this.ComboBoxErrorForeColor = Color.FromArgb(245,247,255);
        }
    }
}