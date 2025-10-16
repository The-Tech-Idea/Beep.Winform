using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GNOMETheme
    {
        private void ApplyComboBox()
        {
            this.ComboBoxBackColor = Color.FromArgb(246,245,244);
            this.ComboBoxForeColor = Color.FromArgb(46,52,54);
            this.ComboBoxBorderColor = Color.FromArgb(205,207,212);
            this.ComboBoxHoverBackColor = Color.FromArgb(246,245,244);
            this.ComboBoxHoverForeColor = Color.FromArgb(46,52,54);
            this.ComboBoxHoverBorderColor = Color.FromArgb(205,207,212);
            this.ComboBoxSelectedBackColor = Color.FromArgb(246,245,244);
            this.ComboBoxSelectedForeColor = Color.FromArgb(46,52,54);
            this.ComboBoxSelectedBorderColor = Color.FromArgb(205,207,212);
            this.ComboBoxErrorBackColor = Color.FromArgb(224,27,36);
            this.ComboBoxErrorForeColor = Color.FromArgb(46,52,54);
        }
    }
}