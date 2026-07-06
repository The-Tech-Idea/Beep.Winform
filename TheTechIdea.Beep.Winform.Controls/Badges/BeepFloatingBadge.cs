using System.ComponentModel;
using TheTechIdea.Beep.Winform.Controls.Layouts.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Badges
{
    public class BeepFloatingBadge : UserControl, IBeepBadge
    {
        protected override Size DefaultSize => BeepLayoutMetrics.Badge;
        private Control? _target;
        private BadgeLocation _location = new();
        private bool _showDropShadow = true;
        private Color _shadowColor = Color.FromArgb(80, 0, 0, 0);
        private int _shadowSize = 2;
        private bool _showBorder = true;
        private Color _borderColor = Color.White;
        private int _badgeDiameter = 22;
        private BadgeShape _shape = BadgeShape.Circle;
        private Color _badgeBackColor = Color.Red;
        private Color _badgeForeColor = Color.White;
        private bool _isAttached;

        private Rectangle _cachedShapeRect;
        private GraphicsPath? _cachedShapePath;
        private BadgeShape _cachedShape;
        private int _cachedDiameter;
        private SolidBrush? _cachedBackBrush;
        private SolidBrush? _cachedShadowBrush;
        private Pen? _cachedBorderPen;

        public BeepFloatingBadge()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            BackColor = Color.Transparent;
            Size = new Size(_badgeDiameter, _badgeDiameter);
            MinimumSize = new Size(8, 8);
            MaximumSize = new Size(48, 48);
            TabStop = false;
            Visible = false;
        }

        public Control? Target => _target;

        public BadgeLocation Location
        {
            get => _location;
            set
            {
                _location = value ?? new BadgeLocation();
                Reposition();
            }
        }

        public BadgeAnchor Anchor
        {
            get => Location.Anchor;
            set
            {
                Location.Anchor = value;
                Reposition();
            }
        }

        public Point Offset
        {
            get => Location.Offset;
            set
            {
                Location.Offset = value;
                Reposition();
            }
        }

        public bool ShowDropShadow
        {
            get => _showDropShadow;
            set
            {
                _showDropShadow = value;
                Invalidate();
            }
        }

        public Color ShadowColor
        {
            get => _shadowColor;
            set
            {
                _shadowColor = value;
                InvalidateCachedBrushes();
                Invalidate();
            }
        }

        public int ShadowSize
        {
            get => _shadowSize;
            set
            {
                _shadowSize = Math.Max(0, Math.Min(6, value));
                InvalidateCachedPaths();
                Invalidate();
            }
        }

        public bool ShowBorder
        {
            get => _showBorder;
            set
            {
                _showBorder = value;
                Invalidate();
            }
        }

        public Color BorderColor
        {
            get => _borderColor;
            set
            {
                _borderColor = value;
                InvalidateCachedBrushes();
                Invalidate();
            }
        }

        public int BadgeDiameter
        {
            get => _badgeDiameter;
            set
            {
                _badgeDiameter = Math.Max(8, Math.Min(48, value));
                Size = new Size(_badgeDiameter, _badgeDiameter);
                InvalidateCachedPaths();
                Reposition();
                Invalidate();
            }
        }

        public BadgeShape Shape
        {
            get => _shape;
            set
            {
                _shape = value;
                InvalidateCachedPaths();
                Invalidate();
            }
        }

        public Color BadgeBackColor
        {
            get => _badgeBackColor;
            set
            {
                _badgeBackColor = value;
                InvalidateCachedBrushes();
                Invalidate();
            }
        }

        public Color BadgeForeColor
        {
            get => _badgeForeColor;
            set
            {
                _badgeForeColor = value;
                Invalidate();
            }
        }

        public bool IsAttached => _isAttached;

        private bool _cornerOverlap = true;
        /// <summary>
        /// When true (default), the badge is centered on the anchor corner and sticks out
        /// beyond the target's edge by half its size. This is the modern UI/UX pattern
        /// (Material Design, Fluent UI) where the badge visually floats above the corner,
        /// sitting between the control and its parent in z-order. When false, the badge is
        /// flush inside the target's corner (legacy behavior).
        /// </summary>
        [Category("Layout")]
        [Description("Center the badge on the anchor corner so it sticks out by half its size (modern UI/UX).")]
        public bool CornerOverlap
        {
            get => _cornerOverlap;
            set { if (_cornerOverlap == value) return; _cornerOverlap = value; Reposition(); }
        }

        public event EventHandler? BadgeClick;
        public event EventHandler? BadgeOpened;
        public event EventHandler? BadgeClosed;

        public virtual void Attach(Control target)
        {
            if (target is null)
                throw new ArgumentNullException(nameof(target));

            Detach();

            if (target.Parent is null)
                throw new InvalidOperationException("Target control must have a parent to attach a badge.");

            _target = target;

            target.Parent.Controls.Add(this);
            target.Parent.Controls.SetChildIndex(this, 0);

            target.LocationChanged += OnTargetLayoutChanged;
            target.SizeChanged += OnTargetLayoutChanged;
            target.VisibleChanged += OnTargetVisibleChanged;
            target.ParentChanged += OnTargetParentChanged;
            target.Disposed += OnTargetDisposed;

            if (target.Parent is not null)
                target.Parent.Resize += OnParentResize;

            _isAttached = true;
            Reposition();
            BringToFront();
            Visible = target.Visible;

            try { BadgeOpened?.Invoke(this, EventArgs.Empty); }
            catch { }
        }

        public virtual void Detach()
        {
            if (_target is not null)
            {
                _target.LocationChanged -= OnTargetLayoutChanged;
                _target.SizeChanged -= OnTargetLayoutChanged;
                _target.VisibleChanged -= OnTargetVisibleChanged;
                _target.ParentChanged -= OnTargetParentChanged;
                _target.Disposed -= OnTargetDisposed;

                if (_badgeParent is not null)
                {
                    _badgeParent.Resize -= OnParentResize;
                    _badgeParent = null;
                }

                _target = null;
            }

            if (Parent is not null)
                Parent.Controls.Remove(this);

            _isAttached = false;
            Visible = false;

            try { BadgeClosed?.Invoke(this, EventArgs.Empty); }
            catch { }
        }

        public virtual void Reposition()
        {
            if (_target is null || Parent is null) return;
            var newLoc = Location.ComputeBounds(_target.Bounds, Size);
            if (_cornerOverlap)
            {
                newLoc = ApplyCornerOverlap(newLoc, _target.Bounds);
            }
            SetBounds(newLoc.X, newLoc.Y, Width, Height);
        }

        /// <summary>
        /// Centers the badge on the anchor corner so it sticks out by half its size in both
        /// directions. This produces the modern "floating" UI/UX look where the badge appears
        /// to sit between the control and its parent (on the parent's surface, centered on the
        /// control's corner).
        /// </summary>
        private Rectangle ApplyCornerOverlap(Rectangle currentBounds, Rectangle targetBounds)
        {
            int halfW = currentBounds.Width / 2;
            int halfH = currentBounds.Height / 2;

            // Determine which corner the anchor is on by inspecting the offset of
            // currentBounds relative to targetBounds.
            bool isTop = currentBounds.Top < targetBounds.Top + targetBounds.Height / 2;
            bool isBottom = !isTop;
            bool isLeft = currentBounds.Left < targetBounds.Left + targetBounds.Width / 2;
            bool isRight = !isLeft;

            int newX = currentBounds.X;
            int newY = currentBounds.Y;

            if (isLeft) newX = targetBounds.Left - halfW;
            else if (isRight) newX = targetBounds.Right - halfW;

            if (isTop) newY = targetBounds.Top - halfH;
            else if (isBottom) newY = targetBounds.Bottom - halfH;

            // Middle anchors (MiddleLeft, MiddleRight, MiddleCenter) center on the side.
            if (currentBounds.Left + halfW == targetBounds.Left + targetBounds.Width / 2)
            {
                // Centered horizontally â€” don't shift X.
            }
            if (currentBounds.Top + halfH == targetBounds.Top + targetBounds.Height / 2)
            {
                // Centered vertically â€” don't shift Y.
            }

            return new Rectangle(newX, newY, currentBounds.Width, currentBounds.Height);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            int w = Width - 1;
            int h = Height - 1;
            int shadowOffset = _showDropShadow ? _shadowSize : 0;

            int cbX = shadowOffset;
            int cbY = shadowOffset;
            int cbW = w - shadowOffset * 2;
            int cbH = h - shadowOffset * 2;
            var contentBounds = new Rectangle(cbX, cbY, cbW, cbH);

            if (_showDropShadow && shadowOffset > 0)
            {
                var shadowRect = new Rectangle(cbX + 1, cbY + 1, cbW, cbH);
                using var shadowPath = GetOrCreateShapePath(shadowRect);
                var shadowBrush = GetOrCreateShadowBrush();
                g.FillPath(shadowBrush, shadowPath);
            }

            using var shapePath = GetOrCreateShapePath(contentBounds);
            var backBrush = GetOrCreateBackBrush();
            g.FillPath(backBrush, shapePath);

            if (_showBorder)
            {
                var borderRect = new Rectangle(cbX - 1, cbY - 1, cbW + 2, cbH + 2);
                using var borderPath = GetShapePath(borderRect);
                var borderPen = GetOrCreateBorderPen();
                g.DrawPath(borderPen, borderPath);
            }

            DrawBadgeContent(g, contentBounds);
        }

        protected virtual void DrawBadgeContent(Graphics g, Rectangle contentBounds)
        {
        }

        protected GraphicsPath GetShapePath(Rectangle bounds)
        {
            switch (_shape)
            {
                case BadgeShape.Circle:
                    var circle = new GraphicsPath();
                    circle.AddEllipse(bounds);
                    return circle;

                case BadgeShape.RoundedSquare:
                    return GraphicsExtensions.GetRoundedRectPath(bounds, Math.Max(2, bounds.Width / 5));

                case BadgeShape.Pill:
                    return GraphicsExtensions.GetRoundedRectPath(bounds, bounds.Height / 2);

                case BadgeShape.Diamond:
                    var diamond = new GraphicsPath();
                    diamond.AddPolygon(new PointF[]
                    {
                        new(bounds.X + bounds.Width / 2f, bounds.Top),
                        new(bounds.Right, bounds.Y + bounds.Height / 2f),
                        new(bounds.X + bounds.Width / 2f, bounds.Bottom),
                        new(bounds.Left, bounds.Y + bounds.Height / 2f)
                    });
                    return diamond;

                default:
                    var rect = new GraphicsPath();
                    rect.AddRectangle(bounds);
                    return rect;
            }
        }

        private GraphicsPath GetOrCreateShapePath(Rectangle bounds)
        {
            if (_cachedShapePath is not null &&
                _cachedShapeRect == bounds &&
                _cachedShape == _shape &&
                _cachedDiameter == _badgeDiameter)
            {
                return _cachedShapePath;
            }

            _cachedShapePath?.Dispose();
            _cachedShapePath = GetShapePath(bounds);
            _cachedShapeRect = bounds;
            _cachedShape = _shape;
            _cachedDiameter = _badgeDiameter;
            return _cachedShapePath;
        }

        private SolidBrush GetOrCreateBackBrush()
        {
            if (_cachedBackBrush is not null && _cachedBackBrush.Color == _badgeBackColor)
                return _cachedBackBrush;

            _cachedBackBrush?.Dispose();
            _cachedBackBrush = new SolidBrush(_badgeBackColor);
            return _cachedBackBrush;
        }

        private SolidBrush GetOrCreateShadowBrush()
        {
            if (_cachedShadowBrush is not null && _cachedShadowBrush.Color == _shadowColor)
                return _cachedShadowBrush;

            _cachedShadowBrush?.Dispose();
            _cachedShadowBrush = new SolidBrush(_shadowColor);
            return _cachedShadowBrush;
        }

        private Pen GetOrCreateBorderPen()
        {
            if (_cachedBorderPen is not null && _cachedBorderPen.Color == _borderColor)
                return _cachedBorderPen;

            _cachedBorderPen?.Dispose();
            _cachedBorderPen = new Pen(_borderColor, 1.5f);
            return _cachedBorderPen;
        }

        private void InvalidateCachedPaths()
        {
            _cachedShapePath?.Dispose();
            _cachedShapePath = null;
        }

        private void InvalidateCachedBrushes()
        {
            _cachedBackBrush?.Dispose();
            _cachedBackBrush = null;
            _cachedShadowBrush?.Dispose();
            _cachedShadowBrush = null;
            _cachedBorderPen?.Dispose();
            _cachedBorderPen = null;
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            var handler = BadgeClick;
            if (handler is not null)
            {
                try { handler(this, EventArgs.Empty); }
                catch { }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Detach();
                InvalidateCachedPaths();
                InvalidateCachedBrushes();
            }
            base.Dispose(disposing);
        }

        private void OnTargetLayoutChanged(object? sender, EventArgs e) => Reposition();
        private void OnTargetVisibleChanged(object? sender, EventArgs e)
        {
            if (_target is not null) Visible = _target.Visible;
        }

        private Control? _badgeParent;
        private void OnTargetParentChanged(object? sender, EventArgs e)
        {
            if (_badgeParent is not null)
                _badgeParent.Resize -= OnParentResize;

            if (_target?.Parent is null)
            {
                _badgeParent = null;
                Detach();
            }
            else
            {
                _badgeParent = _target.Parent;
                if (Parent is not null) Parent.Controls.Remove(this);
                _target.Parent.Controls.Add(this);
                _target.Parent.Controls.SetChildIndex(this, 0);
                _target.Parent.Resize += OnParentResize;
                Reposition();
                BringToFront();
            }
        }

        private void OnTargetDisposed(object? sender, EventArgs e)
        {
            if (_badgeParent is not null)
                _badgeParent.Resize -= OnParentResize;
            _badgeParent = null;
            Detach();
        }
        private void OnParentResize(object? sender, EventArgs e) => Reposition();
    }
}
