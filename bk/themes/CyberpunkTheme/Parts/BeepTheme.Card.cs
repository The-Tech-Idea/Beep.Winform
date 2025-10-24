using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CyberpunkTheme
    {
        private void ApplyCard()
        {
            this.CardTextForeColor = Color.FromArgb(228,244,255);
            this.CardBackColor = Color.FromArgb(10,8,20);
            this.CardTitleForeColor = Color.FromArgb(228,244,255);
            this.CardSubTitleForeColor = Color.FromArgb(228,244,255);
            this.CardrGradiantStartColor = Color.FromArgb(10,8,20);
            this.CardGradiantEndColor = Color.FromArgb(10,8,20);
            this.CardGradiantMiddleColor = Color.FromArgb(10,8,20);
            this.CardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
        }
    }
}