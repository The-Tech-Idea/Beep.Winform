using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.CellTemplates
{
    internal static class BeepCellTemplateHelpers
    {
        public static void DrawAccentBadge(Graphics g, Rectangle rect, Color color, int width = 4)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;
            using var brush = new SolidBrush(color);
            g.FillRectangle(brush, rect.X, rect.Y, width, rect.Height);
        }

        public static void DrawStatusDot(Graphics g, int x, int centerY, Color color, int radius = 4)
        {
            using var brush = new SolidBrush(color);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.FillEllipse(brush, x, centerY - radius, radius * 2, radius * 2);
        }

        public static void DrawTimeBar(Graphics g, Rectangle rect,
            DateTime start, DateTime end, Color fillColor)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;
            var totalMinutes = (end - start).TotalMinutes;
            if (totalMinutes <= 0) return;
            var nowMinutes = (DateTime.Now - start).TotalMinutes;
            var fraction = (float)Math.Max(0, Math.Min(1, nowMinutes / totalMinutes));

            var barHeight = 3;
            var barY = rect.Bottom - barHeight - 2;
            var barRect = new Rectangle(rect.X + 2, barY, rect.Width - 4, barHeight);

            using var bgBrush = new SolidBrush(Color.FromArgb(40, fillColor));
            g.FillRectangle(bgBrush, barRect);

            var fillWidth = (int)(barRect.Width * fraction);
            if (fillWidth > 0)
            {
                using var fgBrush = new SolidBrush(fillColor);
                g.FillRectangle(fgBrush, barRect.X, barRect.Y, fillWidth, barHeight);
            }
        }

        public static void DrawProgressMini(Graphics g, Rectangle rect, float percent,
            Color fillColor, Color trackColor)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;
            percent = Math.Max(0, Math.Min(1, percent / 100f));
            var barHeight = 4;
            var barY = rect.Bottom - barHeight - 2;
            var barRect = new Rectangle(rect.X + 2, barY, rect.Width - 4, barHeight);

            using var bgBrush = new SolidBrush(trackColor);
            g.FillRectangle(bgBrush, barRect);

            var fillWidth = (int)(barRect.Width * percent);
            if (fillWidth > 0)
            {
                using var fgBrush = new SolidBrush(fillColor);
                g.FillRectangle(fgBrush, barRect.X, barRect.Y, fillWidth, barHeight);
            }
        }

        public static void DrawIconLabel(Graphics g, Rectangle rect, Font font,
            Color textColor, string iconPath, string text, int iconSize = 12)
        {
            if (string.IsNullOrEmpty(text) && string.IsNullOrEmpty(iconPath)) return;
            if (rect.Width <= 0 || rect.Height <= 0) return;

            int x = rect.X;
            int y = rect.Y + (rect.Height - iconSize) / 2;

            if (!string.IsNullOrEmpty(iconPath))
            {
                var iconRect = new Rectangle(x, y, iconSize, iconSize);
                try { StyledImagePainter.Paint(g, iconRect, iconPath); }
                catch (Exception ex)
                {
                    // The painter returns quietly for an unresolvable path; this only sees a corrupt
                    // image. Keyed by path so a bad asset reports once, not per paint.
                    Diagnostics.BeepLog.FailureOnce(iconPath, null, $"render cell icon '{iconPath}'", ex);
                }
                x += iconSize + 4;
            }

            if (!string.IsNullOrEmpty(text) && rect.Right - x > 0)
            {
                var textRect = new Rectangle(x, rect.Y, rect.Right - x, rect.Height);
                TextRenderer.DrawText(g, text, font ?? SystemFonts.DefaultFont,
                    textRect, textColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
                    TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            }
        }

        /// <summary>A populated theme slot, or a second slot when the instance left the first empty.</summary>
        private static Color Slot(Color slot, Color fallbackSlot) => slot.A > 0 ? slot : fallbackSlot;

        public static Color StatusColor(CalendarEventStatus status)
        {
            // Semantic states resolve from the theme's semantic slots (CLAUDE.md rule 3) - these
            // were a hardcoded GitHub-ish palette that ignored every theme. Static helpers have no
            // ViewPaintArgs, so they self-theme from the current theme like other Beep controls.
            var t = ThemeManagement.BeepThemesManager.CurrentTheme;
            return status switch
            {
                CalendarEventStatus.Confirmed => Slot(t?.SuccessColor ?? Color.Empty, t?.PrimaryColor ?? Color.Gray),
                CalendarEventStatus.Tentative => Slot(t?.WarningColor ?? Color.Empty, t?.SecondaryColor ?? Color.Gray),
                CalendarEventStatus.Cancelled => Slot(t?.ErrorColor ?? Color.Empty, t?.PrimaryColor ?? Color.Gray),
                _ => Slot(t?.DisabledForeColor ?? Color.Empty, t?.CalendarForeColor ?? Color.Gray)
            };
        }

        public static Color PriorityColor(string priority)
        {
            var th = ThemeManagement.BeepThemesManager.CurrentTheme;
            return (priority?.ToLowerInvariant()) switch
            {
                "critical" or "emergency" => Slot(th?.ErrorColor ?? Color.Empty, th?.PrimaryColor ?? Color.Gray),
                "high" => Slot(th?.WarningColor ?? Color.Empty, th?.SecondaryColor ?? Color.Gray),
                "medium" => Slot(th?.AccentColor ?? Color.Empty, th?.PrimaryColor ?? Color.Gray),
                _ => Slot(th?.DisabledForeColor ?? Color.Empty, th?.CalendarForeColor ?? Color.Gray)
            };
        }

        public static void DrawBadge(Graphics g, Rectangle rect, Font font,
            string text, Color bgColor, Color textColor)
        {
            if (string.IsNullOrEmpty(text) || rect.Width <= 0 || rect.Height <= 0) return;
            var size = TextRenderer.MeasureText(text, font ?? SystemFonts.DefaultFont);
            var badgeW = size.Width + 10;
            var badgeH = size.Height + 2;
            var badgeX = rect.Right - badgeW - 4;
            var badgeY = rect.Y + (rect.Height - badgeH) / 2;
            var badgeRect = new Rectangle(badgeX, badgeY, badgeW, badgeH);

            using var brush = new SolidBrush(bgColor);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.FillRectangle(brush, badgeRect);
            TextRenderer.DrawText(g, text, font ?? SystemFonts.DefaultFont,
                badgeRect, textColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                TextFormatFlags.NoPrefix);
        }
    }
}
