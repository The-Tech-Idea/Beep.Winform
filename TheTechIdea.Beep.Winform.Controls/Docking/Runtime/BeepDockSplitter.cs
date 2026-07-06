using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;
using TheTechIdea.Beep.Winform.Controls.Docking.Painters;
using TheTechIdea.Beep.Winform.Controls.Docking.Painters.Splitter;
using TheTechIdea.Beep.Winform.Controls.Layouts.Helpers;

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
    ///   move, release on mouse up.  No ghost line â€” live resize, same as VS/DockPanelSuite.
    ///
    /// Reference files:
    ///   dockpanelsuite-master\WinFormsUI\Docking\SplitterBase.cs
    ///   dockpanelsuite-master\WinFormsUI\Docking\DockPanel.AutoHideWindow.cs (SplitterControl)
    ///   Krypton.Docking â€” uses internal splitter via KryptonWorkspaceCellSeparator concept
    /// </summary>
    [ToolboxItem(false)]
    [DesignerCategory("code")]
    [DesignTimeVisible(false)]
    public class BeepDockSplitter : Control
    {
        protected override Size DefaultSize => BeepLayoutMetrics.DockSplitter;
        // â”€â”€ Constants â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        public  const int DefaultThickness = 4;

        // â”€â”€ Fields â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        private bool   _dragging;
        private bool   _hovered;
        private Point  _dragStart;         // cursor position at drag start
        private int    _sizeAtDragStart;   // size of the "first" panel at drag start
        private SplitterOrientation _orientation = SplitterOrientation.Vertical;
        private DockingThemeColors _themeColors = DockingThemeColors.Default;
        private readonly DockingPainterContext _paintContext = new DockingPainterContext();

        /// <summary>Control style driving splitter rendering. Set by the manager.</summary>
        internal BeepControlStyle ControlStyle { get; set; } = BeepControlStyle.Material3;

        /// <summary>
        /// Id of the edge group this splitter resizes. Set by the manager when the splitter is
        /// positioned by the layout engine (manual-bounds mode rather than docked mode).
        /// </summary>
        internal string GroupId { get; set; }

        /// <summary>
        /// Resize orientation used when the splitter is positioned manually (Dock = None).
        /// Vertical = a vertical bar dragged horizontally (Left/Right edge groups);
        /// Horizontal = a horizontal bar dragged vertically (Top/Bottom edge groups).
        /// </summary>
        internal SplitterOrientation Orientation
        {
            get => _orientation;
            set
            {
                _orientation = value;
                if (base.Dock == DockStyle.None)
                    Cursor = value == SplitterOrientation.Horizontal ? Cursors.HSplit : Cursors.VSplit;
                Invalidate();
            }
        }

        private SplitterOrientation EffectiveOrientation =>
            (base.Dock == DockStyle.Top || base.Dock == DockStyle.Bottom)
                ? SplitterOrientation.Horizontal
                : (base.Dock == DockStyle.Left || base.Dock == DockStyle.Right)
                    ? SplitterOrientation.Vertical
                    : _orientation;

        // â”€â”€ Events â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        /// <summary>
        /// Raised continuously while the splitter is being dragged.
        /// The handler should resize the adjacent panels.
        /// </summary>
        public event EventHandler<SplitterMovedEventArgs> SplitterMoved;

        // â”€â”€ Constructor â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        public BeepDockSplitter()
        {
            SetStyle(ControlStyles.Selectable, false);
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint  |
                     ControlStyles.ResizeRedraw, true);

            BackColor = Color.FromArgb(60, 60, 65);
        }

        internal void ApplyDockingTheme(DockingThemeColors colors)
        {
            _themeColors = colors ?? DockingThemeColors.Default;
            BackColor = _themeColors.SplitterBackColor;
            Invalidate();
        }

        // â”€â”€ Dock override â€” mirrors DockPanelSuite SplitterBase.Dock â”€â”€â”€â”€â”€â”€â”€â”€â”€

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

        // â”€â”€ Mouse drag â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button != MouseButtons.Left) return;

            _dragging        = true;
            _dragStart       = PointToScreen(e.Location);
            _sizeAtDragStart = EffectiveOrientation == SplitterOrientation.Vertical
                                   ? Width
                                   : Height;
            Capture = true;
            Invalidate();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _hovered = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _hovered = false;
            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (!_dragging) return;

            var current = PointToScreen(e.Location);
            int delta   = EffectiveOrientation == SplitterOrientation.Vertical
                              ? current.X - _dragStart.X
                              : current.Y - _dragStart.Y;

            if (delta == 0) return;

            // Report an incremental delta (since the previous move) so consumers can accumulate.
            _dragStart = current;

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
            Invalidate();
        }

        // â”€â”€ Painting â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        protected override void OnPaint(PaintEventArgs e)
        {
            var orientation = EffectiveOrientation;

            _paintContext.Update(_themeColors, ControlStyle, ClientRectangle);
            _paintContext.IsHover = _hovered;
            _paintContext.IsDragging = _dragging;

            DockingPainterFactory.GetRenderers(ControlStyle).Splitter.Paint(e.Graphics, _paintContext, orientation);
        }
    }

    /// <summary>
    /// Event arguments for <see cref="BeepDockSplitter.SplitterMoved"/>.
    /// </summary>
    public class SplitterMovedEventArgs : EventArgs
    {
        /// <summary>Size (width or height) of the splitter at drag start.</summary>
        public int SizeAtDragStart { get; set; }

        /// <summary>
        /// Incremental pixel delta since the previous move event
        /// (positive = drag toward bottom/right). Accumulate to track total movement.
        /// </summary>
        public int Delta { get; set; }
    }
}
