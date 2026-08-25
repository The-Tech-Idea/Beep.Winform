using System.Drawing;
using TheTechIdea.Beep.Icons;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.CellTemplates
{
    public sealed class BeepClassLectureCell : BeepCellTemplateBase
    {
        protected override void DrawCellContent(Graphics g, Rectangle rect)
        {
            if (Event == null) return;
            if (_currentTheme == null) return;

            var lectureType = Metadatum("LectureType", "Lecture");
            var accentColor = lectureType switch
            {
                "Exam" => BeepCellTemplateHelpers.PriorityColor("critical"),
                "Lab" => BeepCellTemplateHelpers.StatusColor(CalendarEventStatus.Confirmed),
                "Seminar" => BeepCellTemplateHelpers.PriorityColor("high"),
                _ => BeepCellTemplateHelpers.PriorityColor("medium")
            };

            var content = CellContentRect(rect);
            BeepCellTemplateHelpers.DrawAccentBadge(g, rect, accentColor, CellAccentWidth);

            var drawY = content.Y;

            var courseCode = Metadatum("CourseCode", "");
            var titleLine = string.IsNullOrEmpty(courseCode)
                ? Event.Title
                : $"{courseCode}: {Event.Title}";
            var titleRect = new Rectangle(content.X, drawY, content.Width, CellLineHeight);
            TextRenderer.DrawText(g, titleLine, TitleFont, titleRect, TextColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
                TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            drawY += CellLineHeight;

            var instructor = Metadatum("Instructor", "");
            var room = Metadatum("Room", "");
            var instLine = string.IsNullOrEmpty(instructor)
                ? $"Room {room}"
                : string.IsNullOrEmpty(room)
                    ? instructor
                    : $"{instructor} · Room {room}";
            if (!string.IsNullOrEmpty(instLine) && instLine != "Room ")
            {
                var instRect = new Rectangle(content.X, drawY, content.Width, CellLineHeight);
                BeepCellTemplateHelpers.DrawIconLabel(g, instRect, DetailFont,
                    SecondaryTextColor, SvgsUIcons.Common.Calendar, instLine, 10);
                drawY += CellLineHeight;
            }

            var enrollment = Metadatum("Enrollment", "");
            var hasSpace = content.Y + content.Height - drawY >= CellLineHeight;
            if (!string.IsNullOrEmpty(enrollment) && hasSpace)
            {
                var enrollRect = new Rectangle(content.X, drawY, content.Width, CellLineHeight);
                BeepCellTemplateHelpers.DrawIconLabel(g, enrollRect, DetailFont,
                    SecondaryTextColor, SvgsUIcons.Common.Calendar, enrollment, 10);
                drawY += CellLineHeight;
            }

            if (!Event.IsAllDay && content.Y + content.Height - drawY >= CellLineHeight)
            {
                var dayPattern = Event.IsRecurring
                    ? $"{Event.StartTime:ddd} " : "";
                var timeText = $"{dayPattern}{Event.StartTime:t}-{Event.EndTime:t}";
                var timeRect = new Rectangle(content.X, drawY, content.Width, CellLineHeight);
                BeepCellTemplateHelpers.DrawIconLabel(g, timeRect, DetailFont,
                    SecondaryTextColor, SvgsUIcons.Common.Clock, timeText, 10);
            }
        }
    }
}
