using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MacOSTheme
    {
        private void ApplyComboBox()
        {
            this.ComboBoxBackColor = Color.FromArgb(250,250,252);
            this.ComboBoxForeColor = Color.FromArgb(28,28,30);
            this.ComboBoxBorderColor = Color.FromArgb(229,229,234);
            this.ComboBoxHoverBackColor = Color.FromArgb(250,250,252);
            this.ComboBoxHoverForeColor = Color.FromArgb(28,28,30);
            this.ComboBoxHoverBorderColor = Color.FromArgb(229,229,234);
            this.ComboBoxSelectedBackColor = Color.FromArgb(250,250,252);
            this.ComboBoxSelectedForeColor = Color.FromArgb(28,28,30);
            this.ComboBoxSelectedBorderColor = Color.FromArgb(229,229,234);
            this.ComboBoxErrorBackColor = Color.FromArgb(255,69,58);
            this.ComboBoxErrorForeColor = Color.FromArgb(28,28,30);
        }
    }
}