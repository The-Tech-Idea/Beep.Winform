using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class TokyoTheme
    {
        private void ApplyComboBox()
        {
            this.ComboBoxBackColor = Color.FromArgb(26,27,38);
            this.ComboBoxForeColor = Color.FromArgb(192,202,245);
            this.ComboBoxBorderColor = Color.FromArgb(86,95,137);
            this.ComboBoxHoverBackColor = Color.FromArgb(26,27,38);
            this.ComboBoxHoverForeColor = Color.FromArgb(192,202,245);
            this.ComboBoxHoverBorderColor = Color.FromArgb(86,95,137);
            this.ComboBoxSelectedBackColor = Color.FromArgb(26,27,38);
            this.ComboBoxSelectedForeColor = Color.FromArgb(192,202,245);
            this.ComboBoxSelectedBorderColor = Color.FromArgb(86,95,137);
            this.ComboBoxErrorBackColor = Color.FromArgb(26,27,38);
            this.ComboBoxErrorForeColor = Color.FromArgb(192,202,245);
        }
    }
}