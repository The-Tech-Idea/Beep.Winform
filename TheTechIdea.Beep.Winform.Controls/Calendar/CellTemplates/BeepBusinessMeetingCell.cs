using System.Drawing;
using TheTechIdea.Beep.Icons;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.CellTemplates
{
    public sealed class BeepBusinessMeetingCell : BeepCellTemplateBase
    {
        protected override void DrawCellContent(Graphics g, Rectangle rect)
        {
            if (Event == null) return;
            if (_currentTheme == null) return;

            var accentColor = BeepCellTemplateHelpers.StatusColor(Event.Status);
            var content = CellContentRect(rect);
            BeepCellTemplateHelpers.DrawAccentBadge(g, rect, accentColor, CellAccentWidth);

            var drawY = content.Y;

            var titleRect = new Rectangle(content.X, drawY, content.Width, CellLineHeight);
            TextRenderer.DrawText(g, Event.Title, TitleFont, titleRect, TextColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
                TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            drawY += CellLineHeight;

            var roomName = Metadatum("ConferenceRoom", "");
            var attendees = Metadatum("Attendees", "");
            var roomLine = string.IsNullOrEmpty(roomName)
                ? ""
                : string.IsNullOrEmpty(attendees)
                    ? roomName
                    : $"{roomName} · {attendees}";
            if (!string.IsNullOrEmpty(roomLine))
            {
                var roomRect = new Rectangle(content.X, drawY, content.Width, CellLineHeight);
                BeepCellTemplateHelpers.DrawIconLabel(g, roomRect, DetailFont,
                    SecondaryTextColor, SvgsUIcons.Common.Calendar, roomLine, 10);
                drawY += CellLineHeight;
            }

            var agenda = Metadatum("Agenda", "");
            var hasSpace = content.Y + content.Height - drawY >= CellLineHeight;
            if (!string.IsNullOrEmpty(agenda) && hasSpace)
            {
                var agendaRect = new Rectangle(content.X, drawY, content.Width, CellLineHeight);
                BeepCellTemplateHelpers.DrawIconLabel(g, agendaRect, DetailFont,
                    SecondaryTextColor, SvgsUIcons.Common.Edit, agenda, 10);
                drawY += CellLineHeight;
            }

            if (!Event.IsAllDay && content.Y + content.Height - drawY >= CellLineHeight)
            {
                var timeText = $"{Event.StartTime:t} - {Event.EndTime:t}";
                var timeRect = new Rectangle(content.X, drawY, content.Width, CellLineHeight);
                BeepCellTemplateHelpers.DrawIconLabel(g, timeRect, DetailFont,
                    SecondaryTextColor, SvgsUIcons.Common.Clock, timeText, 10);
            }
        }
    }
}
