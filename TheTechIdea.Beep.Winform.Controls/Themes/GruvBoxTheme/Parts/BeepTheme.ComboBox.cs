using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GruvBoxTheme
    {
        private void ApplyComboBox()
        {
            this.ComboBoxBackColor = Color.FromArgb(40,40,40);
            this.ComboBoxForeColor = Color.FromArgb(235,219,178);
            this.ComboBoxBorderColor = Color.FromArgb(168,153,132);
            this.ComboBoxHoverBackColor = Color.FromArgb(40,40,40);
            this.ComboBoxHoverForeColor = Color.FromArgb(235,219,178);
            this.ComboBoxHoverBorderColor = Color.FromArgb(168,153,132);
            this.ComboBoxSelectedBackColor = Color.FromArgb(40,40,40);
            this.ComboBoxSelectedForeColor = Color.FromArgb(235,219,178);
            this.ComboBoxSelectedBorderColor = Color.FromArgb(168,153,132);
            this.ComboBoxErrorBackColor = Color.FromArgb(40,40,40);
            this.ComboBoxErrorForeColor = Color.FromArgb(235,219,178);
        }
    }
}