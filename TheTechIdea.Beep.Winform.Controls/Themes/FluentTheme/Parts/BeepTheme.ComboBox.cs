using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class FluentTheme
    {
        private void ApplyComboBox()
        {
            this.ComboBoxBackColor = Color.FromArgb(245,246,248);
            this.ComboBoxForeColor = Color.FromArgb(32,32,32);
            this.ComboBoxBorderColor = Color.FromArgb(218,223,230);
            this.ComboBoxHoverBackColor = Color.FromArgb(245,246,248);
            this.ComboBoxHoverForeColor = Color.FromArgb(32,32,32);
            this.ComboBoxHoverBorderColor = Color.FromArgb(218,223,230);
            this.ComboBoxSelectedBackColor = Color.FromArgb(245,246,248);
            this.ComboBoxSelectedForeColor = Color.FromArgb(32,32,32);
            this.ComboBoxSelectedBorderColor = Color.FromArgb(218,223,230);
            this.ComboBoxErrorBackColor = Color.FromArgb(196,30,58);
            this.ComboBoxErrorForeColor = Color.FromArgb(32,32,32);
        }
    }
}