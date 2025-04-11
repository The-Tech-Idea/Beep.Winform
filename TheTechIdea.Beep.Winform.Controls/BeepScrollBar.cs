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

        // COLORS (Now managed via theme, but kept as properties for customization)
        private Color _trackColor = SystemColors.ControlDark;
        private Color _thumbColor = SystemColors.ControlDarkDark;
        private Color _thumbColorHover = SystemColors.ControlDark; // New field
        private Color _thumbColorActive = SystemColors.ControlDarkDark; // New field

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
                    if (Height < DefaultSize.Height)
                        Height = DefaultSize.Height;
                }
                else
                {
                    Height = 10;
                    if (Width < DefaultSize.Width)
                        Width = DefaultSize.Width;
                }
                Invalidate();
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
                Invalidate();
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
                Invalidate();
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
                // Revert to allow full range, but map correctly in grid
                int newValue = Math.Max(_minimum, Math.Min(value, _maximum));
                if (_value != newValue)
                {
                    _value = newValue;
                    Invalidate();
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
                Invalidate();
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
                Invalidate();
            }
        }

        [Category("Appearance")]
        [DefaultValue(typeof(Color), "ControlDark")]
        public Color TrackColor
        {
            get => _trackColor;
            set
            {
                _trackColor = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [DefaultValue(typeof(Color), "ControlDarkDark")]
        public Color ThumbColor
        {
            get => _thumbColor;
            set
            {
                _thumbColor = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [DefaultValue(typeof(Color), "ControlDark")]
        public Color ThumbColorHover
        {
            get => _thumbColorHover;
            set
            {
                _thumbColorHover = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [DefaultValue(typeof(Color), "ControlDarkDark")]
        public Color ThumbColorActive
        {
            get => _thumbColorActive;
            set
            {
                _thumbColorActive = value;
                Invalidate();
            }
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
            {
                Width = 10;
                Height = 100;
            }
            else
            {
                Width = 100;
                Height = 10;
            }
        }

        // THEMING
        public override void ApplyTheme()
        {
            base.ApplyTheme();

                BackColor = _currentTheme.ScrollBarBackColor;
                TrackColor = _currentTheme.ScrollBarTrackColor;
                ThumbColor = _currentTheme.ScrollBarThumbColor;
                ThumbColorHover = _currentTheme.ScrollBarThumbColor;
                ThumbColorActive = _currentTheme.ScrollBarTrackColor;
            
            Invalidate();
        }
        protected override void DrawContent(Graphics g)
        {
            // First, call the base drawing code from BeepControl.
            UpdateDrawingRect();
            Rectangle drawingRect = DrawingRect;

            if (drawingRect.Width <= 0 || drawingRect.Height <= 0)
                return;

            // Draw background
            using (Brush backBrush = new SolidBrush(BackColor))
            {
                g.FillRectangle(backBrush, drawingRect);
            }

            // Use theme colors with hover/active states
            Color thumbColor = _dragging ? ThumbColorActive :
                              _isHovering ? ThumbColorHover :
                              ThumbColor;

            using (Brush trackBrush = new SolidBrush(TrackColor))
            using (Brush thumbBrush = new SolidBrush(thumbColor))
            {
                if (_maximum <= _minimum + _largeChange)
                {
                    g.FillRectangle(trackBrush, drawingRect);
                    g.FillRectangle(thumbBrush, drawingRect);
                    return;
                }

                if (_scrollOrientation == Orientation.Vertical)
                {
                    int range = _maximum - _minimum;
                    int thumbHeight = Math.Max(10, (drawingRect.Height * _largeChange) / range);
                    int trackLength = drawingRect.Height - thumbHeight;
                    if (trackLength < 1) trackLength = 1;

                    int valPosition = _value - _minimum;
                    int thumbY = drawingRect.Y + (trackLength * valPosition) / (range - _largeChange);

                    g.FillRectangle(trackBrush, drawingRect);
                    g.FillRectangle(thumbBrush, new Rectangle(drawingRect.X, thumbY, drawingRect.Width, thumbHeight));
                }
                else
                {
                    int range = _maximum - _minimum;
                    int thumbWidth = Math.Max(10, (drawingRect.Width * _largeChange) / range);
                    int trackLength = drawingRect.Width - thumbWidth;
                    if (trackLength < 1) trackLength = 1;

                    int valPosition = _value - _minimum;
                    int thumbX = drawingRect.X + (trackLength * valPosition) / (range - _largeChange);

                    g.FillRectangle(trackBrush, drawingRect);
                    g.FillRectangle(thumbBrush, new Rectangle(thumbX, drawingRect.Y, thumbWidth, drawingRect.Height));
                }
            }

            // If additional app bar elements (icons, text) require custom drawing,
            // they can be added here or left to be drawn by the child controls.
        }
        // RENDERING
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
           
       
        }

        // MOUSE INTERACTION
        protected override void OnMouseDown(MouseEventArgs e)
        {
         //   base.OnMouseDown(e);
            Rectangle drawingRect = DrawingRect;
            if (drawingRect.Width <= 0 || drawingRect.Height <= 0) return;
            if (_maximum <= _minimum + _largeChange) return;

            if (_scrollOrientation == Orientation.Vertical)
            {
                int range = _maximum - _minimum;
                int thumbHeight = Math.Max(10, (drawingRect.Height * _largeChange) / range);
                int trackLength = drawingRect.Height - thumbHeight;
                if (trackLength < 1) trackLength = 1;
                int valPosition = _value - _minimum;
                int thumbY = drawingRect.Y + (trackLength * valPosition) / (range - _largeChange);

                if (e.Y >= thumbY && e.Y <= thumbY + thumbHeight)
                {
                    _dragging = true;
                    _dragOffset = e.Y - thumbY;
                }
                else
                {
                    if (e.Y < thumbY)
                        Value = Value - LargeChange;
                    else
                        Value = Value + LargeChange;
                }
            }
            else
            {
                int range = _maximum - _minimum;
                int thumbWidth = Math.Max(10, (drawingRect.Width * _largeChange) / range);
                int trackLength = drawingRect.Width - thumbWidth;
                if (trackLength < 1) trackLength = 1;
                int valPosition = _value - _minimum;
                int thumbX = drawingRect.X + (trackLength * valPosition) / (range - _largeChange);

                if (e.X >= thumbX && e.X <= thumbX + thumbWidth)
                {
                    _dragging = true;
                    _dragOffset = e.X - thumbX;
                }
                else
                {
                    if (e.X < thumbX)
                        Value = Value - LargeChange;
                    else
                        Value = Value + LargeChange;
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (!_dragging) return;

            Rectangle drawingRect = DrawingRect;
            if (drawingRect.Width <= 0 || drawingRect.Height <= 0) return;
            if (_maximum <= _minimum + _largeChange) return;

            if (_scrollOrientation == Orientation.Vertical)
            {
                int range = _maximum - _minimum;
                int thumbHeight = Math.Max(10, (drawingRect.Height * _largeChange) / range);
                int trackLength = drawingRect.Height - thumbHeight;
                if (trackLength < 1) trackLength = 1;

                int newVal = (((e.Y - _dragOffset) - drawingRect.Y) * (range - _largeChange)) / trackLength + _minimum;
                newVal = Math.Max(_minimum, Math.Min(newVal, _maximum - _largeChange));
                Value = newVal;
            }
            else
            {
                int range = _maximum - _minimum;
                int thumbWidth = Math.Max(10, (drawingRect.Width * _largeChange) / range);
                int trackLength = drawingRect.Width - thumbWidth;
                if (trackLength < 1) trackLength = 1;

                int newVal = (((e.X - _dragOffset) - drawingRect.X) * (range - _largeChange)) / trackLength + _minimum;
                newVal = Math.Max(_minimum, Math.Min(newVal, _maximum - _largeChange));
                Value = newVal;
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _isHovering = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _isHovering = false;
            _dragging = false;
            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _dragging = false;
        }
    }
}