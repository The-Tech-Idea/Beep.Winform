using System.Drawing;
using TheTechIdea.Beep.Icons;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.CellTemplates
{
    public sealed class BeepFlightTravelCell : BeepCellTemplateBase
    {
        protected override void DrawCellContent(Graphics g, Rectangle rect)
        {
            if (Event == null) return;
            var theme = _currentTheme;
            if (theme == null) return;

            var travelStatus = Metadatum("TravelStatus", "OnTime");
            var accentColor = travelStatus switch
            {
                "Delayed" or "Cancelled" => Color.FromArgb(207, 34, 46),
                "Boarding" => Color.FromArgb(210, 153, 34),
                "Departed" or "Arrived" => Color.FromArgb(46, 160, 67),
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

            var flightNum = Metadatum("FlightNumber", Event.Title);
            var origin = Metadatum("Origin", "");
            var dest = Metadatum("Destination", "");
            var routeLine = string.IsNullOrEmpty(dest)
                ? flightNum
                : $"{flightNum}  {origin} → {dest}";
            var titleRect = new Rectangle(content.X, drawY, content.Width, lineH);
            TextRenderer.DrawText(g, routeLine, titleFont, titleRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
                TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            drawY += lineH;

            var gate = Metadatum("Gate", "");
            var baggage = Metadatum("Baggage", "");
            var infoLine = string.IsNullOrEmpty(gate)
                ? (string.IsNullOrEmpty(baggage) ? "" : $"Baggage {baggage}")
                : string.IsNullOrEmpty(baggage)
                    ? $"Gate {gate}"
                    : $"Gate {gate} · Baggage {baggage}";
            if (!string.IsNullOrEmpty(infoLine))
            {
                var infoRect = new Rectangle(content.X, drawY, content.Width, lineH);
                BeepCellTemplateHelpers.DrawIconLabel(g, infoRect, detailFont,
                    secondaryColor, SvgsUIcons.Common.Calendar, infoLine, 10);
                drawY += lineH;
            }

            if (!Event.IsAllDay && content.Y + content.Height - drawY >= lineH)
            {
                var timeText = $"{Event.StartTime:t} - {Event.EndTime:t}";
                var timeRect = new Rectangle(content.X, drawY, content.Width, lineH);
                BeepCellTemplateHelpers.DrawIconLabel(g, timeRect, detailFont,
                    secondaryColor, SvgsUIcons.Common.Clock, timeText, 10);
                drawY += lineH;
            }

            if (content.Y + content.Height - drawY >= lineH)
            {
                var badgeRect = new Rectangle(content.X, drawY, content.Width, lineH);
                var statusColor = travelStatus switch
                {
                    "OnTime" => Color.FromArgb(46, 160, 67),
                    "Delayed" => Color.FromArgb(210, 153, 34),
                    "Cancelled" => Color.FromArgb(207, 34, 46),
                    _ => Color.FromArgb(69, 133, 244)
                };
                BeepCellTemplateHelpers.DrawIconLabel(g, badgeRect, detailFont,
                    statusColor, null, travelStatus, 10);
            }

            titleFont.Dispose();
            detailFont.Dispose();
        }
    }
}
