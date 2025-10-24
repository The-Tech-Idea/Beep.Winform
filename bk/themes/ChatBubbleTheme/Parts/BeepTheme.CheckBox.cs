using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ChatBubbleTheme
    {
        private void ApplyCheckBox()
        {
            this.CheckBoxBackColor = Color.FromArgb(245,248,255);
            this.CheckBoxForeColor = Color.FromArgb(24,28,35);
            this.CheckBoxBorderColor = Color.FromArgb(210,220,235);
            this.CheckBoxCheckedBackColor = Color.FromArgb(245,248,255);
            this.CheckBoxCheckedForeColor = Color.FromArgb(24,28,35);
            this.CheckBoxCheckedBorderColor = Color.FromArgb(210,220,235);
            this.CheckBoxHoverBackColor = Color.FromArgb(245,248,255);
            this.CheckBoxHoverForeColor = Color.FromArgb(24,28,35);
            this.CheckBoxHoverBorderColor = Color.FromArgb(210,220,235);
        }
    }
}