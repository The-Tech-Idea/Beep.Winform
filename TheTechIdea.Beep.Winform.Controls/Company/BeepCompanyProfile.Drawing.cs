using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Company
{
    public partial class BeepCompanyProfile : BaseControl
    {
        #region Drawing
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);
            Draw(g, DrawingRect);
        }

        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            if (_currentTheme == null)
                return;

            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            Color backgroundFill = _currentTheme.CompanyPopoverBackgroundColor != Color.Empty
                ? _currentTheme.CompanyPopoverBackgroundColor
                : _currentTheme.CardBackColor;

            using (SolidBrush brush = new SolidBrush(backgroundFill))
            {
                graphics.FillRectangle(brush, rectangle);
            }

            _profilePainter.Draw(graphics, _layout);
        }
        #endregion
    }
}
