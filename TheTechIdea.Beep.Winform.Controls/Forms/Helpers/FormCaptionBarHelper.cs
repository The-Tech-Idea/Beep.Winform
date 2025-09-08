using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Forms.Helpers
{
    internal sealed class FormCaptionBarHelper
    {
        public delegate void PaddingAdjuster(ref Padding padding); // must match BeepiForm signature
        private readonly IBeepModernFormHost _host;
        private readonly FormOverlayPainterRegistry _overlayRegistry;
        private readonly Action<PaddingAdjuster> _registerPaddingProvider;
        public bool ShowCaptionBar { get; set; } = true;
        public int CaptionHeight { get; set; } = 36;
        public bool ShowSystemButtons { get; set; } = true;
        public bool EnableCaptionGradient { get; set; } = true;
        public Rectangle CloseRect { get; private set; }
        public Rectangle MaxRect { get; private set; }
        public Rectangle MinRect { get; private set; }
        private bool _hoverClose, _hoverMax, _hoverMin;
        private Form Form => _host.AsForm;
        private IBeepTheme Theme => _host.CurrentTheme;
        public FormCaptionBarHelper(IBeepModernFormHost host, FormOverlayPainterRegistry overlayRegistry, Action<PaddingAdjuster> registerPaddingProvider)
        {
            _host = host ?? throw new ArgumentNullException(nameof(host));
            _overlayRegistry = overlayRegistry ?? throw new ArgumentNullException(nameof(overlayRegistry));
            _registerPaddingProvider = registerPaddingProvider ?? throw new ArgumentNullException(nameof(registerPaddingProvider));
            _overlayRegistry.Add(PaintOverlay);
            _registerPaddingProvider((ref Padding p) => { if (ShowCaptionBar) p.Top += CaptionHeight; });
        }
        public bool IsPointInSystemButtons(Point p) => ShowSystemButtons && (CloseRect.Contains(p) || MaxRect.Contains(p) || MinRect.Contains(p));
        public bool IsCursorOverSystemButton => IsPointInSystemButtons(Form.PointToClient(Cursor.Position));
        public void OnMouseMove(MouseEventArgs e)
        {
            if (!ShowCaptionBar || !ShowSystemButtons) return;
            var prev = (_hoverClose, _hoverMax, _hoverMin);
            _hoverClose = CloseRect.Contains(e.Location);
            _hoverMax = MaxRect.Contains(e.Location);
            _hoverMin = MinRect.Contains(e.Location);
            if (prev != (_hoverClose, _hoverMax, _hoverMin))
            {
                var invalid = Rectangle.Union(CloseRect, MaxRect);
                invalid = Rectangle.Union(invalid, MinRect);
                Form.Invalidate(invalid);
            }
        }
        public void OnMouseLeave()
        {
            if (_hoverClose || _hoverMax || _hoverMin)
            {
                _hoverClose = _hoverMax = _hoverMin = false;
                var invalid = Rectangle.Union(CloseRect, MaxRect);
                invalid = Rectangle.Union(invalid, MinRect);
                Form.Invalidate(invalid);
            }
        }
        public void OnMouseDown(MouseEventArgs e)
        {
            if (!ShowCaptionBar || !ShowSystemButtons) return;
            if (CloseRect.Contains(e.Location)) Form.Close();
            else if (MaxRect.Contains(e.Location)) ToggleMaximize();
            else if (MinRect.Contains(e.Location)) Form.WindowState = FormWindowState.Minimized;
        }
        private void ToggleMaximize() => Form.WindowState = Form.WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
        private void LayoutButtons()
        {
            if (!ShowCaptionBar || !ShowSystemButtons)
            { CloseRect = MaxRect = MinRect = Rectangle.Empty; return; }
            float scale = Form.DeviceDpi / 96f;
            int btnSize = Math.Max(24, (int)(CaptionHeight - 8 * scale));
            int top = (int)(4 * scale);
            CloseRect = new Rectangle(Form.ClientSize.Width - btnSize - 8, top, btnSize, btnSize);
            MaxRect = new Rectangle(CloseRect.Left - btnSize - 6, top, btnSize, btnSize);
            MinRect = new Rectangle(MaxRect.Left - btnSize - 6, top, btnSize, btnSize);
        }
        private void PaintOverlay(Graphics g)
        {
            if (!ShowCaptionBar) return;
            LayoutButtons();
            var rect = new Rectangle(0, 0, Form.ClientSize.Width, CaptionHeight);
            if (rect.Width <= 0 || rect.Height <= 0) return;
            if (EnableCaptionGradient && Theme != null && Theme.AppBarBackColor != Color.Empty)
            {
                using var brush = new LinearGradientBrush(rect, ControlPaint.Light(Theme.AppBarBackColor, .05f), ControlPaint.Dark(Theme.AppBarBackColor, .05f), LinearGradientMode.Vertical);
                g.FillRectangle(brush, rect);
            }
            else
            {
                using var b = new SolidBrush(Theme?.AppBarBackColor ?? SystemColors.ControlDark);
                g.FillRectangle(b, rect);
            }
            using (var sf = new StringFormat { LineAlignment = StringAlignment.Center })
            using (var titleBrush = new SolidBrush(Theme?.AppBarTitleForeColor ?? Form.ForeColor))
            {
                var titleRect = new Rectangle(10, 0, rect.Width - 160, rect.Height);
                g.DrawString(Form.Text, Form.Font, titleBrush, titleRect, sf);
            }
            if (ShowSystemButtons)
            {
                using var pen = new Pen(Theme?.AppBarButtonForeColor ?? Form.ForeColor, 1.5f) { Alignment = PenAlignment.Center };
                float scale = Form.DeviceDpi / 96f;
                if (_hoverMin) { using var hb = new SolidBrush(Theme?.ButtonHoverBackColor ?? Color.LightGray); g.FillRectangle(hb, MinRect); }
                CaptionGlyphProvider.DrawMinimize(g, pen, MinRect, scale);
                if (_hoverMax) { using var hb = new SolidBrush(Theme?.ButtonHoverBackColor ?? Color.LightGray); g.FillRectangle(hb, MaxRect); }
                if (Form.WindowState == FormWindowState.Maximized) CaptionGlyphProvider.DrawRestore(g, pen, MaxRect, scale); else CaptionGlyphProvider.DrawMaximize(g, pen, MaxRect, scale);
                if (_hoverClose) { using var hb = new SolidBrush(Theme?.ButtonErrorBackColor ?? Color.IndianRed); g.FillRectangle(hb, CloseRect); }
                CaptionGlyphProvider.DrawClose(g, pen, CloseRect, scale);
            }
        }
    }
}
