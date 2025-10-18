using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordTheme
    {
        private void ApplyComboBox()
        {
            this.ComboBoxBackColor = Color.FromArgb(46,52,64);
            this.ComboBoxForeColor = Color.FromArgb(216,222,233);
            this.ComboBoxBorderColor = Color.FromArgb(76,86,106);
            this.ComboBoxHoverBackColor = Color.FromArgb(46,52,64);
            this.ComboBoxHoverForeColor = Color.FromArgb(216,222,233);
            this.ComboBoxHoverBorderColor = Color.FromArgb(76,86,106);
            this.ComboBoxSelectedBackColor = Color.FromArgb(46,52,64);
            this.ComboBoxSelectedForeColor = Color.FromArgb(216,222,233);
            this.ComboBoxSelectedBorderColor = Color.FromArgb(76,86,106);
            this.ComboBoxErrorBackColor = Color.FromArgb(46,52,64);
            this.ComboBoxErrorForeColor = Color.FromArgb(216,222,233);
        }
    }
}