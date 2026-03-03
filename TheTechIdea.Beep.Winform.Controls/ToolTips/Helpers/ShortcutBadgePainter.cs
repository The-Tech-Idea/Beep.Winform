using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips.Helpers
{
    /// <summary>
    /// Sprint 5 — Renders keyboard shortcut key-cap badges in the tooltip footer.
    /// Produces the classic VS Code / Figma "Ctrl  S" key badge style.
    /// </summary>
    public static class ShortcutBadgePainter
    {
        // ──────────────────────────────────────────────────────────────────────
        // Metrics
        // ──────────────────────────────────────────────────────────────────────
        private const int KeyCapPaddingH = 6;   // horizontal inner padding per key
        private const int KeyCapPaddingV = 3;   // vertical inner padding per key
        private const int KeyCapRadius   = 3;   // corner radius
        private const int KeyCapSpacing  = 4;   // gap between adjacent key caps
        private static readonly Font DefaultFont =
            new Font("Consolas", 8.5f, FontStyle.Regular, GraphicsUnit.Point);

        // ──────────────────────────────────────────────────────────────────────
        // Measure
        // ──────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Measure the total width and height required to draw all shortcut badges.
        /// </summary>
        public static Size MeasureShortcuts(Graphics g, List<ShortcutKeyItem> shortcuts)
        {
            if (shortcuts == null || shortcuts.Count == 0)
                return Size.Empty;

            int totalWidth  = 0;
            int maxHeight   = 0;

            foreach (var item in shortcuts)
            {
                var (capW, capH) = MeasureSingleBadge(g, item);
                totalWidth += capW + KeyCapSpacing;
                maxHeight   = Math.Max(maxHeight, capH);
            }

            if (totalWidth > 0) totalWidth -= KeyCapSpacing; // remove trailing gap
            return new Size(totalWidth, maxHeight);
        }

        // ──────────────────────────────────────────────────────────────────────
        // Draw
        // ──────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Draw all shortcut badges starting from <paramref name="origin"/> (top-left corner).
        /// Colors are sourced from the active theme.
        /// </summary>
        public static void DrawShortcuts(
            Graphics g,
            List<ShortcutKeyItem> shortcuts,
            Point origin,
            IBeepTheme theme)
        {
            if (shortcuts == null || shortcuts.Count == 0) return;

            // Derive key-cap colors from theme (dark if background is dark, etc.)
            Color capBack    = DeriveKeyCapBackground(theme);
            Color capFore    = DeriveKeyCapForeground(theme);
            Color capTopLine = Lighten(capBack, 0.3f);
            Color capBotLine = Darken(capBack, 0.25f);

            int x = origin.X;
            int y = origin.Y;

            foreach (var item in shortcuts)
            {
                var parts = SplitToParts(item);
                foreach (var part in parts)
                {
                    int capW = DrawSingleCap(g, part, new Point(x, y),
                                             capBack, capFore, capTopLine, capBotLine);
                    x += capW + KeyCapSpacing;
                }

                // Separator "+" or " " between modifier and key already split by SplitToParts
            }
        }

        // ──────────────────────────────────────────────────────────────────────
        // Private helpers
        // ──────────────────────────────────────────────────────────────────────

        private static List<string> SplitToParts(ShortcutKeyItem item)
        {
            var parts = new List<string>();
            if ((item.Modifiers & Keys.Control) == Keys.Control) parts.Add("Ctrl");
            if ((item.Modifiers & Keys.Alt)     == Keys.Alt)     parts.Add("Alt");
            if ((item.Modifiers & Keys.Shift)   == Keys.Shift)   parts.Add("Shift");

            if (!string.IsNullOrEmpty(item.DisplayText))
                parts.Add(item.DisplayText);
            else if (item.Key != Keys.None)
                parts.Add(KeyToString(item.Key));

            return parts;
        }

        private static string KeyToString(Keys key)
        {
            return key switch
            {
                Keys.Delete  => "Del",
                Keys.Insert  => "Ins",
                Keys.Back    => "⌫",
                Keys.Return  => "Enter",
                Keys.Escape  => "Esc",
                Keys.Up      => "↑",
                Keys.Down    => "↓",
                Keys.Left    => "←",
                Keys.Right   => "→",
                Keys.Prior   => "PgUp",
                Keys.Next    => "PgDn",
                Keys.Home    => "Home",
                Keys.End     => "End",
                Keys.Tab     => "Tab",
                Keys.Space   => "Space",
                _            => key.ToString()
            };
        }

        private static (int width, int height) MeasureSingleBadge(Graphics g, ShortcutKeyItem item)
        {
            int totalW = 0;
            int maxH   = 0;
            foreach (var part in SplitToParts(item))
            {
                var sz = TextRenderer.MeasureText(g, part, DefaultFont);
                int capW = sz.Width  + KeyCapPaddingH * 2;
                int capH = sz.Height + KeyCapPaddingV * 2;
                totalW  += capW + KeyCapSpacing;
                maxH     = Math.Max(maxH, capH);
            }
            if (totalW > 0) totalW -= KeyCapSpacing;
            return (totalW, maxH);
        }

        /// <summary>Draws one key cap and returns its width.</summary>
        private static int DrawSingleCap(
            Graphics g, string label, Point origin,
            Color back, Color fore, Color topLine, Color botLine)
        {
            var sz   = TextRenderer.MeasureText(g, label, DefaultFont);
            int capW = sz.Width  + KeyCapPaddingH * 2;
            int capH = sz.Height + KeyCapPaddingV * 2;
            var rect = new Rectangle(origin.X, origin.Y, capW, capH);

            // Background
            using (var path = RoundedRect(rect, KeyCapRadius))
            using (var brush = new SolidBrush(back))
                g.FillPath(brush, path);

            // Raised border — lighter top, darker bottom
            using (var pen = new Pen(topLine))
                g.DrawLine(pen, rect.Left + 1, rect.Top, rect.Right - 2, rect.Top);
            using (var pen = new Pen(botLine))
                g.DrawLine(pen, rect.Left + 1, rect.Bottom - 1, rect.Right - 2, rect.Bottom - 1);
            using (var pen = new Pen(topLine))
            {
                g.DrawLine(pen, rect.Left,      rect.Top + 1, rect.Left,      rect.Bottom - 2);
                g.DrawLine(pen, rect.Right - 1, rect.Top + 1, rect.Right - 1, rect.Bottom - 2);
            }

            // Label
            TextRenderer.DrawText(g, label, DefaultFont,
                new Rectangle(rect.X + KeyCapPaddingH, rect.Y + KeyCapPaddingV, sz.Width, sz.Height),
                fore, TextFormatFlags.SingleLine);

            return capW;
        }

        private static GraphicsPath RoundedRect(Rectangle r, int radius)
        {
            var p = new GraphicsPath();
            p.AddArc(r.X, r.Y, radius * 2, radius * 2, 180, 90);
            p.AddArc(r.Right - radius * 2, r.Y, radius * 2, radius * 2, 270, 90);
            p.AddArc(r.Right - radius * 2, r.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
            p.AddArc(r.X, r.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
            p.CloseFigure();
            return p;
        }

        private static Color DeriveKeyCapBackground(IBeepTheme theme)
        {
            if (theme == null) return Color.FromArgb(50, 50, 50);
            // Use panel/card surface color from theme; fall back to a mid-gray
            try { return ChangeAlpha(theme.PanelBackColor, 230); }
            catch { return Color.FromArgb(60, 60, 60); }
        }

        private static Color DeriveKeyCapForeground(IBeepTheme theme)
        {
            if (theme == null) return Color.FromArgb(200, 200, 200);
            try { return theme.LabelForeColor; }
            catch { return Color.FromArgb(200, 200, 200); }
        }

        private static Color ChangeAlpha(Color c, int alpha)
            => Color.FromArgb(alpha, c.R, c.G, c.B);

        private static Color Lighten(Color c, float factor)
            => Color.FromArgb(c.A,
                Math.Min(255, (int)(c.R + (255 - c.R) * factor)),
                Math.Min(255, (int)(c.G + (255 - c.G) * factor)),
                Math.Min(255, (int)(c.B + (255 - c.B) * factor)));

        private static Color Darken(Color c, float factor)
            => Color.FromArgb(c.A,
                Math.Max(0, (int)(c.R * (1f - factor))),
                Math.Max(0, (int)(c.G * (1f - factor))),
                Math.Max(0, (int)(c.B * (1f - factor))));
    }
}
