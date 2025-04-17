using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Controls")]
    [DisplayName("Beep Scrollbar")]
    [Description("A custom ScrollBar control")]
    public class BeepScrollBar : BeepControl
    {
        // EVENTS
        public event EventHandler Scroll;
        public event EventHandler ValueChanged;

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

        // COLORS (managed via theme but customizable)
        private Color _trackColor = SystemColors.ControlDark;
        private Color _thumbColor = SystemColors.ControlDarkDark;
        private Color _thumbColorHover = SystemColors.ControlDark;
        private Color _thumbColorActive = SystemColors.ControlDarkDark;

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
                    Width = 10;
                    if (Height < DefaultSize.Height) Height = DefaultSize.Height;
                }
                else
                {
                    Height = 10;
                    if (Width < DefaultSize.Width) Width = DefaultSize.Width;
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
                if (DesignMode) return;
                _minimum = value;
                if (_minimum > _maximum) _maximum = _minimum + 1;
                if (_value < _minimum) _value = _minimum;
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
                if (DesignMode) return;
                _maximum = Math.Max(value, _minimum + 1);
                if (_largeChange >= _maximum - _minimum)
                    _largeChange = (_maximum - _minimum) - 1;
                if (_value > _maximum) _value = _maximum;
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
                if (DesignMode) return;
                int newValue = Math.Max(_minimum, Math.Min(value, _maximum));
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
                if (DesignMode) return;
                _largeChange = Math.Max(1, Math.Min(value, (_maximum - _minimum)));
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
                if (DesignMode) return;
                _smallChange = Math.Max(1, value);
                Refresh();
            }
        }

        [Category("Appearance")]
        [DefaultValue(typeof(Color), "ControlDark")]
        public Color TrackColor
        {
            get => _trackColor;
            set { _trackColor = value; Refresh(); }
        }

        [Category("Appearance")]
        [DefaultValue(typeof(Color), "ControlDarkDark")]
        public Color ThumbColor
        {
            get => _thumbColor;
            set { _thumbColor = value; Refresh(); }
        }

        [Category("Appearance")]
        [DefaultValue(typeof(Color), "ControlDark")]
        public Color ThumbColorHover
        {
            get => _thumbColorHover;
            set { _thumbColorHover = value; Refresh(); }
        }

        [Category("Appearance")]
        [DefaultValue(typeof(Color), "ControlDarkDark")]
        public Color ThumbColorActive
        {
            get => _thumbColorActive;
            set { _thumbColorActive = value; Refresh(); }
        }

        protected override Size DefaultSize => new Size(10, 100);

        // CONSTRUCTOR
        public BeepScrollBar()
        {
            if (!DesignMode)
            {
                SetStyle(ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.OptimizedDoubleBuffer |
                         ControlStyles.ResizeRedraw |
                         ControlStyles.UserPaint, true);
            }
            if (_scrollOrientation == Orientation.Vertical)
                Size = new Size(10, 100);
            else
                Size = new Size(100, 10);
        }

        // THEMING
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            BackColor = _currentTheme.ScrollBarBackColor;
            TrackColor = _currentTheme.ScrollBarTrackColor;
            ThumbColor = _currentTheme.ScrollBarThumbColor;
            ThumbColorHover = _currentTheme.ScrollBarHoverThumbColor;
            ThumbColorActive = _currentTheme.ScrollBarActiveThumbColor;
        }

        // DRAWING
        protected override void DrawContent(Graphics g)
        {
            UpdateDrawingRect();
            var r = DrawingRect;
            if (r.Width <= 0 || r.Height <= 0) return;

            // Background
            using (var backBrush = new SolidBrush(BackColor))
                g.FillRectangle(backBrush, r);

            // Track
            using (var trackBrush = new SolidBrush(TrackColor))
                g.FillRectangle(trackBrush, r);

            // Thumb
            var thumbRect = GetThumbRectangle();
            Color thumbCol = _dragging ? ThumbColorActive :
                             _isHovering ? ThumbColorHover :
                                           ThumbColor;
            using (var thumbBrush = new SolidBrush(thumbCol))
                g.FillRectangle(thumbBrush, thumbRect);
        }

        private Rectangle GetThumbRectangle()
        {
            var r = DrawingRect;
            int range = _maximum - _minimum;
            if (range <= _largeChange)
                return r;

            if (_scrollOrientation == Orientation.Vertical)
            {
                int thumbH = Math.Max(10, r.Height * _largeChange / range);
                int trackLen = r.Height - thumbH;
                int pos = r.Y + trackLen * (_value - _minimum) / (range - _largeChange);
                return new Rectangle(r.X, pos, r.Width, thumbH);
            }
            else
            {
                int thumbW = Math.Max(10, r.Width * _largeChange / range);
                int trackLen = r.Width - thumbW;
                int pos = r.X + trackLen * (_value - _minimum) / (range - _largeChange);
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
                    Value += (e.Y < thumb.Y ? -_largeChange : _largeChange);
                else
                    Value += (e.X < thumb.X ? -_largeChange : _largeChange);
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
            int range = _maximum - _minimum - _largeChange;
            int newVal = _minimum + pos * range / trackLen;
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
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Left) Value -= _smallChange;
            if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Right) Value += _smallChange;
            if (e.KeyCode == Keys.PageUp) Value -= _largeChange;
            if (e.KeyCode == Keys.PageDown) Value += _largeChange;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            int delta = e.Delta / SystemInformation.MouseWheelScrollDelta * _smallChange;
            Value -= delta;
        }
    }
}
