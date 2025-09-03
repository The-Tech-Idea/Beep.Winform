using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Windows.Forms.VisualStyles;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepiForm
    {
        private bool _showCaptionBar = true;
        private int _captionHeight = 36;
        private bool _showSystemButtons = true;
        private bool _enableCaptionGradient = true;

        [Category("Beep Caption")]
        [DefaultValue(true)]
        public bool ShowCaptionBar
        {
            get => _showCaptionBar;
            set { _showCaptionBar = value; Invalidate(); }
        }

        [Category("Beep Caption")]
        [DefaultValue(36)]
        public int CaptionHeight
        {
            get => _captionHeight;
            set { _captionHeight = Math.Max(24, value); Invalidate(); }
        }

        [Category("Beep Caption")]
        [DefaultValue(true)]
        public bool ShowSystemButtons
        {
            get => _showSystemButtons;
            set { _showSystemButtons = value; Invalidate(); }
        }

        [Category("Beep Caption")]
        [DefaultValue(true)]
        public bool EnableCaptionGradient
        {
            get => _enableCaptionGradient;
            set { _enableCaptionGradient = value; Invalidate(); }
        }

        private Rectangle _btnClose, _btnMax, _btnMin;
        private bool _hoverClose, _hoverMax, _hoverMin;

        partial void InitializeCaptionFeature()
        {
            RegisterPaddingProvider((ref Padding p) => { if (_showCaptionBar) p.Top += _captionHeight; });
            RegisterMouseMoveHandler(Caption_OnMouseMove);
            RegisterMouseLeaveHandler(Caption_OnMouseLeave);
            RegisterMouseDownHandler(Caption_OnMouseDown);
            RegisterOverlayPainter(Caption_OnPaintOverlay);
        }

        private void Caption_OnMouseMove(MouseEventArgs e)
        {
            if (!_showCaptionBar || !_showSystemButtons) return;
            var prev = (_hoverClose, _hoverMax, _hoverMin);
            _hoverClose = _btnClose.Contains(e.Location);
            _hoverMax = _btnMax.Contains(e.Location);
            _hoverMin = _btnMin.Contains(e.Location);
            if (prev != (_hoverClose, _hoverMax, _hoverMin))
            {
                var invalid = Rectangle.Union(_btnClose, _btnMax);
                invalid = Rectangle.Union(invalid, _btnMin);
                Invalidate(invalid);
            }
        }

        private void Caption_OnMouseLeave()
        {
            if (_hoverClose || _hoverMax || _hoverMin)
            {
                _hoverClose = _hoverMax = _hoverMin = false;
                var invalid = Rectangle.Union(_btnClose, _btnMax);
                invalid = Rectangle.Union(invalid, _btnMin);
                Invalidate(invalid);
            }
        }

        private void Caption_OnMouseDown(MouseEventArgs e)
        {
            if (!_showCaptionBar || !_showSystemButtons) return;
            if (_btnClose.Contains(e.Location)) Close();
            else if (_btnMax.Contains(e.Location)) ToggleMaximize();
            else if (_btnMin.Contains(e.Location)) WindowState = FormWindowState.Minimized;
        }

        private void Caption_OnPaintOverlay(Graphics g)
        {
            if (!_showCaptionBar) return;
            var rect = new Rectangle(0, 0, Width, _captionHeight);

            if (_enableCaptionGradient && _currentTheme != null)
            {
                using var br = new LinearGradientBrush(rect, _currentTheme.AppBarGradiantStartColor, _currentTheme.AppBarGradiantEndColor, _currentTheme.AppBarGradiantDirection);
                g.FillRectangle(br, rect);
            }
            else
            {
                using var br = new SolidBrush(_currentTheme?.AppBarBackColor ?? SystemColors.ControlDark);
                g.FillRectangle(br, rect);
            }

            using (var sf = new StringFormat { LineAlignment = StringAlignment.Center })
            using (var titleBrush = new SolidBrush(_currentTheme?.AppBarTitleForeColor ?? ForeColor))
            {
                var titleRect = new Rectangle(10, 0, Width - 120, _captionHeight);
                g.DrawString(Text, Font, titleBrush, titleRect, sf);
            }

            if (_showSystemButtons)
            {
                float scale = DeviceDpi / 96f;
                int btnSize = (int)(Math.Max(24, (_currentTheme?.ButtonStyle?.FontStyle == FontStyle.Bold ? _captionHeight - 6 : _captionHeight - 8)));
                int top = (int)(4 * scale);
                _btnClose = new Rectangle(Width - btnSize - 8, top, btnSize, btnSize);
                _btnMax = new Rectangle(_btnClose.Left - btnSize - 6, top, btnSize, btnSize);
                _btnMin = new Rectangle(_btnMax.Left - btnSize - 6, top, btnSize, btnSize);

                using var p = new Pen(_currentTheme?.AppBarButtonForeColor ?? ForeColor, 1.5f) { Alignment = PenAlignment.Center };
                if (_hoverMin)
                {
                    using var b = new SolidBrush(_currentTheme?.ButtonHoverBackColor ?? Color.LightGray);
                    g.FillRectangle(b, _btnMin);
                }
                CaptionGlyphProvider.DrawMinimize(g, p, _btnMin, scale);

                if (_hoverMax)
                {
                    using var b = new SolidBrush(_currentTheme?.ButtonHoverBackColor ?? Color.LightGray);
                    g.FillRectangle(b, _btnMax);
                }
                if (WindowState == FormWindowState.Maximized)
                    CaptionGlyphProvider.DrawRestore(g, p, _btnMax, scale);
                else
                    CaptionGlyphProvider.DrawMaximize(g, p, _btnMax, scale);

                if (_hoverClose)
                {
                    using var b = new SolidBrush(_currentTheme?.ButtonErrorBackColor ?? Color.IndianRed);
                    g.FillRectangle(b, _btnClose);
                }
                CaptionGlyphProvider.DrawClose(g, p, _btnClose, scale);
            }
        }

        private enum CaptionGlyph { Minimize, Maximize, Restore, Close }
    }
}
