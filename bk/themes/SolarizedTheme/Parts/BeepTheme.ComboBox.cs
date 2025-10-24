using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class SolarizedTheme
    {
        private void ApplyComboBox()
        {
            this.ComboBoxBackColor = Color.FromArgb(0,43,54);
            this.ComboBoxForeColor = Color.FromArgb(147,161,161);
            this.ComboBoxBorderColor = Color.FromArgb(88,110,117);
            this.ComboBoxHoverBackColor = Color.FromArgb(0,43,54);
            this.ComboBoxHoverForeColor = Color.FromArgb(147,161,161);
            this.ComboBoxHoverBorderColor = Color.FromArgb(88,110,117);
            this.ComboBoxSelectedBackColor = Color.FromArgb(0,43,54);
            this.ComboBoxSelectedForeColor = Color.FromArgb(147,161,161);
            this.ComboBoxSelectedBorderColor = Color.FromArgb(88,110,117);
            this.ComboBoxErrorBackColor = Color.FromArgb(0,43,54);
            this.ComboBoxErrorForeColor = Color.FromArgb(147,161,161);
        }
    }
}