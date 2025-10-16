using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeoMorphismTheme
    {
        private void ApplyComboBox()
        {
            this.ComboBoxBackColor = Color.FromArgb(236,240,243);
            this.ComboBoxForeColor = Color.FromArgb(58,66,86);
            this.ComboBoxBorderColor = Color.FromArgb(221,228,235);
            this.ComboBoxHoverBackColor = Color.FromArgb(236,240,243);
            this.ComboBoxHoverForeColor = Color.FromArgb(58,66,86);
            this.ComboBoxHoverBorderColor = Color.FromArgb(221,228,235);
            this.ComboBoxSelectedBackColor = Color.FromArgb(236,240,243);
            this.ComboBoxSelectedForeColor = Color.FromArgb(58,66,86);
            this.ComboBoxSelectedBorderColor = Color.FromArgb(221,228,235);
            this.ComboBoxErrorBackColor = Color.FromArgb(231,76,60);
            this.ComboBoxErrorForeColor = Color.FromArgb(58,66,86);
        }
    }
}