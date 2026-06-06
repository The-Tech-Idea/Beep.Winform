using System.Drawing;
using TheTechIdea.Beep.Icons;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.CellTemplates
{
    public sealed class BeepTaskDeadlineCell : BeepCellTemplateBase
    {
        protected override void DrawCellContent(Graphics g, Rectangle rect)
        {
            if (Event == null) return;
            var theme = _currentTheme;
            if (theme == null) return;

            var priority = Metadatum("Priority", "Medium");
            var priorityColor = BeepCellTemplateHelpers.PriorityColor(priority);
            int pad = 6;
            int accentWidth = 4;
            var content = new Rectangle(rect.X + pad + accentWidth, rect.Y + pad,
                rect.Width - pad * 2 - accentWidth, rect.Height - pad * 2);

            BeepCellTemplateHelpers.DrawAccentBadge(g, rect, priorityColor, accentWidth);

            var titleFont = new Font(Font?.FontFamily ?? FontFamily.GenericSansSerif, 9f, FontStyle.Regular);
            var detailFont = new Font(Font?.FontFamily ?? FontFamily.GenericSansSerif, 7.5f, FontStyle.Regular);
            var textColor = theme.PrimaryTextColor;
            var secondaryColor = Color.FromArgb(180, textColor);

            int lineH = 16;
            var drawY = content.Y;

            var titleRect = new Rectangle(content.X, drawY, content.Width, lineH);
            BeepCellTemplateHelpers.DrawStatusDot(g, titleRect.X,
                titleRect.Y + titleRect.Height / 2, priorityColor, 4);
            var titleTextRect = new Rectangle(titleRect.X + 10, titleRect.Y,
                titleRect.Width - 10, titleRect.Height);
            TextRenderer.DrawText(g, Event.Title, titleFont, titleTextRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
                TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            drawY += lineH;

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
                var assignRect = new Rectangle(content.X, drawY, content.Width, lineH);
                BeepCellTemplateHelpers.DrawIconLabel(g, assignRect, detailFont,
                    secondaryColor, SvgsUIcons.Common.Calendar, assigneeLine, 10);
                drawY += lineH;
            }

            var progress = ParseFloat("ProgressPercent", 0f);
            if (progress > 0)
            {
                var progressRect = new Rectangle(content.X, drawY, content.Width, lineH);
                BeepCellTemplateHelpers.DrawProgressMini(g, progressRect,
                    progress, priorityColor, Color.FromArgb(30, textColor));
                var pctText = $"{progress:F0}%";
                var pctSize = TextRenderer.MeasureText(pctText, detailFont);
                TextRenderer.DrawText(g, pctText, detailFont,
                    new Rectangle(progressRect.Right - pctSize.Width - 4,
                        drawY, pctSize.Width, lineH),
                    secondaryColor,
                    TextFormatFlags.Right | TextFormatFlags.VerticalCenter |
                    TextFormatFlags.NoPrefix);
                drawY += lineH;
            }

            var showDue = content.Y + content.Height - drawY >= lineH;
            if (showDue && !Event.IsAllDay)
            {
                var dueText = $"Due: {Event.EndTime:ddd h:mm tt}";
                var dueRect = new Rectangle(content.X, drawY, content.Width, lineH);
                BeepCellTemplateHelpers.DrawIconLabel(g, dueRect, detailFont,
                    secondaryColor, SvgsUIcons.Common.Clock, dueText, 10);
            }

            titleFont.Dispose();
            detailFont.Dispose();
        }
    }
}
