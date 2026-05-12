using System;
using System.Drawing;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private void DrawPainterHeaderText(Graphics g, CalendarPainterContext ctx, string headerText, Rectangle textRect)
        {
            if (textRect.Width <= 0 || textRect.Height <= 0) return;

            using (var brush = new SolidBrush(ctx.ForegroundColor))
            using (var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter
            })
            {
                g.DrawString(headerText, HeaderFont, brush, textRect, sf);
            }
        }

        private Rectangle GetHeaderTextBounds()
        {
            var headerRect = _rects.HeaderRect;
            if (headerRect.Width <= 0 || headerRect.Height <= 0)
            {
                return Rectangle.Empty;
            }

            int navRightAnchor = new[]
            {
                _nextButton?.Right ?? headerRect.X,
                _todayButton?.Right ?? headerRect.X,
                _undoButton?.Right ?? headerRect.X,
                _redoButton?.Right ?? headerRect.X
            }.Max();
            int leftFromNav = navRightAnchor + CalendarLayoutMetrics.HeaderTextSpacingFromNav;
            int leftFromPadding = headerRect.X + Math.Max(0, HeaderLeftPadding);
            int leftFromGrid = Math.Max(headerRect.X, _rects.CalendarGridRect.Left);
            int availableLeft = Math.Max(Math.Max(leftFromNav, leftFromPadding), leftFromGrid);

            int rightFromPadding = headerRect.Right - Math.Max(0, HeaderRightPadding);
            int rightFromGrid = _rects.CalendarGridRect.Width > 0
                ? Math.Min(headerRect.Right, _rects.CalendarGridRect.Right)
                : headerRect.Right;
            int availableRight = Math.Min(rightFromPadding, rightFromGrid);

            if (availableRight <= availableLeft)
            {
                availableLeft = headerRect.X + Math.Max(0, HeaderLeftPadding);
                availableRight = headerRect.Right - Math.Max(0, HeaderRightPadding);
                if (availableRight <= availableLeft)
                {
                    availableLeft = headerRect.X;
                    availableRight = headerRect.Right;
                }
            }

            return new Rectangle(
                availableLeft,
                headerRect.Y,
                Math.Max(1, availableRight - availableLeft),
                headerRect.Height);
        }
    }
}