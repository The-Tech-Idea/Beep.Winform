using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.Rendering
{
    /// <summary>
    /// Custom paint engine for the Beep Ribbon — produces an Office 365 / Figma-compliant
    /// ribbon appearance. Replaces ToolStripProfessionalRenderer and BeepRibbonRenderer.
    /// All fonts via BeepThemesManager.ToFont(). All text via TextRenderer.DrawText.
    /// All icons via StyledImagePainter. DPI-scaled throughout.
    /// </summary>
    public class BeepRibbonPainter
    {
        private readonly RibbonTheme _theme;
        private readonly Control _owner;

        public BeepRibbonPainter(RibbonTheme theme, Control owner)
        {
            _theme = theme;
            _owner = owner;
        }

        private int S(int v) => DpiScalingHelper.ScaleValue(v, _owner);

        // ── Tab Strip ────────────────────────────────────────────────────────

        public void PaintTabStripBackground(Graphics g, Rectangle bounds)
        {
            using var brush = new SolidBrush(_theme.Background);
            g.FillRectangle(brush, bounds);
            // Subtle bottom border
            using var pen = new Pen(_theme.TabBorder, S(1));
            g.DrawLine(pen, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
        }

        public void PaintTab(Graphics g, Rectangle bounds, string text, bool isActive, bool isHovered)
        {
            int radius = S(6);
            var font = BeepThemesManager.ToFont(_theme.TabTypography) ?? SystemFonts.DefaultFont;

            if (isActive)
            {
                // Active tab: fills the tab area, connects to panel below
                var activeRect = new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height + S(2));
                using var path = CreateTopRoundedRect(activeRect, radius);
                using var fill = new SolidBrush(_theme.TabActiveBack);
                g.FillPath(fill, path);
                using var border = new Pen(_theme.TabBorder, S(1));
                g.DrawPath(border, path);

                // Accent indicator bar at bottom of active tab
                var accentColor = _theme.FocusBorder;
                if (accentColor == Color.Empty) accentColor = Color.FromArgb(0, 120, 215);
                int barH = S(2);
                var bar = new Rectangle(bounds.X + S(4), bounds.Bottom - barH, bounds.Width - S(8), barH);
                using var barBrush = new SolidBrush(accentColor);
                g.FillRectangle(barBrush, bar);
            }
            else if (isHovered)
            {
                using var path = CreateTopRoundedRect(bounds, radius);
                using var fill = new SolidBrush(_theme.HoverBack);
                g.FillPath(fill, path);
            }

            // Tab text
            var textColor = isActive ? _theme.Text : _theme.DisabledText;
            var textRect = new Rectangle(bounds.X + S(12), bounds.Y, bounds.Width - S(24), bounds.Height);
            TextRenderer.DrawText(g, text, font, textRect, textColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine);
        }

        // ── Group Panel ────────────────────────────────────────────────────────

        public void PaintGroupPanel(Graphics g, Rectangle bounds)
        {
            int radius = S(4);
            using var path = CreateRoundedRect(bounds, radius);
            using var fill = new SolidBrush(_theme.GroupBack);
            g.FillPath(fill, path);
            using var border = new Pen(_theme.GroupBorder, S(1));
            g.DrawPath(border, path);
        }

        public void PaintGroupSeparator(Graphics g, int x, int y, int height)
        {
            using var pen = new Pen(_theme.Separator, S(1));
            g.DrawLine(pen, x, y + S(8), x, y + height - S(8));
        }

        public void PaintGroupTitle(Graphics g, Rectangle bounds, string title)
        {
            if (string.IsNullOrEmpty(title)) return;
            var font = BeepThemesManager.ToFont(_theme.GroupTypography) ?? SystemFonts.DefaultFont;
            var titleRect = new Rectangle(bounds.X, bounds.Bottom - S(16), bounds.Width, S(14));
            TextRenderer.DrawText(g, title, font, titleRect, _theme.Text,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine);
        }

        // ── Command Buttons ──────────────────────────────────────────────────

        public void PaintLargeButton(Graphics g, Rectangle bounds, string? iconPath, string label,
            bool hovered, bool pressed, bool enabled = true)
        {
            PaintButtonBackground(g, bounds, hovered, pressed, enabled);
            int iconSize = S(32);
            if (!string.IsNullOrEmpty(iconPath) && iconSize > 0)
            {
                var iconRect = new Rectangle(bounds.X + (bounds.Width - iconSize) / 2,
                    bounds.Y + S(6), iconSize, iconSize);
                PaintIcon(g, iconRect, iconPath, enabled);
            }
            if (!string.IsNullOrEmpty(label))
            {
                var font = BeepThemesManager.ToFont(_theme.CommandTypography) ?? SystemFonts.DefaultFont;
                var labelRect = new Rectangle(bounds.X + S(2), bounds.Bottom - S(18), bounds.Width - S(4), S(16));
                var textColor = enabled ? _theme.Text : _theme.DisabledText;
                TextRenderer.DrawText(g, label, font, labelRect, textColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                    TextFormatFlags.EndEllipsis | TextFormatFlags.WordBreak);
            }
        }

        public void PaintSmallButton(Graphics g, Rectangle bounds, string? iconPath, string label,
            bool hovered, bool pressed, bool enabled = true, bool isSplit = false)
        {
            PaintButtonBackground(g, bounds, hovered, pressed, enabled);
            int iconSize = S(16);
            int x = bounds.X + S(6);
            if (!string.IsNullOrEmpty(iconPath) && iconSize > 0)
            {
                var iconRect = new Rectangle(x, bounds.Y + (bounds.Height - iconSize) / 2, iconSize, iconSize);
                PaintIcon(g, iconRect, iconPath, enabled);
                x += iconSize + S(4);
            }
            if (!string.IsNullOrEmpty(label))
            {
                var font = BeepThemesManager.ToFont(_theme.CommandTypography) ?? SystemFonts.DefaultFont;
                var labelRect = new Rectangle(x, bounds.Y, bounds.Width - x - (isSplit ? S(14) : S(6)), bounds.Height);
                var textColor = enabled ? _theme.Text : _theme.DisabledText;
                TextRenderer.DrawText(g, label, font, labelRect, textColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
                    TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine);
            }
            // Split button arrow
            if (isSplit)
            {
                int arrowX = bounds.Right - S(12);
                int arrowY = bounds.Y + bounds.Height / 2;
                using var pen = new Pen(enabled ? _theme.Text : _theme.DisabledText, DpiScalingHelper.ScaleValue(1.5f, _owner));
                g.DrawLine(pen, arrowX, arrowY - S(2), arrowX + S(4), arrowY + S(2));
                g.DrawLine(pen, arrowX + S(4), arrowY + S(2), arrowX + S(8), arrowY - S(2));
            }
        }

        private void PaintButtonBackground(Graphics g, Rectangle bounds, bool hovered, bool pressed, bool enabled)
        {
            if (!enabled) return;
            if (pressed)
            {
                using var path = CreateRoundedRect(bounds, S(4));
                using var fill = new SolidBrush(_theme.PressedBack);
                g.FillPath(fill, path);
            }
            else if (hovered)
            {
                using var path = CreateRoundedRect(bounds, S(4));
                using var fill = new SolidBrush(_theme.HoverBack);
                g.FillPath(fill, path);
                using var border = new Pen(_theme.FocusBorder, S(1));
                g.DrawPath(border, path);
            }
        }

        private void PaintIcon(Graphics g, Rectangle bounds, string iconPath, bool enabled)
        {
            var color = enabled ? _theme.IconColor : _theme.DisabledText;
            using var path = new GraphicsPath();
            path.AddRectangle(bounds);
            StyledImagePainter.PaintWithTint(g, path, iconPath, color, enabled ? 1f : 0.4f);
        }

        // ── QAT ──────────────────────────────────────────────────────────────

        public void PaintQatBackground(Graphics g, Rectangle bounds)
        {
            using var brush = new SolidBrush(_theme.QuickAccessBack);
            g.FillRectangle(brush, bounds);
            using var pen = new Pen(_theme.QuickAccessBorder, S(1));
            g.DrawLine(pen, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
        }

        public void PaintQatButton(Graphics g, Rectangle bounds, string? iconPath, bool hovered, bool pressed)
        {
            int iconSize = S(16);
            if (hovered || pressed)
            {
                using var path = CreateRoundedRect(bounds, S(3));
                using var fill = new SolidBrush(hovered ? _theme.HoverBack : _theme.PressedBack);
                g.FillPath(fill, path);
            }
            if (!string.IsNullOrEmpty(iconPath))
            {
                var iconRect = new Rectangle(bounds.X + (bounds.Width - iconSize) / 2,
                    bounds.Y + (bounds.Height - iconSize) / 2, iconSize, iconSize);
                PaintIcon(g, iconRect, iconPath, true);
            }
        }

        // ── Backstage ────────────────────────────────────────────────────────

        public void PaintBackstageOverlay(Graphics g, Rectangle bounds)
        {
            using var brush = new SolidBrush(Color.FromArgb(180, Color.Black));
            g.FillRectangle(brush, bounds);
        }

        public void PaintBackstagePanel(Graphics g, Rectangle bounds)
        {
            using var brush = new SolidBrush(_theme.Background);
            g.FillRectangle(brush, bounds);
        }

        public void PaintBackstageNavItem(Graphics g, Rectangle bounds, string text, string? iconPath,
            bool isSelected, bool isHovered)
        {
            if (isSelected)
            {
                using var brush = new SolidBrush(_theme.SelectionBack);
                g.FillRectangle(brush, bounds);
                int indicatorW = S(3);
                using var accent = new SolidBrush(_theme.FocusBorder);
                g.FillRectangle(accent, bounds.Left, bounds.Y, indicatorW, bounds.Height);
            }
            else if (isHovered)
            {
                using var brush = new SolidBrush(_theme.HoverBack);
                g.FillRectangle(brush, bounds);
            }
            var font = BeepThemesManager.ToFont(_theme.CommandTypography) ?? SystemFonts.DefaultFont;
            int x = bounds.X + S(12);
            if (!string.IsNullOrEmpty(iconPath))
            {
                var iconRect = new Rectangle(x, bounds.Y + (bounds.Height - S(20)) / 2, S(20), S(20));
                PaintIcon(g, iconRect, iconPath, true);
                x += S(24);
            }
            var textRect = new Rectangle(x, bounds.Y, bounds.Width - x - S(8), bounds.Height);
            TextRenderer.DrawText(g, text, font, textRect, _theme.Text,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine);
        }

        // ── Dropdown menu ────────────────────────────────────────────────────

        public void PaintDropDownBackground(Graphics g, Rectangle bounds)
        {
            int radius = S(4);
            using var path = CreateRoundedRect(bounds, radius);
            using var fill = new SolidBrush(_theme.GroupBack);
            g.FillPath(fill, path);
            using var border = new Pen(_theme.GroupBorder, S(1));
            g.DrawPath(border, path);
        }

        public void PaintDropDownItem(Graphics g, Rectangle bounds, string text, string? iconPath,
            string? shortcutText, bool hovered, bool enabled)
        {
            if (hovered && enabled)
            {
                using var path = CreateRoundedRect(bounds, S(3));
                using var fill = new SolidBrush(_theme.HoverBack);
                g.FillPath(fill, path);
            }
            var font = BeepThemesManager.ToFont(_theme.CommandTypography) ?? SystemFonts.DefaultFont;
            int x = bounds.X + S(8);
            if (!string.IsNullOrEmpty(iconPath))
            {
                var iconRect = new Rectangle(x, bounds.Y + (bounds.Height - S(16)) / 2, S(16), S(16));
                PaintIcon(g, iconRect, iconPath, enabled);
                x += S(20);
            }
            var textColor = enabled ? _theme.Text : _theme.DisabledText;
            var textRect = new Rectangle(x, bounds.Y, bounds.Width - x - S(40), bounds.Height);
            TextRenderer.DrawText(g, text, font, textRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine);
            if (!string.IsNullOrEmpty(shortcutText))
            {
                var shortcutRect = new Rectangle(bounds.Right - S(60), bounds.Y, S(56), bounds.Height);
                TextRenderer.DrawText(g, shortcutText, font, shortcutRect, _theme.DisabledText,
                    TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine);
            }
        }

        public void PaintDropDownSeparator(Graphics g, Rectangle bounds)
        {
            int y = bounds.Y + bounds.Height / 2;
            using var pen = new Pen(_theme.Separator, S(1));
            g.DrawLine(pen, bounds.X + S(8), y, bounds.Right - S(8), y);
        }

        // ── Helpers ──────────────────────────────────────────────────────────

        private GraphicsPath CreateRoundedRect(Rectangle r, int radius)
        {
            var path = new GraphicsPath();
            if (r.Width <= 0 || r.Height <= 0) return path;
            int d = radius * 2;
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private GraphicsPath CreateTopRoundedRect(Rectangle r, int radius)
        {
            var path = new GraphicsPath();
            if (r.Width <= 0 || r.Height <= 0) return path;
            int d = radius * 2;
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddLine(r.Right, r.Y + radius, r.Right, r.Bottom);
            path.AddLine(r.Right, r.Bottom, r.Left, r.Bottom);
            path.CloseFigure();
            return path;
        }
    }
}
