using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.BottomNavBars
{
    /// <summary>
    /// How the bar positions itself inside its parent.
    /// </summary>
    public enum BottomBarPlacement
    {
        /// <summary>
        /// Sized to its content and centred horizontally, sitting above the parent's bottom edge.
        /// This is the floating pill the reference designs show.
        /// </summary>
        CenteredBottom = 0,

        /// <summary>Docked across the full width of the parent's bottom edge.</summary>
        FullWidthBottom = 1,

        /// <summary>The bar is positioned by the caller or the designer; the control does not move it.</summary>
        Manual = 2
    }

    public partial class BottomBar
    {
        /// <summary>
        /// Margin <c>OnPaint</c> insets the client rectangle by before handing it to a painter.
        /// Shared so the placement's width calculation and the paint agree on one rectangle.
        /// </summary>
        internal const int PainterInset = 8;

        private BottomBarPlacement _placement = BottomBarPlacement.CenteredBottom;
        private int _bottomMargin = 12;
        private int _sideMargin = 12;
        private Control _placementParent;

        /// <summary>
        /// How the bar positions itself. Defaults to <see cref="BottomBarPlacement.CenteredBottom"/>.
        /// </summary>
        /// <remarks>
        /// The control used to set <c>Dock = DockStyle.Bottom</c> in its constructor, which forces the
        /// bar to span the parent's full width - and a docked control cannot be centred, because
        /// docking owns its bounds. Most of the reference designs are not full-width strips at all:
        /// they are pills sized to their items, centred, floating just above the bottom edge.
        /// <see cref="BottomBarPlacement.FullWidthBottom"/> keeps the old behaviour for the designs
        /// that do span the screen.
        /// </remarks>
        [Browsable(true)]
        [Category("Layout")]
        [DefaultValue(BottomBarPlacement.CenteredBottom)]
        [Description("How the bar positions itself in its parent: centred and sized to content, full width, or manual.")]
        public BottomBarPlacement Placement
        {
            get => _placement;
            set
            {
                if (_placement == value) return;
                _placement = value;
                ApplyPlacement();
            }
        }

        /// <summary>Gap between the bar and the parent's bottom edge when centred.</summary>
        [Browsable(true)]
        [Category("Layout")]
        [DefaultValue(12)]
        [Description("Gap between the bar and the bottom of its parent when Placement is CenteredBottom.")]
        public int BottomMargin
        {
            get => _bottomMargin;
            set
            {
                int v = Math.Max(0, value);
                if (_bottomMargin == v) return;
                _bottomMargin = v;
                ApplyPlacement();
            }
        }

        /// <summary>
        /// Minimum gap kept on each side when centred, so the bar stays a floating panel.
        /// </summary>
        /// <remarks>
        /// Without this the bar only looked like a floating pill when the parent happened to be wider
        /// than the grid needed; on a narrow parent it grew to the full width and became a strip
        /// again, which is the very thing <see cref="BottomBarPlacement.CenteredBottom"/> exists to
        /// avoid. The grid shrinks its cells to fit the reduced width, which is what it already does
        /// when space is tight.
        /// </remarks>
        [Browsable(true)]
        [Category("Layout")]
        [DefaultValue(12)]
        [Description("Minimum gap on each side when Placement is CenteredBottom, so the bar stays a floating panel.")]
        public int SideMargin
        {
            get => _sideMargin;
            set
            {
                int v = Math.Max(0, value);
                if (_sideMargin == v) return;
                _sideMargin = v;
                ApplyPlacement();
            }
        }

        /// <summary>Positions and sizes the bar according to <see cref="Placement"/>.</summary>
        internal void ApplyPlacement()
        {
            switch (_placement)
            {
                case BottomBarPlacement.FullWidthBottom:
                    Dock = DockStyle.Bottom;
                    break;

                case BottomBarPlacement.CenteredBottom:
                    Dock = DockStyle.None;
                    Anchor = AnchorStyles.Bottom;
                    SizeAndCentre();
                    break;

                case BottomBarPlacement.Manual:
                    Dock = DockStyle.None;
                    break;
            }
        }

        private void SizeAndCentre()
        {
            var parent = Parent;
            if (parent == null) return;

            int count = Items?.Count ?? 0;
            bool hasCta = CTAIndex >= 0 && CTAIndex < count;
            int desired = _layoutHelper.GetPreferredContentWidth(count, hasCta);

            // GetPreferredContentWidth measures the grid, but this sets the control's OUTER width,
            // and OnPaint hands the painters a rect inset by PainterInset on each side. Without
            // adding that back the grid is 16px short of what was asked for, and the cells land at
            // 71px against a 74px spec - the kind of near-miss that looks like a rounding bug and is
            // actually two measurements of different rectangles.
            desired += PainterInset * 2 + Math.Max(0, Width - ClientSize.Width);

            // Never wider than the parent minus its side margins, so the panel keeps its inset even
            // when the grid would rather have more room. A bar that overflows its container is worse
            // than one that gives up its preferred width, and the layout already shrinks cells to fit.
            int maxWidth = Math.Max(1, parent.ClientSize.Width - _sideMargin * 2);
            int width = Math.Min(desired, maxWidth);

            int left = Math.Max(0, (parent.ClientSize.Width - width) / 2);
            int top = Math.Max(0, parent.ClientSize.Height - Height - _bottomMargin);

            Bounds = new Rectangle(left, top, width, Height);
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);

            if (_placementParent != null)
                _placementParent.SizeChanged -= PlacementParent_SizeChanged;

            _placementParent = Parent;

            if (_placementParent != null)
                _placementParent.SizeChanged += PlacementParent_SizeChanged;

            ApplyPlacement();
        }

        private void PlacementParent_SizeChanged(object sender, EventArgs e) => ApplyPlacement();

        private void DisposePlacement()
        {
            if (_placementParent == null) return;
            _placementParent.SizeChanged -= PlacementParent_SizeChanged;
            _placementParent = null;
        }
    }
}
