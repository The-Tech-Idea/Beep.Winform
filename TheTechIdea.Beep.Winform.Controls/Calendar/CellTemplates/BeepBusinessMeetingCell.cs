using System.Drawing;
using TheTechIdea.Beep.Icons;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.CellTemplates
{
    public sealed class BeepBusinessMeetingCell : BeepCellTemplateBase
    {
        protected override void DrawCellContent(Graphics g, Rectangle rect)
        {
            if (Event == null) return;
            var theme = _currentTheme;
            if (theme == null) return;

            var accentColor = BeepCellTemplateHelpers.StatusColor(Event.Status);
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

            var titleRect = new Rectangle(content.X, drawY, content.Width, lineH);
            TextRenderer.DrawText(g, Event.Title, titleFont, titleRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
                TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            drawY += lineH;

            var roomName = Metadatum("ConferenceRoom", "");
            var attendees = Metadatum("Attendees", "");
            var roomLine = string.IsNullOrEmpty(roomName)
                ? ""
                : string.IsNullOrEmpty(attendees)
                    ? roomName
                    : $"{roomName} · {attendees}";
            if (!string.IsNullOrEmpty(roomLine))
            {
                var roomRect = new Rectangle(content.X, drawY, content.Width, lineH);
                BeepCellTemplateHelpers.DrawIconLabel(g, roomRect, detailFont,
                    secondaryColor, SvgsUIcons.Common.Calendar, roomLine, 10);
                drawY += lineH;
            }

            var agenda = Metadatum("Agenda", "");
            var hasSpace = content.Y + content.Height - drawY >= lineH;
            if (!string.IsNullOrEmpty(agenda) && hasSpace)
            {
                var agendaRect = new Rectangle(content.X, drawY, content.Width, lineH);
                BeepCellTemplateHelpers.DrawIconLabel(g, agendaRect, detailFont,
                    secondaryColor, SvgsUIcons.Common.Edit, agenda, 10);
                drawY += lineH;
            }

            if (!Event.IsAllDay && content.Y + content.Height - drawY >= lineH)
            {
                var timeText = $"{Event.StartTime:t} - {Event.EndTime:t}";
                var timeRect = new Rectangle(content.X, drawY, content.Width, lineH);
                BeepCellTemplateHelpers.DrawIconLabel(g, timeRect, detailFont,
                    secondaryColor, SvgsUIcons.Common.Clock, timeText, 10);
            }

            titleFont.Dispose();
            detailFont.Dispose();
        }
    }
}
