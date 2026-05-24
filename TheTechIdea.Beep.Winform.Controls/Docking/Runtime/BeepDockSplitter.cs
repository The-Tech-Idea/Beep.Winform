using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Runtime
{
    /// <summary>
    /// A visual splitter control that sits between two docked panels or groups and lets the
    /// user resize them by dragging.
    ///
    /// Design notes (follows DockPanelSuite SplitterBase / SplitterControl pattern):
    /// - Inherits <see cref="Control"/>, <c>Selectable = false</c>.
    /// - Dock property sets the correct cursor (VSplit for Left/Right, HSplit for Top/Bottom).
    /// - Thickness: 4 px (same as DockPanelSuite default splitter size).
    /// - Drag model: capture mouse on LButton down, fire <see cref="SplitterMoved"/> on mouse
    ///   move, release on mouse up.  No ghost line — live resize, same as VS/DockPanelSuite.
    ///
    /// Reference files:
    ///   dockpanelsuite-master\WinFormsUI\Docking\SplitterBase.cs
    ///   dockpanelsuite-master\WinFormsUI\Docking\DockPanel.AutoHideWindow.cs (SplitterControl)
    ///   Krypton.Docking — uses internal splitter via KryptonWorkspaceCellSeparator concept
    /// </summary>
    [ToolboxItem(false)]
    [DesignerCategory("code")]
    [DesignTimeVisible(false)]
    public class BeepDockSplitter : Control
    {
        // ── Constants ───────────────────────────────────────────────────────
        public  const int DefaultThickness = 4;

        // ── Fields ──────────────────────────────────────────────────────────
        private bool   _dragging;
        private Point  _dragStart;         // cursor position at drag start
        private int    _sizeAtDragStart;   // size of the "first" panel at drag start
        private DockingThemeColors _themeColors = DockingThemeColors.Default;

        // ── Events ───────────────────────────────────────────────────────────

        /// <summary>
        /// Raised continuously while the splitter is being dragged.
        /// The handler should resize the adjacent panels.
        /// </summary>
        public event EventHandler<SplitterMovedEventArgs> SplitterMoved;

        // ── Constructor ──────────────────────────────────────────────────────

        public BeepDockSplitter()
        {
            SetStyle(ControlStyles.Selectable, false);
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint, true);

            BackColor = Color.FromArgb(60, 60, 65);
        }

        internal void ApplyDockingTheme(DockingThemeColors colors)
        {
            _themeColors = colors ?? DockingThemeColors.Default;
            BackColor = _themeColors.SplitterBackColor;
            Invalidate();
        }

        // ── Dock override — mirrors DockPanelSuite SplitterBase.Dock ─────────

        public override DockStyle Dock
        {
            get => base.Dock;
            set
            {
                SuspendLayout();
                base.Dock = value;

                if (value == DockStyle.Left || value == DockStyle.Right)
                {
                    Width  = DefaultThickness;
                    Cursor = Cursors.VSplit;
                }
                else if (value == DockStyle.Top || value == DockStyle.Bottom)
                {
                    Height = DefaultThickness;
                    Cursor = Cursors.HSplit;
                }
                else
                {
                    Bounds = Rectangle.Empty;
                    Cursor = Cursors.Default;
                }

                ResumeLayout();
            }
        }

        // ── Mouse drag ───────────────────────────────────────────────────────

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button != MouseButtons.Left) return;

            _dragging        = true;
            _dragStart       = PointToScreen(e.Location);
            _sizeAtDragStart = (Dock == DockStyle.Left || Dock == DockStyle.Right)
                                   ? Width
                                   : Height;
            Capture = true;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (!_dragging) return;

            var current = PointToScreen(e.Location);
            int delta   = (Dock == DockStyle.Left || Dock == DockStyle.Right)
                              ? current.X - _dragStart.X
                              : current.Y - _dragStart.Y;

            SplitterMoved?.Invoke(this, new SplitterMovedEventArgs
            {
                SizeAtDragStart = _sizeAtDragStart,
                Delta           = delta
            });
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (!_dragging) return;
            _dragging = false;
            Capture   = false;
        }

        // ── Painting ─────────────────────────────────────────────────────────

        protected override void OnPaint(PaintEventArgs e)
        {
            // Flat splitter bar — same style as VS/DockPanelSuite default
            using (var brush = new SolidBrush(BackColor))
                e.Graphics.FillRectangle(brush, ClientRectangle);
        }
    }

    /// <summary>
    /// Event arguments for <see cref="BeepDockSplitter.SplitterMoved"/>.
    /// </summary>
    public class SplitterMovedEventArgs : EventArgs
    {
        /// <summary>Size (width or height) of the first adjacent panel at drag start.</summary>
        public int SizeAtDragStart { get; set; }

        /// <summary>
        /// Pixel delta from drag-start position (positive = drag toward bottom/right).
        /// </summary>
        public int Delta { get; set; }
    }
}
