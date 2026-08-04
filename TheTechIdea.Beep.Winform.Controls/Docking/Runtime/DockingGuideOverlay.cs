using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

using System.Collections.Generic;

using TheTechIdea.Beep.Winform.Controls.Docking.Layout;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Runtime
{
    /// <summary>
    /// A transparent, always-on-top overlay form that shows five docking guide diamonds
    /// (Left / Right / Top / Bottom / Fill) while the user is dragging a floating panel.
    /// The hovered diamond is highlighted; the active target is returned via <see cref="HitTest"/>.
    ///
    /// Design notes (follows DockPanelSuite DockPanel.DockDragHandler.DockIndicator pattern):
    /// - FormBorderStyle = None, transparent = true, TopMost = true
    /// - Five guide diamonds: Centre-Left, Centre-Right, Centre-Top, Centre-Bottom, Centre-Fill
    /// - Each diamond is 32×32 px, arranged in a plus (+) shape
    /// - Hovered diamond drawn with accent highlight
    ///
    /// Reference files:
    ///   dockpanelsuite-master\WinFormsUI\Docking\DockPanel.DockDragHandler.cs
    ///   dockpanelsuite-master\WinFormsUI\Docking\DockPanel.DragHandler.cs
    ///   Krypton.Docking\DockingDragManager.cs (concept)
    /// </summary>
    [ToolboxItem(false)]
    [DesignerCategory("code")]
    [DesignTimeVisible(false)]
    public class DockingGuideOverlay : Form
    {
        // ── Diamond geometry (mirrors DockPanelSuite PaneIndicator layout) ──
        private const int DiamondSize   = 32;
        private const int DiamondSpacing = 36;  // centre-to-centre distance

        // ── Fields ──────────────────────────────────────────────────────────
        private Rectangle _leftRect;
        private Rectangle _rightRect;
        private Rectangle _topRect;
        private Rectangle _bottomRect;
        private Rectangle _fillRect;
        private DockPosition? _hoveredTarget;

        // 0..1 growth of the hovered diamond. Reset when the hovered target changes, so the
        // growth restarts on the new one rather than continuing from the old one's progress.
        private float _hoverGrowth;
        private System.Windows.Forms.Timer _hoverTimer;

        /// <summary>Pixels the hovered diamond grows by, per side, when fully expanded.</summary>
        private const int HoverGrowthPixels = 4;

        /// <summary>Milliseconds from first hover to full size.</summary>
        private const int HoverGrowthDuration = 120;

        private const int HoverTickInterval = 15;

        /// <summary>
        /// Whether the hover growth is animated. <c>null</c> (the default) follows the operating
        /// system's UI-effects setting.
        /// </summary>
        /// <remarks>
        /// Reduced motion does <b>not</b> mean no feedback: with animation off the diamond still
        /// grows, it simply arrives at full size immediately. Removing the size change instead
        /// would take the hover indication away from exactly the users who asked for less movement,
        /// which is the opposite of the intent.
        /// <para>
        /// Settable so the behaviour can be exercised without changing a machine-wide Windows
        /// setting - the same reason the display set and high contrast are inputs.
        /// </para>
        /// </remarks>
        public bool? AnimateGuides { get; set; }

        /// <summary>True when the hover growth should be animated rather than immediate.</summary>
        public bool IsAnimated => AnimateGuides ?? SystemInformation.UIEffectsEnabled;

        /// <summary>Current growth of the hovered diamond, 0..1.</summary>
        internal float HoverGrowth => _hoverGrowth;
        private Rectangle _snapLineBounds;       // screen-coord rect of the active snap line
        private DockPosition _snapLinePosition;
        private bool _snapLineVisible;

        // ── Constructor ──────────────────────────────────────────────────────

        public DockingGuideOverlay()
        {
            FormBorderStyle     = FormBorderStyle.None;
            ShowInTaskbar       = false;
            TopMost             = true;
            BackColor           = Color.Lime;          // TransparencyKey colour — never drawn
            TransparencyKey     = Color.Lime;
            AllowTransparency   = true;
            StartPosition       = FormStartPosition.Manual;
            Size                = new Size(DiamondSpacing * 3, DiamondSpacing * 3);

            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint, true);

            LayoutDiamonds();
        }

        // ── Public API ───────────────────────────────────────────────────────

        /// <summary>
        /// The guide target currently under the mouse, or null if not over any diamond.
        /// Updated by <see cref="HitTest"/>.
        /// </summary>
        public DockPosition? ActiveTarget => _hoveredTarget;

        /// <summary>
        /// Centres the overlay on the given host-form client area and shows it.
        /// </summary>
        public void ShowOver(Form hostForm) => ShowOver(hostForm, null);

        /// <summary>
        /// Centres the guide rosette over <paramref name="hostForm"/>, kept inside the working area
        /// of whichever display it lands on.
        /// </summary>
        /// <param name="monitors">
        /// Displays to reason about; the real ones when null. Supplied so the placement can be
        /// resolved against a given set rather than the machine's.
        /// </param>
        /// <remarks>
        /// Centring alone is not enough once more than one display is involved: a host form
        /// straddling two monitors, or positioned so its centre falls near an edge, puts part of the
        /// rosette where the user cannot see or hit it. Clamping goes through
        /// <see cref="FloatBoundsResolver.ClampInto"/> - the same rule float restore uses - rather
        /// than a second implementation that could disagree about what "on screen" means.
        /// </remarks>
        public void ShowOver(Form hostForm, IReadOnlyList<MonitorInfo> monitors)
        {
            if (hostForm == null) return;

            var centre = hostForm.RectangleToScreen(
                new Rectangle(hostForm.DisplayRectangle.Width  / 2 - Width  / 2,
                              hostForm.DisplayRectangle.Height / 2 - Height / 2, Width, Height));

            var screens = monitors ?? SystemMonitorProvider.Instance.GetMonitors();
            var target = MonitorUnder(centre, screens);
            if (target.WorkingArea.Width > 0)
                centre = FloatBoundsResolver.ClampInto(centre, target.WorkingArea);

            Location = centre.Location;
            if (!Visible) Show(hostForm);
        }

        /// <summary>
        /// Centres the guide rosette over an arbitrary screen rectangle — used to put it over the
        /// group being hovered rather than over the whole host.
        /// </summary>
        /// <remarks>
        /// This is what makes the compass mean "dock into <i>this</i> panel". Centred on the host,
        /// the same four diamonds can only ever describe the host's edges, so a user aiming at one
        /// panel among several gets no indication of which one will receive the drop.
        /// <para>
        /// Returns false and leaves the overlay alone when the target is too small to hold the
        /// rosette; a compass larger than the region it describes points at its own neighbours.
        /// </para>
        /// </remarks>
        public bool CenterOn(Rectangle screenRect, IReadOnlyList<MonitorInfo> monitors)
        {
            if (screenRect.Width < Width || screenRect.Height < Height)
                return false;

            var placed = new Rectangle(
                screenRect.X + (screenRect.Width - Width) / 2,
                screenRect.Y + (screenRect.Height - Height) / 2,
                Width, Height);

            var screens = monitors ?? SystemMonitorProvider.Instance.GetMonitors();
            var display = MonitorUnder(placed, screens);
            if (display.WorkingArea.Width > 0)
                placed = FloatBoundsResolver.ClampInto(placed, display.WorkingArea);

            if (Location != placed.Location)
                Location = placed.Location;

            return true;
        }

        /// <summary>
        /// Starts (or completes) the hovered diamond's growth.
        /// </summary>
        private void BeginHoverGrowth()
        {
            _hoverGrowth = 0f;

            if (_hoveredTarget == null)
            {
                StopHoverTimer();
                return;
            }

            if (!IsAnimated)
            {
                // Reduced motion: arrive at full size without the intermediate frames.
                _hoverGrowth = 1f;
                StopHoverTimer();
                return;
            }

            if (_hoverTimer == null)
            {
                _hoverTimer = new System.Windows.Forms.Timer { Interval = HoverTickInterval };
                _hoverTimer.Tick += OnHoverTick;
            }

            _hoverTimer.Start();
        }

        private void OnHoverTick(object sender, EventArgs e)
        {
            _hoverGrowth += HoverTickInterval / (float)HoverGrowthDuration;

            if (_hoverGrowth >= 1f || _hoveredTarget == null)
            {
                _hoverGrowth = _hoveredTarget == null ? 0f : 1f;
                StopHoverTimer();
            }

            Invalidate();
        }

        private void StopHoverTimer() => _hoverTimer?.Stop();

        protected override void Dispose(bool disposing)
        {
            if (disposing && _hoverTimer != null)
            {
                _hoverTimer.Tick -= OnHoverTick;
                _hoverTimer.Dispose();
                _hoverTimer = null;
            }

            base.Dispose(disposing);
        }

        /// <summary>The display a rectangle's centre falls on, else the one it most overlaps.</summary>
        private static MonitorInfo MonitorUnder(Rectangle rect, IReadOnlyList<MonitorInfo> monitors)
        {
            if (monitors == null || monitors.Count == 0)
                return default;

            var centre = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
            foreach (var m in monitors)
            {
                if (m.Bounds.Contains(centre))
                    return m;
            }

            MonitorInfo best = default;
            long bestArea = 0;
            foreach (var m in monitors)
            {
                var overlap = Rectangle.Intersect(rect, m.Bounds);
                long area = (long)Math.Max(0, overlap.Width) * Math.Max(0, overlap.Height);
                if (area > bestArea) { bestArea = area; best = m; }
            }

            return bestArea > 0 ? best : monitors[0];
        }

        /// <summary>
        /// Tests whether the given screen point is over a guide diamond.
        /// Updates <see cref="ActiveTarget"/> and repaints the overlay.
        /// </summary>
        /// <param name="screenPt">Cursor position in screen coordinates.</param>
        /// <returns>The hovered <see cref="DockPosition"/>, or null.</returns>
        public DockPosition? HitTest(Point screenPt)
        {
            var clientPt = PointToClient(screenPt);
            DockPosition? hit = null;

            if      (_leftRect.Contains(clientPt))   hit = DockPosition.Left;
            else if (_rightRect.Contains(clientPt))  hit = DockPosition.Right;
            else if (_topRect.Contains(clientPt))    hit = DockPosition.Top;
            else if (_bottomRect.Contains(clientPt)) hit = DockPosition.Bottom;
            else if (_fillRect.Contains(clientPt))   hit = DockPosition.Fill;

            if (hit != _hoveredTarget)
            {
                _hoveredTarget = hit;
                BeginHoverGrowth();
                Invalidate();
            }
            return hit;
        }

        /// <summary>
        /// Shows a thin accent bar at the given screen-coord rectangle to indicate where a
        /// tab-drag would insert or split a dockspace. Used for <c>GroupEdge</c> and
        /// <c>GroupCenterStack</c> drop targets. Pass an empty rect to clear the snap line.
        /// </summary>
        /// <param name="screenRect">Bar location/size in screen coordinates (in
        /// <see cref="DockPosition"/> orientation; the bar is always 3 px wide).</param>
        /// <param name="orientation">Which edge of the bar corresponds to the dock edge.</param>
        public void ShowSnapGuide(Rectangle screenRect, DockPosition orientation)
        {
            bool wasVisible = _snapLineVisible;
            _snapLineVisible = !screenRect.IsEmpty;
            _snapLineBounds  = screenRect;
            _snapLinePosition = orientation;
            if (wasVisible != _snapLineVisible)
                Invalidate();
        }

        /// <summary>Removes any active snap-line indicator.</summary>
        public void ClearSnapGuide()
        {
            if (!_snapLineVisible) return;
            _snapLineVisible = false;
            _snapLineBounds  = Rectangle.Empty;
            Invalidate();
        }

        // ── Layout ───────────────────────────────────────────────────────────

        private void LayoutDiamonds()
        {
            int cx = Width  / 2 - DiamondSize / 2;
            int cy = Height / 2 - DiamondSize / 2;

            _fillRect   = new Rectangle(cx, cy, DiamondSize, DiamondSize);
            _leftRect   = new Rectangle(cx - DiamondSpacing, cy, DiamondSize, DiamondSize);
            _rightRect  = new Rectangle(cx + DiamondSpacing, cy, DiamondSize, DiamondSize);
            _topRect    = new Rectangle(cx, cy - DiamondSpacing, DiamondSize, DiamondSize);
            _bottomRect = new Rectangle(cx, cy + DiamondSpacing, DiamondSize, DiamondSize);
        }

        // ── Painting ─────────────────────────────────────────────────────────

        // Direction glyphs use SvgsUIcons (rendered via StyledImagePainter), not Wingdings.
        private static string IconFor(DockPosition target) => target switch
        {
            DockPosition.Left   => SvgsUIcons.Carets.Left,
            DockPosition.Right  => SvgsUIcons.Carets.Right,
            DockPosition.Top    => SvgsUIcons.Carets.Up,
            DockPosition.Bottom => SvgsUIcons.Carets.Down,
            DockPosition.Fill   => SvgsUIcons.Shapes.Square,
            _                   => SvgsUIcons.Shapes.Square
        };

        protected override void OnPaint(PaintEventArgs e)
        {
            // Background is transparent via TransparencyKey — only paint the diamonds
            DrawDiamond(e.Graphics, _leftRect,   DockPosition.Left);
            DrawDiamond(e.Graphics, _rightRect,  DockPosition.Right);
            DrawDiamond(e.Graphics, _topRect,    DockPosition.Top);
            DrawDiamond(e.Graphics, _bottomRect, DockPosition.Bottom);
            DrawDiamond(e.Graphics, _fillRect,   DockPosition.Fill);

            if (_snapLineVisible)
                DrawSnapLine(e.Graphics);
        }

        private void DrawSnapLine(Graphics g)
        {
            // Convert screen-coord bar to overlay-client coords.
            var local = RectangleToClient(_snapLineBounds);
            if (local.Width <= 0 || local.Height <= 0)
                return;

            // The whole region the panel will occupy, filled translucently - not a sliver of its
            // edge. `local` is the caller's PreviewBounds, the same rectangle that positions the
            // drag ghost and the same one the drop actually produces, so filling it is truthful.
            //
            // Previously only DockPosition.Fill showed the region; every edge position drew a 3px
            // bar, so a user dropping Left saw a thin line and had to infer how much space the
            // panel would take. Every comparable product - Visual Studio, VS Code, Dockview, Golden
            // Layout - previews the resulting rectangle.
            using (var region = new SolidBrush(Color.FromArgb(64, 0, 122, 204)))
                g.FillRectangle(region, local);

            const int thickness = 3;
            Rectangle edge;
            switch (_snapLinePosition)
            {
                case DockPosition.Left:   edge = new Rectangle(local.X, local.Y, thickness, local.Height); break;
                case DockPosition.Right:  edge = new Rectangle(local.Right - thickness, local.Y, thickness, local.Height); break;
                case DockPosition.Top:    edge = new Rectangle(local.X, local.Y, local.Width, thickness); break;
                case DockPosition.Bottom: edge = new Rectangle(local.X, local.Bottom - thickness, local.Width, thickness); break;
                case DockPosition.Fill:   edge = Rectangle.Empty; break;
                default: return;
            }

            // The accent edge says which side the split lands on - Fill has no side, so none.
            if (!edge.IsEmpty)
                using (var accent = new SolidBrush(Color.FromArgb(200, 0, 122, 204)))
                    g.FillRectangle(accent, edge);

            using (var pen = new Pen(Color.FromArgb(220, 255, 255, 255), 1f))
                g.DrawRectangle(pen, new Rectangle(local.X, local.Y, local.Width - 1, local.Height - 1));
        }

        private void DrawDiamond(Graphics g, Rectangle r, DockPosition target)
        {
            bool isHot = (_hoveredTarget == target);

            // The hovered target grows toward the cursor. Reference products all do this, and it
            // matters more than decoration: with five diamonds close together, colour alone is a
            // weak signal about which one a release will actually take.
            if (isHot && _hoverGrowth > 0f)
            {
                int grow = (int)Math.Round(HoverGrowthPixels * _hoverGrowth);
                r = Rectangle.Inflate(r, grow, grow);
            }

            Color back   = isHot ? Color.FromArgb(0, 122, 204) : Color.FromArgb(45, 45, 48);
            Color border = isHot ? Color.White                  : Color.FromArgb(100, 100, 100);
            Color glyph  = Color.White;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Diamond shape path (rotated square)
            using (var path = DiamondPath(r))
            {
                using (var fill = new SolidBrush(back))
                    g.FillPath(fill, path);
                using (var pen = new Pen(border, 1.5f))
                    g.DrawPath(pen, path);
            }

            // Centred SVG glyph, inset so it sits inside the diamond body.
            var iconRect = Rectangle.Inflate(r, -9, -9);
            if (iconRect.Width >= 2 && iconRect.Height >= 2)
            {
                try
                {
                    StyledImagePainter.PaintWithTint(g, iconRect, IconFor(target), glyph, 1f, 0);
                }
                catch
                {
                    // Overlay stays usable even if the icon asset fails to load.
                }
            }
        }

        private static GraphicsPath DiamondPath(Rectangle r)
        {
            int mx = r.X + r.Width  / 2;
            int my = r.Y + r.Height / 2;
            var path = new GraphicsPath();
            path.AddPolygon(new[]
            {
                new Point(mx,         r.Top),
                new Point(r.Right,    my),
                new Point(mx,         r.Bottom),
                new Point(r.Left,     my)
            });
            return path;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            LayoutDiamonds();
        }
    }
}
