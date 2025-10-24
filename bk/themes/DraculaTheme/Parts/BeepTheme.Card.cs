using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class DraculaTheme
    {
        private void ApplyCard()
        {
            this.CardTextForeColor = Color.FromArgb(248,248,242);
            this.CardBackColor = Color.FromArgb(40,42,54);
            this.CardTitleForeColor = Color.FromArgb(248,248,242);
            this.CardSubTitleForeColor = Color.FromArgb(248,248,242);
            this.CardrGradiantStartColor = Color.FromArgb(40,42,54);
            this.CardGradiantEndColor = Color.FromArgb(40,42,54);
            this.CardGradiantMiddleColor = Color.FromArgb(40,42,54);
            this.CardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
        }
    }
}