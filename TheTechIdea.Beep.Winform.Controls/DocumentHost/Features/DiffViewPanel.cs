// DiffViewPanel.cs
// Side-by-side document diff viewer panel.
// Left pane = original (A), right pane = modified (B).
// Lines are synchronised in scroll; changed lines are highlighted via
// DiffHighlightPainter.  Supports export to plain text.
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost.Features
{
    /// <summary>
    /// Owner-draw panel that renders a side-by-side diff of two text strings.
    /// Drop this control into any container, set <see cref="SetDocuments"/>,
    /// and it renders immediately.
    /// </summary>
    public sealed class DiffViewPanel : Panel
    {
        // ── Fields ────────────────────────────────────────────────────────────

        private DiffResult?           _result;
        private readonly DiffHighlightPainter _painter = new();
        private int  _scrollOffset;         // first visible row index
        private int  _selectedRow = -1;
        private bool _synchronizedScroll   = true;

        // Titles shown above each pane
        public string TitleA { get; set; } = "Original";
        public string TitleB { get; set; } = "Modified";

        public int RowHeight
        {
            get => _painter.RowHeight;
            set { _painter.RowHeight = value; Invalidate(); }
        }

        public DiffResult? Result => _result;

        // ── Constructor ───────────────────────────────────────────────────────

        public DiffViewPanel()
        {
            DoubleBuffered = true;
            AutoScroll     = false;
            TabStop        = true;
        }

        // ── Public API ────────────────────────────────────────────────────────

        /// <summary>Compare two texts and refresh the view.</summary>
        public void SetDocuments(string textA, string textB)
        {
            _result       = DocumentComparer.Compare(textA, textB);
            _scrollOffset = 0;
            _selectedRow  = -1;
            Invalidate();
        }

        /// <summary>Exports the diff as a unified-diff text file.</summary>
        public void ExportToFile(string filePath)
        {
            if (_result == null) return;
            using var sw = new StreamWriter(filePath, false, Encoding.UTF8);
            foreach (var line in _result.Lines)
            {
                string prefix = line.Kind switch
                {
                    DiffLineKind.Added   => "+ ",
                    DiffLineKind.Removed => "- ",
                    _                    => "  "
                };
                sw.WriteLine(prefix + line.Text);
            }
        }

        // ── Painting ──────────────────────────────────────────────────────────

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.Clear(SystemColors.Window);

            if (_result == null)
            {
                using var b = new SolidBrush(SystemColors.GrayText);
                g.DrawString("No documents compared.", Font, b, ClientRectangle);
                return;
            }

            int headerH   = _painter.RowHeight + 4;
            int totalRows = _result.Lines.Count;
            int halfW     = ClientWidth / 2;
            int rowH      = _painter.RowHeight;

            // Draw pane titles
            DrawPaneHeader(g, TitleA, 0,     halfW,  headerH);
            DrawPaneHeader(g, TitleB, halfW, halfW,  headerH);

            // Draw divider
            _painter.PaintSplitDivider(g, halfW, ClientHeight);

            int y    = headerH;
            int rows = VisibleRows(headerH);

            for (int i = _scrollOffset; i < Math.Min(_scrollOffset + rows, totalRows); i++)
            {
                var line = _result.Lines[i];
                bool sel = i == _selectedRow;

                // Left pane (side A)
                g.SetClip(new Rectangle(0, y, halfW, rowH));
                _painter.PaintRow(g, line, y, halfW, sel);
                g.ResetClip();

                // Right pane (side B) — swap kind for removed lines
                g.SetClip(new Rectangle(halfW, y, halfW, rowH));
                var bLine = line.Kind == DiffLineKind.Removed
                    ? line with { Kind = DiffLineKind.Unchanged, Text = string.Empty }
                    : line.Kind == DiffLineKind.Added
                        ? line with { Kind = DiffLineKind.Added }
                        : line;
                _painter.PaintRow(g, bLine, y, halfW, sel);
                g.TranslateTransform(halfW, 0);
                g.ResetClip();
                g.ResetTransform();

                y += rowH;
            }
        }

        private void DrawPaneHeader(Graphics g, string title, int x, int w, int h)
        {
            var rect = new Rectangle(x, 0, w, h);
            using var backBr = new SolidBrush(SystemColors.ControlDark);
            g.FillRectangle(backBr, rect);
            using var textBr = new SolidBrush(SystemColors.ControlLightLight);
            var sf = new StringFormat { Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center };
            g.DrawString(title, Font, textBr, rect, sf);
        }

        // ── Mouse wheel scroll ────────────────────────────────────────────────

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            int delta = e.Delta > 0 ? -3 : 3;
            _scrollOffset = Math.Max(0,
                Math.Min(_result?.Lines.Count - 1 ?? 0, _scrollOffset + delta));
            Invalidate();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (_result == null) return;
            switch (e.KeyCode)
            {
                case Keys.Up:   _scrollOffset = Math.Max(0, _scrollOffset - 1); break;
                case Keys.Down: _scrollOffset = Math.Min(_result.Lines.Count - 1, _scrollOffset + 1); break;
                case Keys.PageUp:   _scrollOffset = Math.Max(0, _scrollOffset - VisibleRows(0)); break;
                case Keys.PageDown: _scrollOffset = Math.Min(_result.Lines.Count - 1, _scrollOffset + VisibleRows(0)); break;
                case Keys.Home: _scrollOffset = 0; break;
                case Keys.End:  _scrollOffset = Math.Max(0, _result.Lines.Count - VisibleRows(0)); break;
                default: return;
            }
            e.Handled = true;
            Invalidate();
        }

        protected override bool IsInputKey(Keys keyData)
        {
            return keyData is Keys.Up or Keys.Down or Keys.PageUp or Keys.PageDown
                or Keys.Home or Keys.End || base.IsInputKey(keyData);
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private int ClientWidth  => Math.Max(1, Width);
        private int ClientHeight => Math.Max(1, Height);

        private int VisibleRows(int headerHeight)
            => Math.Max(1, (ClientHeight - headerHeight) / Math.Max(1, _painter.RowHeight));
    }
}
