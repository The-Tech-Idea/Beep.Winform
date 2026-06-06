using System.Drawing;
using TheTechIdea.Beep.Icons;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.CellTemplates
{
    public sealed class BeepRestaurantReservationCell : BeepCellTemplateBase
    {
        protected override void DrawCellContent(Graphics g, Rectangle rect)
        {
            if (Event == null) return;
            var theme = _currentTheme;
            if (theme == null) return;

            var reservationStatus = Metadatum("ReservationStatus", "Booked");
            var accentColor = reservationStatus switch
            {
                "Seated" => Color.FromArgb(210, 153, 34),
                "Completed" => Color.FromArgb(46, 160, 67),
                "Cancelled" => Color.FromArgb(207, 34, 46),
                _ => Color.FromArgb(69, 133, 244)
            };

            int pad = 6;
            int accentWidth = 4;
            var content = new Rectangle(rect.X + pad + accentWidth, rect.Y + pad,
                rect.Width - pad * 2 - accentWidth, rect.Height - pad * 2);

            BeepCellTemplateHelpers.DrawAccentBadge(g, rect, accentColor, accentWidth);

            var titleFont = new Font(Font?.FontFamily ?? FontFamily.GenericSansSerif, 9f, FontStyle.Regular);
            var detailFont = new Font(Font?.FontFamily ?? FontFamily.GenericSansSerif, 7.5f, FontStyle.Regular);
            var textColor = theme.PrimaryTextColor;
            var secondaryColor = Color.FromArgb(180, textColor);

            int lineH = 16;
            var drawY = content.Y;

            var guestName = Metadatum("GuestName", "Guest");
            var partySize = Metadatum("PartySize", "");
            var titleLine = string.IsNullOrEmpty(partySize)
                ? guestName
                : $"{guestName} · Party of {partySize}";
            var titleRect = new Rectangle(content.X, drawY, content.Width, lineH);
            BeepCellTemplateHelpers.DrawIconLabel(g, titleRect, titleFont,
                textColor, SvgsUIcons.Common.Calendar, titleLine, 12);
            drawY += lineH;

            var tableNumber = Metadatum("TableNumber", "");
            var specialRequest = Metadatum("SpecialRequests", "");
            var tableLine = string.IsNullOrEmpty(tableNumber)
                ? specialRequest
                : string.IsNullOrEmpty(specialRequest)
                    ? $"Table {tableNumber}"
                    : $"Table {tableNumber} · {specialRequest}";
            if (!string.IsNullOrEmpty(tableLine))
            {
                var tableRect = new Rectangle(content.X, drawY, content.Width, lineH);
                BeepCellTemplateHelpers.DrawIconLabel(g, tableRect, detailFont,
                    secondaryColor, SvgsUIcons.Common.Calendar, tableLine, 10);
                drawY += lineH;
            }

            if (!Event.IsAllDay && content.Y + content.Height - drawY >= lineH)
            {
                var timeText = $"{Event.StartTime:t} - {Event.EndTime:t}";
                var timeRect = new Rectangle(content.X, drawY, content.Width, lineH);
                BeepCellTemplateHelpers.DrawIconLabel(g, timeRect, detailFont,
                    secondaryColor, SvgsUIcons.Common.Clock, timeText, 10);
            }

            var dotColor = reservationStatus switch
            {
                "Seated" => Color.FromArgb(210, 153, 34),
                "Completed" => Color.FromArgb(46, 160, 67),
                "Cancelled" => Color.FromArgb(207, 34, 46),
                _ => Color.FromArgb(46, 160, 67)
            };
            BeepCellTemplateHelpers.DrawStatusDot(g, rect.Right - 16,
                rect.Y + rect.Height / 2, dotColor, 5);

            titleFont.Dispose();
            detailFont.Dispose();
        }
    }
}
