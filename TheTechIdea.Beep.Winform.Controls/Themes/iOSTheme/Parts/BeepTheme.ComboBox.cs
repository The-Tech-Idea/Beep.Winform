using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class iOSTheme
    {
        private void ApplyComboBox()
        {
            this.ComboBoxBackColor = Color.FromArgb(242,242,247);
            this.ComboBoxForeColor = Color.FromArgb(28,28,30);
            this.ComboBoxBorderColor = Color.FromArgb(198,198,207);
            this.ComboBoxHoverBackColor = Color.FromArgb(242,242,247);
            this.ComboBoxHoverForeColor = Color.FromArgb(28,28,30);
            this.ComboBoxHoverBorderColor = Color.FromArgb(198,198,207);
            this.ComboBoxSelectedBackColor = Color.FromArgb(242,242,247);
            this.ComboBoxSelectedForeColor = Color.FromArgb(28,28,30);
            this.ComboBoxSelectedBorderColor = Color.FromArgb(198,198,207);
            this.ComboBoxErrorBackColor = Color.FromArgb(242,242,247);
            this.ComboBoxErrorForeColor = Color.FromArgb(28,28,30);
        }
    }
}