using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Scolling
{
    [ToolboxItem(true)]
    [Category("Controls")]
    [DisplayName("Beep Scrollbar")]
    [Description("A custom ScrollBar control")]
    public class BeepScrollBar : BaseControl
    {
        // EVENTS
        public event EventHandler Scroll;
        public event EventHandler ValueChanged;
        // DPI-aware property helpers. ScaleValue, NOT ScaleFactorToDpi - the latter
        // converts a scale FACTOR to a DPI value (10 -> 960), which made the minimum
        // thumb 960px tall (swallowing the whole track) and the default size 960x9600.
        private int GetScaledScrollbarWidth() => DpiScalingHelper.ScaleValue(10, this);
        private int GetScaledScrollbarHeight() => DpiScalingHelper.ScaleValue(100, this);
        private int GetScaledMinThumbSize() => DpiScalingHelper.ScaleValue(10, this);

        // FIXED: DPI-aware DefaultSize
        protected override Size DefaultSize => new Size(GetScaledScrollbarWidth(), GetScaledScrollbarHeight());
        protected internal override Padding StylePadding => new Padding(0);

        // FIELDS
        private int _value = 0;
        private int _minimum = 0;
        private int _maximum = 100;
        private int _largeChange = 10;
        private int _smallChange = 1;

        private bool _dragging;
        private int _dragOffset;
        private Orientation _scrollOrientation = Orientation.Vertical;
        private bool _isHovering = false;

        // Helper: effective upper bound for Value (like Win32 ScrollBar: Maximum - LargeChange)
        private int ValueUpperBound => Math.Max(_minimum, _maximum - _largeChange);

        // COLORS: Empty = themed; an explicit caller colour survives theme changes
        // because resolution happens per paint (custom-else-slot), never by stamping.
        private Color _trackColor = Color.Empty;
        private Color _thumbColor = Color.Empty;
        private Color _thumbColorHover = Color.Empty;
        private Color _thumbColorActive = Color.Empty;

        // PROPERTIES
        [Category("Behavior")]
        [DefaultValue(Orientation.Vertical)]
        public Orientation ScrollOrientation
        {
            get => _scrollOrientation;
            set
            {
                _scrollOrientation = value;
                if (_scrollOrientation == Orientation.Vertical)
                {
                    Width = GetScaledScrollbarWidth();
                    if (Height < GetScaledScrollbarHeight())
                        Height = GetScaledScrollbarHeight();
                }
                else
                {
                    Height = GetScaledScrollbarWidth();
                    if (Width < GetScaledScrollbarHeight())
                        Width = GetScaledScrollbarHeight();
                }
                Refresh();
            }
        }

        [Category("Behavior")]
        [DefaultValue(0)]
        public int Minimum
        {
            get => _minimum;
            set
            {
                _minimum = value;
                if (_minimum > _maximum) _maximum = _minimum + 1;
                if (_largeChange >= _maximum - _minimum)
                    _largeChange = Math.Max(1, (_maximum - _minimum));
                if (_value < _minimum) _value = _minimum;
                if (_value > ValueUpperBound) _value = ValueUpperBound;
                Refresh();
            }
        }

        [Category("Behavior")]
        [DefaultValue(100)]
        public int Maximum
        {
            get => _maximum;
            set
            {
                _maximum = Math.Max(value, _minimum + 1);
                if (_largeChange > _maximum - _minimum)
                    _largeChange = Math.Max(1, (_maximum - _minimum));
                if (_value > ValueUpperBound) _value = ValueUpperBound;
                Refresh();
            }
        }

        [Category("Behavior")]
        [DefaultValue(0)]
        public int Value
        {
            get => _value;
            set
            {
                // Clamp to [Minimum, Maximum - LargeChange]
                int upper = ValueUpperBound;
                int newValue = Math.Max(_minimum, Math.Min(value, upper));
                if (_value != newValue)
                {
                    int old = _value;
                    _value = newValue;
                    Refresh();  // immediate repaint

                    ValueChanged?.Invoke(this, EventArgs.Empty);
                    Scroll?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        [Category("Behavior")]
        [DefaultValue(10)]
        public int LargeChange
        {
            get => _largeChange;
            set
            {
                int range = Math.Max(1, (_maximum - _minimum));
                _largeChange = Math.Max(1, Math.Min(value, range));
                if (_value > ValueUpperBound) _value = ValueUpperBound;
                Refresh();
            }
        }

        [Category("Behavior")]
        [DefaultValue(1)]
        public int SmallChange
        {
            get => _smallChange;
            set
            {
                _smallChange = Math.Max(1, value);
                Refresh();
            }
        }

        [Category("Appearance")]
        public Color TrackColor
        {
            get => _trackColor;
            set { _trackColor = value; Refresh(); }
        }

        [Category("Appearance")]
        public Color ThumbColor
        {
            get => _thumbColor;
            set { _thumbColor = value; Refresh(); }
        }

        [Category("Appearance")]
        public Color ThumbColorHover
        {
            get => _thumbColorHover;
            set { _thumbColorHover = value; Refresh(); }
        }

        [Category("Appearance")]
        public Color ThumbColorActive
        {
            get => _thumbColorActive;
            set { _thumbColorActive = value; Refresh(); }
        }

        // CONSTRUCTOR
        public BeepScrollBar()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.UserPaint, true);
            // FIXED: Use DPI-scaled sizes
            if (_scrollOrientation == Orientation.Vertical)
                Size = new Size(GetScaledScrollbarWidth(), GetScaledScrollbarHeight());
            else
                Size = new Size(GetScaledScrollbarHeight(), GetScaledScrollbarWidth());

            // Accessibility
            AccessibleRole = AccessibleRole.ScrollBar;
            AccessibleName = "Scroll Bar";
            AccessibleDescription = "A custom scrollbar control";
        }

        // DRAWING — colours resolve per paint: custom-else-slot from the ScrollBar*
        // family, high contrast from the system palette. The previous ApplyTheme
        // STAMPED the slots into the custom properties, which made an explicit caller
        // colour indistinguishable from a themed one.
        protected override void DrawContent(Graphics g)
        {
            UpdateDrawingRect();
            var r = DrawingRect;
            if (r.Width <= 0 || r.Height <= 0) return;

            bool hc = SystemInformation.HighContrast;
            Color track = hc ? SystemColors.ScrollBar
                : _trackColor != Color.Empty ? _trackColor : _currentTheme.ScrollBarTrackColor;
            Color thumb;
            if (_dragging)
                thumb = hc ? SystemColors.Highlight
                    : _thumbColorActive != Color.Empty ? _thumbColorActive : _currentTheme.ScrollBarActiveThumbColor;
            else if (_isHovering)
                thumb = hc ? SystemColors.Highlight
                    : _thumbColorHover != Color.Empty ? _thumbColorHover : _currentTheme.ScrollBarHoverThumbColor;
            else
                thumb = hc ? SystemColors.ControlText
                    : _thumbColor != Color.Empty ? _thumbColor : _currentTheme.ScrollBarThumbColor;

            using (var trackBrush = new SolidBrush(track))
            {
                g.FillRectangle(trackBrush, r);
            }
            using (var thumbBrush = new SolidBrush(thumb))
            {
                g.FillRectangle(thumbBrush, GetThumbRectangle());
            }
        }
        // ADD: Override DPI change handling
        protected override void OnDpiChangedAfterParent(EventArgs e)
        {
            base.OnDpiChangedAfterParent(e);

            // Recalculate size with new DPI
            if (_scrollOrientation == Orientation.Vertical)
            {
                Width = GetScaledScrollbarWidth();
                // Keep current height but ensure minimum
                if (Height < GetScaledScrollbarHeight())
                    Height = GetScaledScrollbarHeight();
            }
            else
            {
                Height = GetScaledScrollbarWidth();
                // Keep current width but ensure minimum
                if (Width < GetScaledScrollbarHeight())
                    Width = GetScaledScrollbarHeight();
            }

            Invalidate();
        }
        // ADD: Override font change handling (even though scrollbar doesn't use text)
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);

            // Scrollbar size might need adjustment based on font scaling
            if (_scrollOrientation == Orientation.Vertical)
            {
                Width = GetScaledScrollbarWidth();
            }
            else
            {
                Height = GetScaledScrollbarWidth();
            }

            Invalidate();
        }
        private Rectangle GetThumbRectangle()
        {
            var r = DrawingRect;
            int range = _maximum - _minimum;
            if (range <= 0) return r;

            int minThumbSize = GetScaledMinThumbSize(); // Use scaled minimum

            if (_scrollOrientation == Orientation.Vertical)
            {
                if (range <= _largeChange) return new Rectangle(r.X, r.Y, r.Width, r.Height);
                int thumbH = Math.Max(minThumbSize, (int)Math.Round(r.Height * (_largeChange / (double)range)));
                int trackLen = r.Height - thumbH;
                int denom = Math.Max(1, range - _largeChange);
                int pos = r.Y + (int)Math.Round(trackLen * (_value - _minimum) / (double)denom);
                return new Rectangle(r.X, pos, r.Width, thumbH);
            }
            else
            {
                if (range <= _largeChange) return new Rectangle(r.X, r.Y, r.Width, r.Height);
                int thumbW = Math.Max(minThumbSize, (int)Math.Round(r.Width * (_largeChange / (double)range)));
                int trackLen = r.Width - thumbW;
                int denom = Math.Max(1, range - _largeChange);
                int pos = r.X + (int)Math.Round(trackLen * (_value - _minimum) / (double)denom);
                return new Rectangle(pos, r.Y, thumbW, r.Height);
            }
        }

        // MOUSE & DRAG
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            var thumb = GetThumbRectangle();
            if (_maximum <= _minimum + _largeChange) return;

            if (thumb.Contains(e.Location))
            {
                _dragging = true;
                _dragOffset = _scrollOrientation == Orientation.Vertical
                              ? e.Y - thumb.Y
                              : e.X - thumb.X;
                Capture = true;
            }
            else
            {
                if (_scrollOrientation == Orientation.Vertical)
                    Value = Value + (e.Y < thumb.Y ? -_largeChange : _largeChange);
                else
                    Value = Value + (e.X < thumb.X ? -_largeChange : _largeChange);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (!_dragging) return;
            var r = DrawingRect;
            var thumb = GetThumbRectangle();
            int trackLen = (_scrollOrientation == Orientation.Vertical
                            ? r.Height - thumb.Height
                            : r.Width - thumb.Width);
            if (trackLen < 1) return;

            int pos = (_scrollOrientation == Orientation.Vertical
                       ? e.Y - _dragOffset - r.Y
                       : e.X - _dragOffset - r.X);
            pos = Math.Max(0, Math.Min(trackLen, pos));
            int range = _maximum - _minimum - _largeChange;
            if (range < 0) range = 0;
            int newVal = _minimum + (trackLen == 0 ? 0 : (int)Math.Round(pos * (range / (double)trackLen)));
            Value = newVal;
            Update(); // force immediate repaint
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _dragging = false;
            Capture = false;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _isHovering = true;
            Refresh();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _isHovering = false;
            _dragging = false;
            Refresh();
        }

        // OPTIONAL: Keyboard and Mouse Wheel support
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Left) Value = Value - _smallChange;
            if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Right) Value = Value + _smallChange;
            if (e.KeyCode == Keys.PageUp) Value = Value - _largeChange;
            if (e.KeyCode == Keys.PageDown) Value = Value + _largeChange;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            int deltaSteps = e.Delta / SystemInformation.MouseWheelScrollDelta;
            if (deltaSteps != 0)
            {
                Value = Value - (deltaSteps * _smallChange);
            }
        }
      
    }
}
