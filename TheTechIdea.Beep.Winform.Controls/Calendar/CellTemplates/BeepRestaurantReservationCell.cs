using System.Drawing;
using TheTechIdea.Beep.Icons;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.CellTemplates
{
    public sealed class BeepRestaurantReservationCell : BeepCellTemplateBase
    {
        protected override void DrawCellContent(Graphics g, Rectangle rect)
        {
            if (Event == null) return;
            if (_currentTheme == null) return;

            var reservationStatus = Metadatum("ReservationStatus", "Booked");
            var accentColor = reservationStatus switch
            {
                "Seated" => BeepCellTemplateHelpers.PriorityColor("high"),
                "Completed" => BeepCellTemplateHelpers.StatusColor(CalendarEventStatus.Confirmed),
                "Cancelled" => BeepCellTemplateHelpers.PriorityColor("critical"),
                _ => BeepCellTemplateHelpers.PriorityColor("medium")
            };

            var content = CellContentRect(rect);
            BeepCellTemplateHelpers.DrawAccentBadge(g, rect, accentColor, CellAccentWidth);

            var drawY = content.Y;

            var guestName = Metadatum("GuestName", "Guest");
            var partySize = Metadatum("PartySize", "");
            var titleLine = string.IsNullOrEmpty(partySize)
                ? guestName
                : $"{guestName} · Party of {partySize}";
            var titleRect = new Rectangle(content.X, drawY, content.Width, CellLineHeight);
            BeepCellTemplateHelpers.DrawIconLabel(g, titleRect, TitleFont,
                TextColor, SvgsUIcons.Common.Calendar, titleLine, 12);
            drawY += CellLineHeight;

            var tableNumber = Metadatum("TableNumber", "");
            var specialRequest = Metadatum("SpecialRequests", "");
            var tableLine = string.IsNullOrEmpty(tableNumber)
                ? specialRequest
                : string.IsNullOrEmpty(specialRequest)
                    ? $"Table {tableNumber}"
                    : $"Table {tableNumber} · {specialRequest}";
            if (!string.IsNullOrEmpty(tableLine))
            {
                var tableRect = new Rectangle(content.X, drawY, content.Width, CellLineHeight);
                BeepCellTemplateHelpers.DrawIconLabel(g, tableRect, DetailFont,
                    SecondaryTextColor, SvgsUIcons.Common.Calendar, tableLine, 10);
                drawY += CellLineHeight;
            }

            if (!Event.IsAllDay && content.Y + content.Height - drawY >= CellLineHeight)
            {
                var timeText = $"{Event.StartTime:t} - {Event.EndTime:t}";
                var timeRect = new Rectangle(content.X, drawY, content.Width, CellLineHeight);
                BeepCellTemplateHelpers.DrawIconLabel(g, timeRect, DetailFont,
                    SecondaryTextColor, SvgsUIcons.Common.Clock, timeText, 10);
            }

            var dotColor = reservationStatus switch
            {
                "Seated" => BeepCellTemplateHelpers.PriorityColor("high"),
                "Completed" => BeepCellTemplateHelpers.StatusColor(CalendarEventStatus.Confirmed),
                "Cancelled" => BeepCellTemplateHelpers.PriorityColor("critical"),
                _ => BeepCellTemplateHelpers.StatusColor(CalendarEventStatus.Confirmed)
            };
            BeepCellTemplateHelpers.DrawStatusDot(g, rect.Right - 16,
                rect.Y + rect.Height / 2, dotColor, 5);
        }
    }
}
