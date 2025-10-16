using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ChatBubbleTheme
    {
        private void ApplyCard()
        {
            this.CardTextForeColor = Color.FromArgb(24,28,35);
            this.CardBackColor = Color.FromArgb(245,248,255);
            this.CardTitleForeColor = Color.FromArgb(24,28,35);
            this.CardSubTitleForeColor = Color.FromArgb(24,28,35);
            this.CardrGradiantStartColor = Color.FromArgb(245,248,255);
            this.CardGradiantEndColor = Color.FromArgb(245,248,255);
            this.CardGradiantMiddleColor = Color.FromArgb(245,248,255);
            this.CardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}