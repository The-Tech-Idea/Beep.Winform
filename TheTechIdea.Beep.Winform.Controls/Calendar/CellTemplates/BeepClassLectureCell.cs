using System.Drawing;
using TheTechIdea.Beep.Icons;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.CellTemplates
{
    public sealed class BeepClassLectureCell : BeepCellTemplateBase
    {
        protected override void DrawCellContent(Graphics g, Rectangle rect)
        {
            if (Event == null) return;
            var theme = _currentTheme;
            if (theme == null) return;

            var lectureType = Metadatum("LectureType", "Lecture");
            var accentColor = lectureType switch
            {
                "Exam" => BeepCellTemplateHelpers.PriorityColor("critical"),
                "Lab" => BeepCellTemplateHelpers.StatusColor(CalendarEventStatus.Confirmed),
                "Seminar" => BeepCellTemplateHelpers.PriorityColor("high"),
                _ => BeepCellTemplateHelpers.PriorityColor("medium")
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

            var courseCode = Metadatum("CourseCode", "");
            var titleLine = string.IsNullOrEmpty(courseCode)
                ? Event.Title
                : $"{courseCode}: {Event.Title}";
            var titleRect = new Rectangle(content.X, drawY, content.Width, lineH);
            TextRenderer.DrawText(g, titleLine, titleFont, titleRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
                TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            drawY += lineH;

            var instructor = Metadatum("Instructor", "");
            var room = Metadatum("Room", "");
            var instLine = string.IsNullOrEmpty(instructor)
                ? $"Room {room}"
                : string.IsNullOrEmpty(room)
                    ? instructor
                    : $"{instructor} · Room {room}";
            if (!string.IsNullOrEmpty(instLine) && instLine != "Room ")
            {
                var instRect = new Rectangle(content.X, drawY, content.Width, lineH);
                BeepCellTemplateHelpers.DrawIconLabel(g, instRect, detailFont,
                    secondaryColor, SvgsUIcons.Common.Calendar, instLine, 10);
                drawY += lineH;
            }

            var enrollment = Metadatum("Enrollment", "");
            var hasSpace = content.Y + content.Height - drawY >= lineH;
            if (!string.IsNullOrEmpty(enrollment) && hasSpace)
            {
                var enrollRect = new Rectangle(content.X, drawY, content.Width, lineH);
                BeepCellTemplateHelpers.DrawIconLabel(g, enrollRect, detailFont,
                    secondaryColor, SvgsUIcons.Common.Calendar, enrollment, 10);
                drawY += lineH;
            }

            if (!Event.IsAllDay && content.Y + content.Height - drawY >= lineH)
            {
                var dayPattern = Event.IsRecurring
                    ? $"{Event.StartTime:ddd} " : "";
                var timeText = $"{dayPattern}{Event.StartTime:t}-{Event.EndTime:t}";
                var timeRect = new Rectangle(content.X, drawY, content.Width, lineH);
                BeepCellTemplateHelpers.DrawIconLabel(g, timeRect, detailFont,
                    secondaryColor, SvgsUIcons.Common.Clock, timeText, 10);
            }

            titleFont.Dispose();
            detailFont.Dispose();
        }
    }
}
