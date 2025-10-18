using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class OneDarkTheme
    {
        private void ApplyComboBox()
        {
            this.ComboBoxBackColor = Color.FromArgb(40,44,52);
            this.ComboBoxForeColor = Color.FromArgb(171,178,191);
            this.ComboBoxBorderColor = Color.FromArgb(92,99,112);
            this.ComboBoxHoverBackColor = Color.FromArgb(40,44,52);
            this.ComboBoxHoverForeColor = Color.FromArgb(171,178,191);
            this.ComboBoxHoverBorderColor = Color.FromArgb(92,99,112);
            this.ComboBoxSelectedBackColor = Color.FromArgb(40,44,52);
            this.ComboBoxSelectedForeColor = Color.FromArgb(171,178,191);
            this.ComboBoxSelectedBorderColor = Color.FromArgb(92,99,112);
            this.ComboBoxErrorBackColor = Color.FromArgb(40,44,52);
            this.ComboBoxErrorForeColor = Color.FromArgb(171,178,191);
        }
    }
}