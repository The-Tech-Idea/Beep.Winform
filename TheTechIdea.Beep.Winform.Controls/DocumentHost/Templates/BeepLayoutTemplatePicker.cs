// BeepLayoutTemplatePicker.cs
// Owner-draw floating popup for selecting a layout template.
// Shows template name, description, group-count diagram and icon glyph.
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    /// <summary>
    /// Floating, owner-drawn template picker that lists all registered templates.
    /// Set <see cref="SelectedTemplate"/> after <see cref="Form.ShowDialog"/> returns
    /// <see cref="DialogResult.OK"/>.
    /// </summary>
    internal sealed class BeepLayoutTemplatePicker : Form
    {
        // ── Layout constants ──────────────────────────────────────────────────
        private const int ItemHeight   = 64;
        private const int MaxVisible   = 7;
        private const int ThumbW       = 72;
        private const int ThumbH       = 44;
        private const int ThumbPadX    = 8;

        // ── Controls ─────────────────────────────────────────────────────────
        private readonly TextBox  _search;
        private readonly Panel    _listHost;
        private readonly Label    _titleLabel;

        // ── State ─────────────────────────────────────────────────────────────
        private readonly BeepLayoutTemplateLibrary _library;
        private readonly IBeepTheme?               _theme;
        private List<BeepLayoutTemplate>           _filtered = new();
        private int                                _selectedIndex;
        private int                                _scrollOffset;

        // ── Output ────────────────────────────────────────────────────────────
        /// <summary>The template chosen by the user; <c>null</c> if cancelled.</summary>
        public BeepLayoutTemplate? SelectedTemplate { get; private set; }

        // ── Construction ──────────────────────────────────────────────────────

        internal BeepLayoutTemplatePicker(
            BeepLayoutTemplateLibrary library,
            IBeepTheme?               theme,
            Point                     screenPosition)
        {
            _library = library;
            _theme   = theme;

            // ── Form setup ────────────────────────────────────────────────────
            FormBorderStyle = FormBorderStyle.None;
            StartPosition   = FormStartPosition.Manual;
            Location        = screenPosition;
            Size            = new Size(560, 60 + ItemHeight * MaxVisible + 4);
            ShowInTaskbar   = false;
            KeyPreview      = true;
            TopMost         = true;
            BackColor       = theme?.PanelBackColor ?? Tc(BackColor, SystemColors.Window);

            // ── Title bar ─────────────────────────────────────────────────────
            _titleLabel = new Label
            {
                Text      = "Apply Layout Template",
                Font      = BeepFontManager.GetCachedFont("Segoe UI Semibold", 10f, FontStyle.Bold),
                ForeColor = theme?.AppBarTitleForeColor ?? Tc(BackColor, SystemColors.ControlText),
                BackColor = theme?.AppBarTitleBackColor ?? Tc(BackColor, SystemColors.ActiveCaption),
                Dock      = DockStyle.Top,
                Height    = 30,
                TextAlign = ContentAlignment.MiddleCenter,
            };
            Controls.Add(_titleLabel);

            // ── Search box ────────────────────────────────────────────────────
            _search = new TextBox
            {
                PlaceholderText = "Search templates…",
                Font      = BeepFontManager.GetCachedFont("Segoe UI", 10f, FontStyle.Regular),
                Dock      = DockStyle.Top,
                Height    = 28,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = theme?.GridBackColor ?? Tc(BackColor, SystemColors.Window),
                ForeColor = theme?.GridForeColor ?? Tc(BackColor, SystemColors.WindowText),
            };
            _search.TextChanged += (_, _) => RefreshList();
            Controls.Add(_search);

            // ── List host ─────────────────────────────────────────────────────
            _listHost = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = theme?.PanelBackColor ?? Tc(BackColor, SystemColors.Window),
            };
            _listHost.Paint        += OnListPaint;
            _listHost.MouseMove    += OnListMouseMove;
            _listHost.MouseClick   += OnListMouseClick;
            _listHost.MouseWheel   += OnListMouseWheel;
            Controls.Add(_listHost);

            // ── Keyboard ─────────────────────────────────────────────────────
            KeyDown += OnKeyDown;

            Deactivate += (_, _) => { if (DialogResult != DialogResult.OK) Close(); };

            RefreshList();
            ActiveControl = _search;
        }

        // ── List management ───────────────────────────────────────────────────

        private void RefreshList()
        {
            var q = _search.Text.Trim();
            _filtered = string.IsNullOrEmpty(q)
                ? _library.GetAll().ToList()
                : _library.GetAll()
                    .Where(t => t.Name.Contains(q, StringComparison.OrdinalIgnoreCase)
                             || t.Description.Contains(q, StringComparison.OrdinalIgnoreCase)
                             || t.Category.Contains(q, StringComparison.OrdinalIgnoreCase))
                    .ToList();

            _selectedIndex = _filtered.Count > 0 ? 0 : -1;
            _scrollOffset  = 0;
            _listHost.Invalidate();
        }

        // ── Painting ──────────────────────────────────────────────────────────

        private void OnListPaint(object? sender, PaintEventArgs e)
        {
            var g   = e.Graphics;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            var w   = _listHost.Width;
            int y   = 0;

            var backColor   = _theme?.PanelBackColor       ?? Tc(BackColor, SystemColors.Window);
            var textColor   = _theme?.GridForeColor        ?? Tc(BackColor, SystemColors.WindowText);
            var subColor    = _theme?.SecondaryTextColor   ?? Tc(BackColor, SystemColors.GrayText);
            var selBack     = _theme?.SelectedRowBackColor   ?? Tc(BackColor, SystemColors.Highlight);
            var selFore     = _theme?.SelectedRowForeColor   ?? Tc(BackColor, SystemColors.HighlightText);
            var divColor    = _theme?.ActiveBorderColor       ?? Tc(BackColor, SystemColors.ControlLight);
            var accentColor = _theme?.AccentColor          ?? Tc(BackColor, SystemColors.Highlight);

            var titleFont = BeepFontManager.GetCachedFont("Segoe UI", 10f, FontStyle.Regular);
            var descFont  = BeepFontManager.GetCachedFont("Segoe UI", 8.5f, FontStyle.Regular);
            var glyphFont = BeepFontManager.GetCachedFont("Segoe UI Symbol", 20f, FontStyle.Regular);
            var tagFont   = BeepFontManager.GetCachedFont("Segoe UI", 7.5f, FontStyle.Regular);

            for (int i = _scrollOffset; i < _filtered.Count && y < _listHost.Height; i++)
            {
                var t        = _filtered[i];
                bool isSel   = i == _selectedIndex;
                var itemRect = new Rectangle(0, y, w, ItemHeight);

                // Background
                using (var br = new SolidBrush(isSel ? selBack : (i % 2 == 0 ? backColor : Color.FromArgb(8, 128, 128, 128))))
                    g.FillRectangle(br, itemRect);

                // ── Thumbnail diagram ─────────────────────────────────────────
                var thumbRect = new Rectangle(ThumbPadX, y + (ItemHeight - ThumbH) / 2, ThumbW, ThumbH);
                DrawTemplateThumbnail(g, t, thumbRect, isSel ? selFore : textColor, accentColor);

                // ── Glyph badge ───────────────────────────────────────────────
                if (!string.IsNullOrEmpty(t.IconGlyph))
                {
                    var glyphColor = isSel ? Color.FromArgb(180, selFore) : Color.FromArgb(120, textColor);
                    TextRenderer.DrawText(g, t.IconGlyph, glyphFont,
                        new Rectangle(ThumbPadX + ThumbW + 6, y + 4, 28, 28),
                        glyphColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                }

                int textX = ThumbPadX + ThumbW + 38;
                int textW = w - textX - 8;

                // Title
                TextRenderer.DrawText(g, t.Name, titleFont,
                    new Rectangle(textX, y + 8, textW, 20),
                    isSel ? selFore : textColor,
                    TextFormatFlags.Left | TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis);

                // Description
                TextRenderer.DrawText(g, t.Description, descFont,
                    new Rectangle(textX, y + 28, textW, 16),
                    isSel ? Color.FromArgb(220, selFore) : subColor,
                    TextFormatFlags.Left | TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis);

                // Group count tag
                string tag = $"{t.GroupCount} group{(t.GroupCount != 1 ? "s" : "")}";
                var tagSize = TextRenderer.MeasureText(tag, tagFont);
                var tagRect = new Rectangle(textX, y + 44, tagSize.Width + 8, 14);
                using (var br = new SolidBrush(isSel ? Color.FromArgb(60, selFore) : Color.FromArgb(30, accentColor)))
                    g.FillRectangle(br, tagRect);
                TextRenderer.DrawText(g, tag, tagFont, tagRect,
                    isSel ? selFore : accentColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                // Divider
                using (var pen = new Pen(divColor))
                    g.DrawLine(pen, 0, y + ItemHeight - 1, w, y + ItemHeight - 1);

                y += ItemHeight;
            }

            // Empty state
            if (_filtered.Count == 0)
            {
                TextRenderer.DrawText(g, "No templates match your search.", descFont,
                    new Rectangle(0, 20, w, 24), subColor, TextFormatFlags.HorizontalCenter);
            }
        }

        /// <summary>Draws a small structural diagram for the template's shape.</summary>
        private static void DrawTemplateThumbnail(
            Graphics g, BeepLayoutTemplate t, Rectangle r, Color foreColor, Color accentColor)
        {
            using var borderPen = new Pen(foreColor, 1);
            g.DrawRectangle(borderPen, r.X, r.Y, r.Width - 1, r.Height - 1);

            // Draw split lines based on group count (simplified visual)
            switch (t.GroupCount)
            {
                case 2 when t.LayoutShapeJson.Contains("\"Horizontal\""):
                    // vertical divider
                    int mid = r.X + r.Width / 2;
                    using (var pen = new Pen(accentColor, 1.5f))
                        g.DrawLine(pen, mid, r.Y + 2, mid, r.Bottom - 2);
                    break;
                case 2:
                    // horizontal divider
                    int midY = r.Y + r.Height / 2;
                    using (var pen = new Pen(accentColor, 1.5f))
                        g.DrawLine(pen, r.X + 2, midY, r.Right - 2, midY);
                    break;
                case 3:
                    // L-shape: vertical + right horizontal
                    int splitX = r.X + (int)(r.Width * 0.4f);
                    int splitY = r.Y + r.Height / 2;
                    using (var pen = new Pen(accentColor, 1.5f))
                    {
                        g.DrawLine(pen, splitX, r.Y + 2, splitX, r.Bottom - 2);
                        g.DrawLine(pen, splitX, splitY, r.Right - 2, splitY);
                    }
                    break;
                case 4:
                    // 2x2 grid
                    int cx = r.X + r.Width  / 2;
                    int cy = r.Y + r.Height / 2;
                    using (var pen = new Pen(accentColor, 1.5f))
                    {
                        g.DrawLine(pen, cx, r.Y + 2, cx, r.Bottom - 2);
                        g.DrawLine(pen, r.X + 2, cy, r.Right - 2, cy);
                    }
                    break;
                default:
                    // Single — fill with soft accent
                    using (var br = new SolidBrush(Color.FromArgb(30, accentColor)))
                        g.FillRectangle(br, r.X + 1, r.Y + 1, r.Width - 2, r.Height - 2);
                    break;
            }
        }

        // ── Mouse ─────────────────────────────────────────────────────────────

        private void OnListMouseMove(object? sender, MouseEventArgs e)
        {
            int idx = _scrollOffset + e.Y / ItemHeight;
            if (idx >= 0 && idx < _filtered.Count && idx != _selectedIndex)
            {
                _selectedIndex = idx;
                _listHost.Invalidate();
            }
        }

        private void OnListMouseClick(object? sender, MouseEventArgs e)
        {
            int idx = _scrollOffset + e.Y / ItemHeight;
            if (idx >= 0 && idx < _filtered.Count)
            {
                _selectedIndex = idx;
                CommitSelection();
            }
        }

        private void OnListMouseWheel(object? sender, MouseEventArgs e)
        {
            int delta = e.Delta > 0 ? -1 : 1;
            _scrollOffset = Math.Max(0, Math.Min(_filtered.Count - MaxVisible, _scrollOffset + delta));
            _listHost.Invalidate();
        }

        // ── Keyboard ─────────────────────────────────────────────────────────

        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Down:
                    if (_selectedIndex < _filtered.Count - 1)
                    {
                        _selectedIndex++;
                        EnsureVisible(_selectedIndex);
                        _listHost.Invalidate();
                    }
                    e.Handled = true;
                    break;
                case Keys.Up:
                    if (_selectedIndex > 0)
                    {
                        _selectedIndex--;
                        EnsureVisible(_selectedIndex);
                        _listHost.Invalidate();
                    }
                    e.Handled = true;
                    break;
                case Keys.Enter:
                    CommitSelection();
                    e.Handled = true;
                    break;
                case Keys.Escape:
                    DialogResult = DialogResult.Cancel;
                    Close();
                    e.Handled = true;
                    break;
            }
        }

        private void EnsureVisible(int idx)
        {
            if (idx < _scrollOffset) _scrollOffset = idx;
            else if (idx >= _scrollOffset + MaxVisible) _scrollOffset = idx - MaxVisible + 1;
        }

        private void CommitSelection()
        {
            if (_selectedIndex < 0 || _selectedIndex >= _filtered.Count) return;
            SelectedTemplate = _filtered[_selectedIndex];
            DialogResult     = DialogResult.OK;
            Close();
        }

        private static bool IsDark(Color c) => c.GetBrightness() < 0.5;
        private static Color Tc(Color refColor, Color lightColor) => IsDark(refColor)
            ? ColorUtils.MapSystemColor(lightColor)
            : lightColor;
    }
}
