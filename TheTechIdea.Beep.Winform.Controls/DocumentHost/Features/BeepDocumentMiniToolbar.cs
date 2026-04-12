// BeepDocumentMiniToolbar.cs
// Context-aware floating mini toolbar that appears when the mouse hovers over
// a document panel inside BeepDocumentHost.
// It fades in over 150 ms and fades out when the pointer leaves.
// Actions are driven by a list of MiniToolbarAction descriptors so callers
// control what buttons appear (document-type-aware).
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost.Features
{
    /// <summary>One button on the mini toolbar.</summary>
    public sealed class MiniToolbarAction
    {
        /// <summary>Unicode glyph displayed inside the button (Segoe UI Symbol).</summary>
        public string  Glyph       { get; init; } = "●";
        public string  ToolTipText { get; init; } = string.Empty;
        public string  CommandId   { get; init; } = string.Empty;
        public bool    Enabled     { get; init; } = true;
        public Action? Execute     { get; init; }
    }

    /// <summary>
    /// Borderless floating toolbar that appears in the top-right corner of a
    /// document panel on hover.  Fades in/out via an opacity animation timer.
    /// </summary>
    public sealed class BeepDocumentMiniToolbar : Form
    {
        // ── Layout constants ──────────────────────────────────────────────────

        private const int ButtonSize    = 24;
        private const int ButtonPad     = 2;
        private const int ToolbarHeight = 28;

        // ── Fields ────────────────────────────────────────────────────────────

        private readonly List<MiniToolbarAction> _actions;
        private readonly Timer   _fadeTimer  = new() { Interval = 16 };
        private readonly ToolTip _tip        = new();
        private double  _targetOpacity       = 1.0;
        private int     _hoveredIndex        = -1;

        // ── Constructor ───────────────────────────────────────────────────────

        public BeepDocumentMiniToolbar(IEnumerable<MiniToolbarAction> actions)
        {
            _actions = new List<MiniToolbarAction>(actions);

            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar   = false;
            TopMost         = true;
            Opacity         = 0;
            BackColor       = Color.FromArgb(45, 45, 48);

            int w = _actions.Count * (ButtonSize + ButtonPad) + ButtonPad;
            Size = new Size(w, ToolbarHeight);

            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint  |
                     ControlStyles.UserPaint, true);

            _fadeTimer.Tick += OnFadeTick;

            MouseMove  += (_, e) => { _hoveredIndex = HitButton(e.Location); Invalidate(); };
            MouseLeave += (_, _) => { _hoveredIndex = -1; Invalidate(); FadeOut(); };
            MouseClick += OnMouseClick;

            // Build per-button tooltips
            for (int i = 0; i < _actions.Count; i++)
            {
                // ToolTip requires a control — we synthesise using the form itself
                // (advanced per-rect tooltips would need native UxTheme; this is sufficient)
                if (!string.IsNullOrEmpty(_actions[i].ToolTipText))
                    _tip.SetToolTip(this, _actions[i].ToolTipText);
            }
        }

        // ── Public API ────────────────────────────────────────────────────────

        /// <summary>Updates the action list and resizes the toolbar.</summary>
        public void SetActions(IEnumerable<MiniToolbarAction> actions)
        {
            _actions.Clear();
            _actions.AddRange(actions);
            int w = _actions.Count * (ButtonSize + ButtonPad) + ButtonPad;
            Width = w;
            Invalidate();
        }

        /// <summary>Shows the toolbar anchored to the top-right of <paramref name="anchor"/>.</summary>
        public void ShowAt(Control anchor)
        {
            if (anchor == null) return;
            var screen = anchor.RectangleToScreen(anchor.ClientRectangle);
            int x = screen.Right  - Width  - 4;
            int y = screen.Top    + 4;
            Location = new Point(x, y);
            FadeIn();
        }

        public void FadeIn()
        {
            _targetOpacity = 1.0;
            if (!Visible) Show();
            _fadeTimer.Start();
        }

        public void FadeOut()
        {
            _targetOpacity = 0.0;
            _fadeTimer.Start();
        }

        // ── Fade animation ────────────────────────────────────────────────────

        private void OnFadeTick(object? sender, EventArgs e)
        {
            const double step = 0.08;
            if (Math.Abs(Opacity - _targetOpacity) < step)
            {
                Opacity = _targetOpacity;
                _fadeTimer.Stop();
                if (Opacity <= 0) Hide();
                return;
            }
            Opacity += Opacity < _targetOpacity ? step : -step;
        }

        // ── Painting ──────────────────────────────────────────────────────────

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(BackColor);
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            var glyphFont = new Font("Segoe UI Symbol", 11f, FontStyle.Regular);

            for (int i = 0; i < _actions.Count; i++)
            {
                var act  = _actions[i];
                var rect = ButtonRect(i);

                // Hover highlight
                if (i == _hoveredIndex && act.Enabled)
                    using (var hb = new SolidBrush(Color.FromArgb(70, 255, 255, 255)))
                        g.FillRectangle(hb, rect);

                var glyphColor = act.Enabled ? Color.White : Color.Gray;
                using var fb = new SolidBrush(glyphColor);
                var sf = new StringFormat { Alignment = StringAlignment.Center,
                                            LineAlignment = StringAlignment.Center };
                g.DrawString(act.Glyph, glyphFont, fb, rect, sf);
            }

            glyphFont.Dispose();
        }

        // ── Mouse ─────────────────────────────────────────────────────────────

        private void OnMouseClick(object? sender, MouseEventArgs e)
        {
            int idx = HitButton(e.Location);
            if (idx < 0 || idx >= _actions.Count) return;
            var act = _actions[idx];
            if (act.Enabled) act.Execute?.Invoke();
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private Rectangle ButtonRect(int index)
        {
            int x = ButtonPad + index * (ButtonSize + ButtonPad);
            int y = (ToolbarHeight - ButtonSize) / 2;
            return new Rectangle(x, y, ButtonSize, ButtonSize);
        }

        private int HitButton(Point pt)
        {
            for (int i = 0; i < _actions.Count; i++)
                if (ButtonRect(i).Contains(pt)) return i;
            return -1;
        }

        // ── Dispose ───────────────────────────────────────────────────────────

        protected override void Dispose(bool disposing)
        {
            if (disposing) { _fadeTimer.Dispose(); _tip.Dispose(); }
            base.Dispose(disposing);
        }

        // Keep the toolbar non-activating so the document retains focus
        protected override bool ShowWithoutActivation => true;
        protected override CreateParams CreateParams
        {
            get
            {
                const int WS_EX_NOACTIVATE = 0x08000000;
                var cp = base.CreateParams;
                cp.ExStyle |= WS_EX_NOACTIVATE;
                return cp;
            }
        }
    }
}
