using System.Drawing;
using TheTechIdea.Beep.Icons;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.CellTemplates
{
    public sealed class BeepTaskDeadlineCell : BeepCellTemplateBase
    {
        protected override void DrawCellContent(Graphics g, Rectangle rect)
        {
            if (Event == null) return;
            if (_currentTheme == null) return;

            var priority = Metadatum("Priority", "Medium");
            var priorityColor = BeepCellTemplateHelpers.PriorityColor(priority);
            var content = CellContentRect(rect);

            BeepCellTemplateHelpers.DrawAccentBadge(g, rect, priorityColor, CellAccentWidth);

            var drawY = content.Y;

            var titleRect = new Rectangle(content.X, drawY, content.Width, CellLineHeight);
            BeepCellTemplateHelpers.DrawStatusDot(g, titleRect.X,
                titleRect.Y + titleRect.Height / 2, priorityColor, 4);
            var titleTextRect = new Rectangle(titleRect.X + 10, titleRect.Y,
                titleRect.Width - 10, titleRect.Height);
            TextRenderer.DrawText(g, Event.Title, TitleFont, titleTextRect, TextColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
                TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            drawY += CellLineHeight;

            var assignee = Metadatum("AssigneeName", "");
            var project = Metadatum("ProjectName", "");
            var taskId = Metadatum("TaskId", "");
            var assigneeLine = string.IsNullOrEmpty(assignee)
                ? (string.IsNullOrEmpty(taskId) ? project : taskId)
                : string.IsNullOrEmpty(project)
                    ? assignee
                    : $"{assignee} · {project}";
            if (!string.IsNullOrEmpty(assigneeLine))
            {
                var assignRect = new Rectangle(content.X, drawY, content.Width, CellLineHeight);
                BeepCellTemplateHelpers.DrawIconLabel(g, assignRect, DetailFont,
                    SecondaryTextColor, SvgsUIcons.Common.Calendar, assigneeLine, 10);
                drawY += CellLineHeight;
            }

            var progress = ParseFloat("ProgressPercent", 0f);
            if (progress > 0)
            {
                var progressRect = new Rectangle(content.X, drawY, content.Width, CellLineHeight);
                BeepCellTemplateHelpers.DrawProgressMini(g, progressRect,
                    progress, priorityColor, Color.FromArgb(30, TextColor));
                var pctText = $"{progress:F0}%";
                var pctSize = TextRenderer.MeasureText(pctText, DetailFont);
                TextRenderer.DrawText(g, pctText, DetailFont,
                    new Rectangle(progressRect.Right - pctSize.Width - 4,
                        drawY, pctSize.Width, CellLineHeight),
                    SecondaryTextColor,
                    TextFormatFlags.Right | TextFormatFlags.VerticalCenter |
                    TextFormatFlags.NoPrefix);
                drawY += CellLineHeight;
            }

            var showDue = content.Y + content.Height - drawY >= CellLineHeight;
            if (showDue && !Event.IsAllDay)
            {
                var dueText = $"Due: {Event.EndTime:ddd h:mm tt}";
                var dueRect = new Rectangle(content.X, drawY, content.Width, CellLineHeight);
                BeepCellTemplateHelpers.DrawIconLabel(g, dueRect, DetailFont,
                    SecondaryTextColor, SvgsUIcons.Common.Clock, dueText, 10);
            }
        }
    }
}
