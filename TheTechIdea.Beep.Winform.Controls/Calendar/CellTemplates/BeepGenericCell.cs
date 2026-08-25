using System.Drawing;
using TheTechIdea.Beep.Icons;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.CellTemplates
{
    public sealed class BeepGenericCell : BeepCellTemplateBase
    {
        protected override void DrawCellContent(Graphics g, Rectangle rect)
        {
            if (Event == null) return;
            if (_currentTheme == null) return;

            var content = CellContentRect(rect);

            var accentColor = ParseColor("cell:accentColor",
                BeepCellTemplateHelpers.StatusColor(Event.Status));
            if (accentColor.A > 0)
            {
                BeepCellTemplateHelpers.DrawAccentBadge(g, rect, accentColor, CellAccentWidth);
            }

            var drawY = content.Y;

            var titleRect = new Rectangle(content.X, drawY, content.Width, CellLineHeight);
            TextRenderer.DrawText(g, Event.Title, TitleFont, titleRect, TextColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
                TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            drawY += CellLineHeight;

            for (int i = 0; i < 2; i++)
            {
                var labelKey = $"cell:label{i + 1}";
                var valueKey = $"cell:value{i + 1}";
                var iconKey = $"cell:icon{i + 1}";
                var label = Metadatum(labelKey);
                var value = Metadatum(valueKey);
                var icon = Metadatum(iconKey);
                var text = string.IsNullOrEmpty(label)
                    ? value
                    : $"{label}: {value}";
                if (string.IsNullOrEmpty(text)) continue;

                var iconPath = string.IsNullOrEmpty(icon) ? null : icon;
                if (iconPath != null && !iconPath.StartsWith("svgs:"))
                    iconPath = null;

                var detailRect = new Rectangle(content.X, drawY, content.Width, CellLineHeight);
                BeepCellTemplateHelpers.DrawIconLabel(g, detailRect, DetailFont,
                    SecondaryTextColor, iconPath, text, 10);
                drawY += CellLineHeight;
            }

            var showTime = !Event.IsAllDay && content.Y + content.Height - drawY >= CellLineHeight;
            if (showTime)
            {
                var timeText = $"{Event.StartTime:t} - {Event.EndTime:t}";
                var timeRect = new Rectangle(content.X, drawY, content.Width, CellLineHeight);
                BeepCellTemplateHelpers.DrawIconLabel(g, timeRect, DetailFont,
                    SecondaryTextColor, SvgsUIcons.Common.Clock, timeText, 10);
                drawY += CellLineHeight;
            }

            var badgeText = Metadatum("cell:badgeText");
            if (!string.IsNullOrEmpty(badgeText))
            {
                var badgeColor = ParseColor("cell:badgeColor", accentColor);
                BeepCellTemplateHelpers.DrawBadge(g, rect, DetailFont,
                    badgeText, badgeColor, Color.White);
            }
        }
    }
}
