using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Runtime
{
    /// <summary>
    /// An animated slide panel that reveals an auto-hidden <see cref="DockPanel"/> content area.
    /// One instance is shared per <see cref="AutoHideStrip"/> (one per edge).
    ///
    /// Animation model (follows DockPanelSuite AutoHideWindowControl.AnimateWindow):
    /// - Total animation time: ~100 ms (ANIMATE_TIME), 10 ms tick → ~10 steps.
    /// - Slide direction is determined by <see cref="Edge"/>:
    ///     Left  → grow right  (X fixed, Width grows)
    ///     Right → grow left   (X shrinks, Width grows)
    ///     Top   → grow down   (Y fixed, Height grows)
    ///     Bottom→ grow up     (Y shrinks, Height grows)
    /// - Resize grip on the inner edge follows DockPanelSuite separator sizing.
    ///
    /// Reference files:
    ///   dockpanelsuite-master\WinFormsUI\Docking\DockPanel.AutoHideWindow.cs  (AnimateWindow)
    ///   Krypton.Docking\Control Docking\KryptonAutoHiddenSlidePanel.cs (concept)
    /// </summary>
    [ToolboxItem(false)]
    [DesignerCategory("code")]
    [DesignTimeVisible(false)]
    public class AutoHideSlidePanel : Panel
    {
        private const int AnimateTime    = 100;
        private const int AnimateTickMs  = 10;
        private const int AnimateSteps   = AnimateTime / AnimateTickMs;

        /// <summary>Thickness of the resize separator in pixels.</summary>
        private const int SeparatorSize  = 5;
        /// <summary>Minimum slide extent allowed when resizing.</summary>
        private const int MinSlideExtent = 80;

        private readonly DockPosition _edge;
        private DockPanel _hostedPanel;
        private Timer _animTimer;
        private bool _slidingIn;
        private int  _step;
        private int  _targetSize;
        private DockingThemeColors _themeColors = DockingThemeColors.Default;

        // Resize grip state
        private bool _resizeGripArmed;
        private bool _resizeActive;
        private Point _resizeDragOrigin;
        private int   _resizeStartSize;

        /// <summary>Raised when the user drags the separator to resize the slide panel.</summary>
        public event EventHandler<SeparatorResizeEventArgs> SeparatorResize;

        public AutoHideSlidePanel(DockPosition edge)
        {
            _edge = edge;

            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint  |
                     ControlStyles.ResizeRedraw, true);

            BorderStyle = BorderStyle.FixedSingle;
            Visible     = false;
            BackColor   = _themeColors.SlidePanelBackColor;
            Padding     = ComputeSeparatorPadding();

            _animTimer = new Timer { Interval = AnimateTickMs };
            _animTimer.Tick += OnAnimTick;

            ApplyEdgeDock();
        }

        public DockPosition Edge => _edge;
        public DockPanel HostedPanel => _hostedPanel;

        internal void ApplyDockingTheme(DockingThemeColors colors)
        {
            _themeColors = colors ?? DockingThemeColors.Default;
            BackColor = _themeColors.SlidePanelBackColor;
            ForeColor = _themeColors.PanelForeColor;
            _hostedPanel?.ApplyDockingTheme(_themeColors);
            Invalidate();
        }

        public void Show(DockPanel panel)
        {
            if (panel == null) return;

            if (_hostedPanel != null && _hostedPanel != panel)
            {
                Controls.Remove(_hostedPanel);
                _hostedPanel.Visible = false;
            }

            _hostedPanel  = panel;
            _hostedPanel.ApplyDockingTheme(_themeColors);
            _targetSize   = IsVertical ? panel.PreferredWidth : panel.PreferredHeight;
            if (_targetSize <= 0) _targetSize = 200;

            if (!Controls.Contains(panel))
            {
                panel.Dock    = DockStyle.Fill;
                panel.Visible = true;
                Controls.Add(panel);
            }

            SetSizeToZero();
            Visible    = true;
            BringToFront();

            _slidingIn = true;
            _step      = 0;
            _animTimer.Start();
        }

        public new void Hide()
        {
            if (!Visible) return;
            _slidingIn = false;
            _step      = 0;
            _animTimer.Start();
        }

        private bool IsVertical => (_edge == DockPosition.Left || _edge == DockPosition.Right);

        private Padding ComputeSeparatorPadding()
        {
            return _edge switch
            {
                DockPosition.Left   => new Padding(0, 0, SeparatorSize, 0),
                DockPosition.Right  => new Padding(SeparatorSize, 0, 0, 0),
                DockPosition.Top    => new Padding(0, 0, 0, SeparatorSize),
                DockPosition.Bottom => new Padding(0, SeparatorSize, 0, 0),
                _                   => Padding.Empty
            };
        }

        private Rectangle SeparatorRect
        {
            get
            {
                return _edge switch
                {
                    DockPosition.Left   => new Rectangle(Width - SeparatorSize, 0, SeparatorSize, Height),
                    DockPosition.Right  => new Rectangle(0, 0, SeparatorSize, Height),
                    DockPosition.Top    => new Rectangle(0, Height - SeparatorSize, Width, SeparatorSize),
                    DockPosition.Bottom => new Rectangle(0, 0, Width, SeparatorSize),
                    _                   => Rectangle.Empty
                };
            }
        }

        private Cursor SeparatorCursor
        {
            get
            {
                return _edge switch
                {
                    DockPosition.Left   => Cursors.SizeWE,
                    DockPosition.Right  => Cursors.SizeWE,
                    DockPosition.Top    => Cursors.SizeNS,
                    DockPosition.Bottom => Cursors.SizeNS,
                    _                   => Cursors.Default
                };
            }
        }

        private void ApplyEdgeDock()
        {
            switch (_edge)
            {
                case DockPosition.Left:
                    Dock  = DockStyle.Left;
                    Width = 0;
                    break;
                case DockPosition.Right:
                    Dock  = DockStyle.Right;
                    Width = 0;
                    break;
                case DockPosition.Top:
                    Dock   = DockStyle.Top;
                    Height = 0;
                    break;
                case DockPosition.Bottom:
                    Dock   = DockStyle.Bottom;
                    Height = 0;
                    break;
            }
        }

        private void SetSizeToZero()
        {
            if (IsVertical) Width  = 0;
            else            Height = 0;
        }

        private int CurrentSize
        {
            get => IsVertical ? Width  : Height;
            set
            {
                if (IsVertical) Width  = value;
                else            Height = value;
            }
        }

        // ── Animation tick ────────────────────────────────────────────────────────

        private void OnAnimTick(object sender, EventArgs e)
        {
            _step++;
            int newSize;

            if (_slidingIn)
            {
                newSize = (_targetSize * _step) / AnimateSteps;
                if (_step >= AnimateSteps || newSize >= _targetSize)
                {
                    newSize = _targetSize;
                    _animTimer.Stop();
                }
            }
            else
            {
                newSize = _targetSize - (_targetSize * _step) / AnimateSteps;
                if (_step >= AnimateSteps || newSize <= 0)
                {
                    newSize = 0;
                    _animTimer.Stop();
                    Visible = false;
                    if (_hostedPanel != null)
                    {
                        Controls.Remove(_hostedPanel);
                        _hostedPanel.Visible = false;
                        _hostedPanel.Dock = DockStyle.None;
                        _hostedPanel = null;
                    }
                }
            }

            CurrentSize = newSize;
            Parent?.PerformLayout();
        }

        // ── Resize grip ────────────────────────────────────────────────────────────

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button != MouseButtons.Left)
                return;

            var sepRect = SeparatorRect;
            if (sepRect.Contains(e.Location))
            {
                _resizeGripArmed = true;
                _resizeActive = true;
                _resizeDragOrigin = PointToScreen(e.Location);
                _resizeStartSize = CurrentSize;
                Capture = true;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_resizeActive)
            {
                var screenNow = PointToScreen(e.Location);
                int delta = IsVertical
                    ? screenNow.X - _resizeDragOrigin.X
                    : screenNow.Y - _resizeDragOrigin.Y;

                int newSize = Math.Max(MinSlideExtent, _resizeStartSize + delta);
                CurrentSize = newSize;

                if (_hostedPanel != null)
                {
                    if (IsVertical) _hostedPanel.PreferredWidth  = newSize;
                    else            _hostedPanel.PreferredHeight = newSize;
                }

                Parent?.PerformLayout();
                return;
            }

            var sepRect = SeparatorRect;
            if (sepRect.Contains(e.Location))
            {
                _resizeGripArmed = true;
                Cursor = SeparatorCursor;
            }
            else
            {
                _resizeGripArmed = false;
                Cursor = Cursors.Default;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (_resizeActive)
            {
                _resizeActive = false;
                Capture = false;

                if (_hostedPanel != null)
                {
                    int newSize = CurrentSize;
                    if (IsVertical) _hostedPanel.PreferredWidth  = newSize;
                    else            _hostedPanel.PreferredHeight = newSize;
                }

                SeparatorResize?.Invoke(this, new SeparatorResizeEventArgs(
                    _hostedPanel,
                    _edge == DockPosition.Left || _edge == DockPosition.Right
                        ? new Rectangle(0, 0, CurrentSize, Height)
                        : new Rectangle(0, 0, Width, CurrentSize)));
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (!_resizeActive)
            {
                _resizeGripArmed = false;
                Cursor = Cursors.Default;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var sepRect = SeparatorRect;
            if (sepRect.Width <= 0 || sepRect.Height <= 0)
                return;

            using (var brush = new SolidBrush(_resizeGripArmed
                ? _themeColors.AccentColor
                : _themeColors.SplitterBackColor))
            {
                e.Graphics.FillRectangle(brush, sepRect);
            }

            // Small grip dots in the center of the separator.
            int centerX = sepRect.X + sepRect.Width / 2;
            int centerY = sepRect.Y + sepRect.Height / 2;
            using (var pen = new Pen(_themeColors.BorderColor))
            {
                if (_edge == DockPosition.Left || _edge == DockPosition.Right)
                {
                    for (int y = centerY - 8; y <= centerY + 8; y += 4)
                        e.Graphics.DrawLine(pen, centerX - 1, y, centerX + 1, y);
                }
                else
                {
                    for (int x = centerX - 8; x <= centerX + 8; x += 4)
                        e.Graphics.DrawLine(pen, x, centerY - 1, x, centerY + 1);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _animTimer.Stop();
                _animTimer.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
