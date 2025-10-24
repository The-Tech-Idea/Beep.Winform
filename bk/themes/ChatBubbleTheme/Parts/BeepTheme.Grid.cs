using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ChatBubbleTheme
    {
        private void ApplyGrid()
        {
            this.GridBackColor = Color.FromArgb(255,255,255);
            this.GridForeColor = Color.FromArgb(24,28,35);
            this.GridHeaderBackColor = Color.FromArgb(255,255,255);
            this.GridHeaderForeColor = Color.FromArgb(24,28,35);
            this.GridHeaderBorderColor = Color.FromArgb(210,220,235);
            this.GridHeaderHoverBackColor = Color.FromArgb(255,255,255);
            this.GridHeaderHoverForeColor = Color.FromArgb(24,28,35);
            this.GridHeaderSelectedBackColor = Color.FromArgb(255,255,255);
            this.GridHeaderSelectedForeColor = Color.FromArgb(24,28,35);
            this.GridHeaderHoverBorderColor = Color.FromArgb(210,220,235);
            this.GridHeaderSelectedBorderColor = Color.FromArgb(210,220,235);
            this.GridRowHoverBackColor = Color.FromArgb(255,255,255);
            this.GridRowHoverForeColor = Color.FromArgb(24,28,35);
            this.GridRowSelectedBackColor = Color.FromArgb(255,255,255);
            this.GridRowSelectedForeColor = Color.FromArgb(24,28,35);
            this.GridRowHoverBorderColor = Color.FromArgb(210,220,235);
            this.GridRowSelectedBorderColor = Color.FromArgb(210,220,235);
            this.GridLineColor = Color.FromArgb(222, 230, 242);
        }
    }
}