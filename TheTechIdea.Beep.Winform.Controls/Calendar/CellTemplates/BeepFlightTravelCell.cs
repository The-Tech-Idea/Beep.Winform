using System.Drawing;
using TheTechIdea.Beep.Icons;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.CellTemplates
{
    public sealed class BeepFlightTravelCell : BeepCellTemplateBase
    {
        protected override void DrawCellContent(Graphics g, Rectangle rect)
        {
            if (Event == null) return;
            if (_currentTheme == null) return;

            var travelStatus = Metadatum("TravelStatus", "OnTime");
            var accentColor = travelStatus switch
            {
                "Delayed" or "Cancelled" => BeepCellTemplateHelpers.PriorityColor("critical"),
                "Boarding" => BeepCellTemplateHelpers.PriorityColor("high"),
                "Departed" or "Arrived" => BeepCellTemplateHelpers.StatusColor(CalendarEventStatus.Confirmed),
                _ => BeepCellTemplateHelpers.PriorityColor("medium")
            };

            var content = CellContentRect(rect);
            BeepCellTemplateHelpers.DrawAccentBadge(g, rect, accentColor, CellAccentWidth);

            var drawY = content.Y;

            var flightNum = Metadatum("FlightNumber", Event.Title);
            var origin = Metadatum("Origin", "");
            var dest = Metadatum("Destination", "");
            var routeLine = string.IsNullOrEmpty(dest)
                ? flightNum
                : $"{flightNum}  {origin} → {dest}";
            var titleRect = new Rectangle(content.X, drawY, content.Width, CellLineHeight);
            TextRenderer.DrawText(g, routeLine, TitleFont, titleRect, TextColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
                TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            drawY += CellLineHeight;

            var gate = Metadatum("Gate", "");
            var baggage = Metadatum("Baggage", "");
            var infoLine = string.IsNullOrEmpty(gate)
                ? (string.IsNullOrEmpty(baggage) ? "" : $"Baggage {baggage}")
                : string.IsNullOrEmpty(baggage)
                    ? $"Gate {gate}"
                    : $"Gate {gate} · Baggage {baggage}";
            if (!string.IsNullOrEmpty(infoLine))
            {
                var infoRect = new Rectangle(content.X, drawY, content.Width, CellLineHeight);
                BeepCellTemplateHelpers.DrawIconLabel(g, infoRect, DetailFont,
                    SecondaryTextColor, SvgsUIcons.Common.Calendar, infoLine, 10);
                drawY += CellLineHeight;
            }

            if (!Event.IsAllDay && content.Y + content.Height - drawY >= CellLineHeight)
            {
                var timeText = $"{Event.StartTime:t} - {Event.EndTime:t}";
                var timeRect = new Rectangle(content.X, drawY, content.Width, CellLineHeight);
                BeepCellTemplateHelpers.DrawIconLabel(g, timeRect, DetailFont,
                    SecondaryTextColor, SvgsUIcons.Common.Clock, timeText, 10);
                drawY += CellLineHeight;
            }

            if (content.Y + content.Height - drawY >= CellLineHeight)
            {
                var badgeRect = new Rectangle(content.X, drawY, content.Width, CellLineHeight);
                var statusColor = travelStatus switch
                {
                    "OnTime" => BeepCellTemplateHelpers.StatusColor(CalendarEventStatus.Confirmed),
                    "Delayed" => BeepCellTemplateHelpers.PriorityColor("high"),
                    "Cancelled" => BeepCellTemplateHelpers.PriorityColor("critical"),
                    _ => BeepCellTemplateHelpers.PriorityColor("medium")
                };
                BeepCellTemplateHelpers.DrawIconLabel(g, badgeRect, DetailFont,
                    statusColor, null, travelStatus, 10);
            }
        }
    }
}
