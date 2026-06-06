using System.Drawing;
using TheTechIdea.Beep.Icons;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.CellTemplates
{
    public sealed class BeepGenericCell : BeepCellTemplateBase
    {
        protected override void DrawCellContent(Graphics g, Rectangle rect)
        {
            if (Event == null) return;
            var theme = _currentTheme;
            if (theme == null) return;

            int pad = 6;
            int accentWidth = 4;
            var content = new Rectangle(rect.X + pad + accentWidth, rect.Y + pad,
                rect.Width - pad * 2 - accentWidth, rect.Height - pad * 2);

            var accentColor = ParseColor("cell:accentColor",
                BeepCellTemplateHelpers.StatusColor(Event.Status));
            if (accentColor.A > 0)
            {
                BeepCellTemplateHelpers.DrawAccentBadge(g, rect, accentColor, accentWidth);
            }

            var titleFont = new Font(Font?.FontFamily ?? FontFamily.GenericSansSerif, 9f, FontStyle.Regular);
            var detailFont = new Font(Font?.FontFamily ?? FontFamily.GenericSansSerif, 7.5f, FontStyle.Regular);
            var textColor = theme.PrimaryTextColor;
            var secondaryColor = Color.FromArgb(180, textColor);

            var drawY = content.Y;
            int lineH = 16;

            var titleRect = new Rectangle(content.X, drawY, content.Width, lineH);
            TextRenderer.DrawText(g, Event.Title, titleFont, titleRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
                TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            drawY += lineH;

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

                var detailRect = new Rectangle(content.X, drawY, content.Width, lineH);
                BeepCellTemplateHelpers.DrawIconLabel(g, detailRect, detailFont,
                    secondaryColor, iconPath, text, 10);
                drawY += lineH;
            }

            var showTime = !Event.IsAllDay && content.Y + content.Height - drawY >= lineH;
            if (showTime)
            {
                var timeText = $"{Event.StartTime:t} - {Event.EndTime:t}";
                var timeRect = new Rectangle(content.X, drawY, content.Width, lineH);
                BeepCellTemplateHelpers.DrawIconLabel(g, timeRect, detailFont,
                    secondaryColor, SvgsUIcons.Common.Clock, timeText, 10);
                drawY += lineH;
            }

            var badgeText = Metadatum("cell:badgeText");
            if (!string.IsNullOrEmpty(badgeText))
            {
                var badgeColor = ParseColor("cell:badgeColor", accentColor);
                BeepCellTemplateHelpers.DrawBadge(g, rect, detailFont,
                    badgeText, badgeColor, Color.White);
            }

            titleFont.Dispose();
            detailFont.Dispose();
        }
    }
}
