using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class DraculaTheme
    {
        private void ApplyComboBox()
        {
            this.ComboBoxBackColor = Color.FromArgb(40,42,54);
            this.ComboBoxForeColor = Color.FromArgb(248,248,242);
            this.ComboBoxBorderColor = Color.FromArgb(98,114,164);
            this.ComboBoxHoverBackColor = Color.FromArgb(40,42,54);
            this.ComboBoxHoverForeColor = Color.FromArgb(248,248,242);
            this.ComboBoxHoverBorderColor = Color.FromArgb(98,114,164);
            this.ComboBoxSelectedBackColor = Color.FromArgb(40,42,54);
            this.ComboBoxSelectedForeColor = Color.FromArgb(248,248,242);
            this.ComboBoxSelectedBorderColor = Color.FromArgb(98,114,164);
            this.ComboBoxErrorBackColor = Color.FromArgb(40,42,54);
            this.ComboBoxErrorForeColor = Color.FromArgb(248,248,242);
        }
    }
}