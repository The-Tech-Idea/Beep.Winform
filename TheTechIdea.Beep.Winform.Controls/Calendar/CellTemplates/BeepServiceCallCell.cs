using System.Drawing;
using TheTechIdea.Beep.Icons;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.CellTemplates
{
    public sealed class BeepServiceCallCell : BeepCellTemplateBase
    {
        protected override void DrawCellContent(Graphics g, Rectangle rect)
        {
            if (Event == null) return;
            var theme = _currentTheme;
            if (theme == null) return;

            var priority = Metadatum("Priority", "Medium");
            var accentColor = BeepCellTemplateHelpers.PriorityColor(priority);

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

            var ticketNumber = Metadatum("TicketNumber", "");
            var titleLine = string.IsNullOrEmpty(ticketNumber)
                ? Event.Title
                : $"{Event.Title} · #{ticketNumber}";
            var titleRect = new Rectangle(content.X, drawY, content.Width, lineH);
            TextRenderer.DrawText(g, titleLine, titleFont, titleRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
                TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            drawY += lineH;

            var techName = Metadatum("TechnicianName", "");
            var eta = Metadatum("ETAMinutes", "");
            var techLine = string.IsNullOrEmpty(techName)
                ? (string.IsNullOrEmpty(eta) ? "" : $"ETA {eta}m")
                : string.IsNullOrEmpty(eta)
                    ? techName
                    : $"{techName} · ETA {eta}m";
            if (!string.IsNullOrEmpty(techLine))
            {
                var techRect = new Rectangle(content.X, drawY, content.Width, lineH);
                BeepCellTemplateHelpers.DrawIconLabel(g, techRect, detailFont,
                    secondaryColor, SvgsUIcons.Common.Calendar, techLine, 10);
                drawY += lineH;
            }

            var equipment = Metadatum("EquipmentType", "");
            var hasSpace = content.Y + content.Height - drawY >= lineH;
            if (!string.IsNullOrEmpty(equipment) && hasSpace)
            {
                var equipRect = new Rectangle(content.X, drawY, content.Width, lineH);
                BeepCellTemplateHelpers.DrawIconLabel(g, equipRect, detailFont,
                    secondaryColor, SvgsUIcons.Common.Edit, equipment, 10);
                drawY += lineH;
            }

            if (hasSpace && !Event.IsAllDay)
            {
                var timeText = $"{Event.StartTime:t} - {Event.EndTime:t}";
                var timeRect = new Rectangle(content.X, drawY, content.Width, lineH);
                BeepCellTemplateHelpers.DrawIconLabel(g, timeRect, detailFont,
                    secondaryColor, SvgsUIcons.Common.Clock, timeText, 10);
            }

            if (priority == "Emergency" || priority == "Critical" || priority == "High")
            {
                BeepCellTemplateHelpers.DrawBadge(g, rect, detailFont,
                    priority, accentColor, Color.White);
            }

            titleFont.Dispose();
            detailFont.Dispose();
        }
    }
}
