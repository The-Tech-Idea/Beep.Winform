using System.Drawing;
using TheTechIdea.Beep.Icons;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.CellTemplates
{
    public sealed class BeepServiceCallCell : BeepCellTemplateBase
    {
        protected override void DrawCellContent(Graphics g, Rectangle rect)
        {
            if (Event == null) return;
            if (_currentTheme == null) return;

            var priority = Metadatum("Priority", "Medium");
            var accentColor = BeepCellTemplateHelpers.PriorityColor(priority);

            var content = CellContentRect(rect);
            BeepCellTemplateHelpers.DrawAccentBadge(g, rect, accentColor, CellAccentWidth);

            var drawY = content.Y;

            var ticketNumber = Metadatum("TicketNumber", "");
            var titleLine = string.IsNullOrEmpty(ticketNumber)
                ? Event.Title
                : $"{Event.Title} · #{ticketNumber}";
            var titleRect = new Rectangle(content.X, drawY, content.Width, CellLineHeight);
            TextRenderer.DrawText(g, titleLine, TitleFont, titleRect, TextColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
                TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            drawY += CellLineHeight;

            var techName = Metadatum("TechnicianName", "");
            var eta = Metadatum("ETAMinutes", "");
            var techLine = string.IsNullOrEmpty(techName)
                ? (string.IsNullOrEmpty(eta) ? "" : $"ETA {eta}m")
                : string.IsNullOrEmpty(eta)
                    ? techName
                    : $"{techName} · ETA {eta}m";
            if (!string.IsNullOrEmpty(techLine))
            {
                var techRect = new Rectangle(content.X, drawY, content.Width, CellLineHeight);
                BeepCellTemplateHelpers.DrawIconLabel(g, techRect, DetailFont,
                    SecondaryTextColor, SvgsUIcons.Common.Calendar, techLine, 10);
                drawY += CellLineHeight;
            }

            var equipment = Metadatum("EquipmentType", "");
            var hasSpace = content.Y + content.Height - drawY >= CellLineHeight;
            if (!string.IsNullOrEmpty(equipment) && hasSpace)
            {
                var equipRect = new Rectangle(content.X, drawY, content.Width, CellLineHeight);
                BeepCellTemplateHelpers.DrawIconLabel(g, equipRect, DetailFont,
                    SecondaryTextColor, SvgsUIcons.Common.Edit, equipment, 10);
                drawY += CellLineHeight;
            }

            if (hasSpace && !Event.IsAllDay)
            {
                var timeText = $"{Event.StartTime:t} - {Event.EndTime:t}";
                var timeRect = new Rectangle(content.X, drawY, content.Width, CellLineHeight);
                BeepCellTemplateHelpers.DrawIconLabel(g, timeRect, DetailFont,
                    SecondaryTextColor, SvgsUIcons.Common.Clock, timeText, 10);
            }

            if (priority == "Emergency" || priority == "Critical" || priority == "High")
            {
                BeepCellTemplateHelpers.DrawBadge(g, rect, DetailFont,
                    priority, accentColor, Color.White);
            }
        }
    }
}
