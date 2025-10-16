using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GlassTheme
    {
        private void ApplyComboBox()
        {
            this.ComboBoxBackColor = Color.FromArgb(236,244,255);
            this.ComboBoxForeColor = Color.FromArgb(17,24,39);
            this.ComboBoxBorderColor = Color.FromArgb(140, 255, 255, 255);
            this.ComboBoxHoverBackColor = Color.FromArgb(236,244,255);
            this.ComboBoxHoverForeColor = Color.FromArgb(17,24,39);
            this.ComboBoxHoverBorderColor = Color.FromArgb(140, 255, 255, 255);
            this.ComboBoxSelectedBackColor = Color.FromArgb(236,244,255);
            this.ComboBoxSelectedForeColor = Color.FromArgb(17,24,39);
            this.ComboBoxSelectedBorderColor = Color.FromArgb(140, 255, 255, 255);
            this.ComboBoxErrorBackColor = Color.FromArgb(239,68,68);
            this.ComboBoxErrorForeColor = Color.FromArgb(17,24,39);
        }
    }
}