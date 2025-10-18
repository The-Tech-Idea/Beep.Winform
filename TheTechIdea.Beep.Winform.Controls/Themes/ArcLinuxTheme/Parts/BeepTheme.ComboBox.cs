using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ArcLinuxTheme
    {
        private void ApplyComboBox()
        {
            this.ComboBoxBackColor = Color.FromArgb(245,246,247);
            this.ComboBoxForeColor = Color.FromArgb(43,45,48);
            this.ComboBoxBorderColor = Color.FromArgb(220,223,230);
            this.ComboBoxHoverBackColor = Color.FromArgb(245,246,247);
            this.ComboBoxHoverForeColor = Color.FromArgb(43,45,48);
            this.ComboBoxHoverBorderColor = Color.FromArgb(220,223,230);
            this.ComboBoxSelectedBackColor = Color.FromArgb(245,246,247);
            this.ComboBoxSelectedForeColor = Color.FromArgb(43,45,48);
            this.ComboBoxSelectedBorderColor = Color.FromArgb(220,223,230);
            this.ComboBoxErrorBackColor = Color.FromArgb(244,67,54);
            this.ComboBoxErrorForeColor = Color.FromArgb(43,45,48);
        }
    }
}