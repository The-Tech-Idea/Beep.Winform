using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Docking.Layoutmanagers;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;
using TheTechIdea.Beep.Winform.Controls.Docking.Painters;
using TheTechIdea.Beep.Winform.Controls.Docking.Painters.Caption;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Runtime
{
    /// <summary>
    /// A borderless, themed floating tool-window that hosts a single <see cref="DockPanel"/>.
    /// The window owns a themed caption strip (painted via the shared <see cref="CaptionRenderer"/>)
    /// so the floating chrome matches the docked look. The hosted panel's own caption is suppressed
    /// (<see cref="DockPanel.ShowCaption"/> = false) to avoid duplication.
    ///
    /// Interaction:
    /// - Drag the caption to move (native move via WM_NCLBUTTONDOWN → keeps snapping smooth).
    /// - Double-click the caption to re-dock.
    /// - Close button re-docks/closes via the manager.
    /// - Resize from the thin frame around the panel (WM_NCHITTEST border codes).
    /// - Edge-snaps to the owner form's screen rectangle while moving.
    /// </summary>
    [ToolboxItem(false)]
    [DesignerCategory("code")]
    [DesignTimeVisible(false)]
    public class FloatWindow : Form
    {
        // ── Win32 ───────────────────────────────────────────────────────────
        private const int WM_NCHITTEST     = 0x0084;
        private const int WM_NCLBUTTONDOWN = 0x00A1;
        private const int WM_EXITSIZEMOVE  = 0x0232;
        private const int HTCLIENT     = 1;
        private const int HTCAPTION    = 2;
        private const int HTLEFT       = 10;
        private const int HTRIGHT      = 11;
        private const int HTTOP        = 12;
        private const int HTTOPLEFT    = 13;
        private const int HTTOPRIGHT   = 14;
        private const int HTBOTTOM     = 15;
        private const int HTBOTTOMLEFT = 16;
        private const int HTBOTTOMRIGHT= 17;

        // ── Layout constants ────────────────────────────────────────────────
        private const int CaptionHeight = 28;
        private const int ResizeMargin  = 5;
        private const int SnapThreshold = 14;

        // ── Fields ──────────────────────────────────────────────────────────
        private DockPanel _panel;
        private bool _redocking;
        private DockingThemeColors _themeColors = DockingThemeColors.Default;
        private readonly CaptionLayoutManager _captionLayout = new CaptionLayoutManager();

        /// <summary>Control style driving caption rendering. Set by the manager.</summary>
        internal BeepControlStyle ControlStyle { get; set; } = BeepControlStyle.Material3;

        // ── Events ──────────────────────────────────────────────────────────

        /// <summary>Raised when the user re-docks the panel (double-click caption or close).</summary>
        public event EventHandler<DockPanel> PanelRedocked;

        /// <summary>Raised after a native caption move or resize loop ends (WM_EXITSIZEMOVE).</summary>
        internal event EventHandler MoveOperationEnded;

        // ── Constructors ─────────────────────────────────────────────────────

        public FloatWindow(DockPanel panel, Form owner)
            : this(panel, owner, Rectangle.Empty) { }

        public FloatWindow(DockPanel panel, Form owner, Rectangle initialBounds)
        {
            _panel = panel ?? throw new ArgumentNullException(nameof(panel));

            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar   = false;
            TopMost         = true;
            TopLevel        = true;
            ShowIcon        = false;
            Text            = panel.Title;
            DoubleBuffered  = true;
            Padding         = new Padding(ResizeMargin, CaptionHeight, ResizeMargin, ResizeMargin);

            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.ResizeRedraw, true);

            if (owner != null)
            {
                Owner = owner;
                owner.VisibleChanged += OnOwnerVisibleChanged;
                owner.SizeChanged += OnOwnerSizeChanged;
            }

            if (initialBounds.IsEmpty)
            {
                StartPosition = FormStartPosition.WindowsDefaultLocation;
                Size = new Size(
                    panel.PreferredWidth  > 0 ? panel.PreferredWidth  + ResizeMargin * 2 : 320,
                    panel.PreferredHeight > 0 ? panel.PreferredHeight + CaptionHeight + ResizeMargin : 240);
            }
            else
            {
                StartPosition = FormStartPosition.Manual;
                Bounds = initialBounds;
            }

            // Host the panel filling the padded client area; suppress its own caption.
            panel.Parent?.Controls.Remove(panel);
            panel.ShowCaption = false;
            panel.Dock = DockStyle.Fill;
            Controls.Add(panel);
        }

        // ── Public API ───────────────────────────────────────────────────────

        public DockPanel Panel => _panel;

        internal void ApplyDockingTheme(DockingThemeColors colors)
        {
            _themeColors = colors ?? DockingThemeColors.Default;
            BackColor = _themeColors.HeaderBackColor;
            _panel?.ApplyDockingTheme(_themeColors);
            Invalidate();
        }

        // ── Caption painting ──────────────────────────────────────────────────

        private Rectangle CaptionBounds => new Rectangle(0, 0, Width, CaptionHeight);

        private IReadOnlyList<CaptionButtonKind> CaptionButtons
        {
            get
            {
                var list = new List<CaptionButtonKind>(1);
                if (_panel != null && _panel.CanClose)
                    list.Add(CaptionButtonKind.Close);
                return list;
            }
        }

        private void RecomputeCaption()
        {
            var tabs = new List<CaptionTabModel>
            {
                new CaptionTabModel
                {
                    Key = _panel?.Key,
                    Title = _panel?.Title,
                    IconPath = _panel?.IconPath,
                    IsDirty = _panel?.IsDirty ?? false,
                    IsActive = true,
                    Tag = _panel
                }
            };
            _captionLayout.Compute(Width, CaptionHeight, tabs, CaptionButtons);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            RecomputeCaption();
            var ctx = new DockingPainterContext
            {
                Colors = _themeColors,
                Style = ControlStyle,
                Bounds = CaptionBounds,
                IsDesignTime = false
            };
            DockingPainterFactory.GetRenderers(ControlStyle).Caption.Paint(e.Graphics, ctx, _captionLayout, CaptionButtons);

            // Themed 1px border around the whole window.
            using var pen = new Pen(_themeColors.TabBorderColor);
            e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
        }

        // ── Mouse — caption move / close / redock ─────────────────────────────

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button != MouseButtons.Left || e.Y > CaptionHeight)
                return;

            RecomputeCaption();
            if (_captionLayout.HitTestButton(e.Location) == CaptionButtonKind.Close)
            {
                TriggerRedock();
                return;
            }

            // Native caption move (gives smooth drag + lets LocationChanged drive snapping/guides).
            ReleaseCapture();
            SendMessage(Handle, WM_NCLBUTTONDOWN, (IntPtr)HTCAPTION, IntPtr.Zero);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            if (e.Button == MouseButtons.Left && e.Y <= CaptionHeight)
                TriggerRedock();
        }

        // ── Edge snapping ──────────────────────────────────────────────────────

        protected override void OnMove(EventArgs e)
        {
            base.OnMove(e);
            SnapToOwnerEdges();
        }

        private void SnapToOwnerEdges()
        {
            if (Owner == null || Owner.IsDisposed)
                return;

            Rectangle dock = Owner.Bounds;     // screen coordinates
            Rectangle me   = Bounds;
            int x = me.X, y = me.Y;

            if (Math.Abs(me.Left   - dock.Left)   <= SnapThreshold) x = dock.Left;
            else if (Math.Abs(me.Right  - dock.Right)  <= SnapThreshold) x = dock.Right  - me.Width;
            if (Math.Abs(me.Top    - dock.Top)    <= SnapThreshold) y = dock.Top;
            else if (Math.Abs(me.Bottom - dock.Bottom) <= SnapThreshold) y = dock.Bottom - me.Height;

            if (x != me.X || y != me.Y)
                Location = new Point(x, y);
        }

        // ── WndProc — resize-border hit testing ───────────────────────────────

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_NCHITTEST)
            {
                base.WndProc(ref m);
                if ((int)m.Result == HTCLIENT)
                {
                    var screen = new Point(m.LParam.ToInt32() & 0xFFFF, (m.LParam.ToInt32() >> 16) & 0xFFFF);
                    var p = PointToClient(screen);
                    m.Result = (IntPtr)HitTestResize(p);
                }
                return;
            }

            if (m.Msg == WM_EXITSIZEMOVE)
            {
                base.WndProc(ref m);
                MoveOperationEnded?.Invoke(this, EventArgs.Empty);
                return;
            }

            base.WndProc(ref m);
        }

        private int HitTestResize(Point p)
        {
            bool left   = p.X <= ResizeMargin;
            bool right  = p.X >= Width - ResizeMargin;
            bool top    = p.Y <= ResizeMargin;
            bool bottom = p.Y >= Height - ResizeMargin;

            if (top && left) return HTTOPLEFT;
            if (top && right) return HTTOPRIGHT;
            if (bottom && left) return HTBOTTOMLEFT;
            if (bottom && right) return HTBOTTOMRIGHT;
            if (left) return HTLEFT;
            if (right) return HTRIGHT;
            if (top) return HTTOP;
            if (bottom) return HTBOTTOM;
            return HTCLIENT;
        }

        // ── FormClosing / Dispose ──────────────────────────────────────────────

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (e.Cancel || _redocking)
                return;

            if (_panel == null)
                return;

            var panel = _panel;
            var manager = panel.Manager;
            ExtractHostedPanel();

            // User closed the float window (not double-click redock) — route through the manager
            // so PageCloseRequest can veto and the panel lands in the closed store.
            if (manager != null && manager.ContainsPanel(panel.Key))
                manager.CloseRequest(panel.Key);
            else
            {
                panel.State = DockPanelState.Closed;
                panel.OnClosed();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Owner != null)
                {
                    Owner.VisibleChanged -= OnOwnerVisibleChanged;
                    Owner.SizeChanged -= OnOwnerSizeChanged;
                }

                if (_panel != null)
                {
                    ExtractHostedPanel();
                }
            }
            base.Dispose(disposing);
        }

        // ── Owner tracking (auto-hide when owner form is minimized) ─────────────

        private void OnOwnerVisibleChanged(object sender, EventArgs e)
        {
            if (Owner == null || IsDisposed) return;
            Visible = Owner.Visible && Owner.WindowState != FormWindowState.Minimized;
        }

        private void OnOwnerSizeChanged(object sender, EventArgs e)
        {
            if (Owner == null || IsDisposed) return;
            Visible = Owner.WindowState != FormWindowState.Minimized;
        }

        // ── Helpers ────────────────────────────────────────────────────────────

        private void TriggerRedock()
        {
            if (_panel == null) return;
            _redocking = true;

            var panel = ExtractHostedPanel();
            PanelRedocked?.Invoke(this, panel);
            Close();
        }

        /// <summary>
        /// Removes the hosted panel from this window without disposing it.
        /// Sets <see cref="_redocking"/> so <see cref="OnFormClosing"/> skips close handling.
        /// </summary>
        internal DockPanel ExtractHostedPanel()
        {
            _redocking = true;
            if (_panel == null)
                return null;

            var panel = _panel;
            panel.ShowCaption = true;
            panel.Dock = DockStyle.None;
            Controls.Remove(panel);
            _panel = null;
            return panel;
        }

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
    }
}
