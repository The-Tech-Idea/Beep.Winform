using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordicTheme
    {
        private void ApplyComboBox()
        {
            this.ComboBoxBackColor = Color.FromArgb(250,250,251);
            this.ComboBoxForeColor = Color.FromArgb(31,41,55);
            this.ComboBoxBorderColor = Color.FromArgb(229,231,235);
            this.ComboBoxHoverBackColor = Color.FromArgb(250,250,251);
            this.ComboBoxHoverForeColor = Color.FromArgb(31,41,55);
            this.ComboBoxHoverBorderColor = Color.FromArgb(229,231,235);
            this.ComboBoxSelectedBackColor = Color.FromArgb(250,250,251);
            this.ComboBoxSelectedForeColor = Color.FromArgb(31,41,55);
            this.ComboBoxSelectedBorderColor = Color.FromArgb(229,231,235);
            this.ComboBoxErrorBackColor = Color.FromArgb(220,38,38);
            this.ComboBoxErrorForeColor = Color.FromArgb(31,41,55);
        }
    }
}