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

        // COLORS
        private Color _trackColor = SystemColors.ControlDark;
        private Color _thumbColor = SystemColors.ControlDarkDark;

        // PROPERTIES
        /// <summary>
        /// Gets or sets the scrollbar orientation (Vertical or Horizontal).
        /// </summary>
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

        /// <summary>
        /// Gets or sets the minimum scroll value.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the maximum scroll value.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the current scroll value.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(0)]
        public int Value
        {
            get => _value;
            set
            {
                if (DesignMode) return;

                // Clamp between Minimum and (Maximum - LargeChange) if you want typical behavior
                // or clamp to [Minimum, Maximum] if you prefer that style.
                int newValue = Math.Max(_minimum, Math.Min(value, _maximum - _largeChange));
                if (_value != newValue)
                {
                    _value = newValue;
                    Invalidate();

                    // Fire events
                    ValueChanged?.Invoke(this, EventArgs.Empty);
                    Scroll?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the large change amount (scroll step size).
        /// </summary>
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

        /// <summary>
        /// Gets or sets the small change amount (arrow key or small step).
        /// </summary>
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

        /// <summary>
        /// Color of the scrollbar track (background).
        /// </summary>
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

        /// <summary>
        /// Color of the scrollbar thumb (the draggable part).
        /// </summary>
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

        /// <summary>
        /// Sets default size for the control.
        /// </summary>
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

        // NO-OP theming if you want to define your own later
        public override void ApplyTheme()
        {
            // If you have your own theming system, override or remove as needed.
            if (BeepThemesManager.ThemeScrollBarColors.TryGetValue(Theme, out var colors))
            {
                this.BackColor = colors.ScrollbarBackColor;
                if (_currentTheme != null)
                {
                    _currentTheme.ScrollbarTrackColor = colors.ScrollbarTrackColor;
                    _currentTheme.ScrollbarThumbColor = colors.ScrollbarThumbColor;
                }
            }
        }

        // RENDERING
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            UpdateDrawingRect();
            Rectangle drawingRect = DrawingRect;

            if (drawingRect.Width <= 0 || drawingRect.Height <= 0)
                return;

            // Draw background
            using (Brush backBrush = new SolidBrush(BackColor))
            {
                g.FillRectangle(backBrush, drawingRect);
            }

            using (Brush trackBrush = new SolidBrush(_currentTheme.ScrollbarTrackColor))
            using (Brush thumbBrush = new SolidBrush(_currentTheme.ScrollbarThumbColor))
            {
                // If there's effectively no scroll range, fill entire rect with the thumb
                if (_maximum <= _minimum + _largeChange)
                {
                    // track
                    g.FillRectangle(trackBrush, drawingRect);
                    // thumb
                    g.FillRectangle(thumbBrush, drawingRect);
                    return;
                }

                // Orientation-based drawing
                if (_scrollOrientation == Orientation.Vertical)
                {
                    // compute thumb height
                    int range = _maximum - _minimum;
                    int thumbHeight = Math.Max(10, (drawingRect.Height * _largeChange) / range);
                    int trackLength = drawingRect.Height - thumbHeight;
                    if (trackLength < 1) trackLength = 1;

                    // compute thumb Y based on Value
                    int valPosition = _value - _minimum;
                    int thumbY = drawingRect.Y + (trackLength * valPosition) / (range - _largeChange);

                    // draw track
                    g.FillRectangle(trackBrush, drawingRect);
                    // draw thumb
                    g.FillRectangle(thumbBrush, new Rectangle(drawingRect.X, thumbY, drawingRect.Width, thumbHeight));
                }
                else
                {
                    // Horizontal
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
        }

        // MOUSE INTERACTION
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
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

                // check if click on thumb
                if (e.Y >= thumbY && e.Y <= thumbY + thumbHeight)
                {
                    _dragging = true;
                    _dragOffset = e.Y - thumbY;
                }
                else
                {
                    // Page jump (LargeChange) or direct set
                    if (e.Y < thumbY)
                    {
                        // clicked above thumb => page up
                        Value = Value - LargeChange;
                    }
                    else
                    {
                        // clicked below thumb => page down
                        Value = Value + LargeChange;
                    }
                }
            }
            else
            {
                // Horizontal
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
                    // Page jump
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

                // offset-based
                int newVal = (((e.Y - _dragOffset) - drawingRect.Y) * (range - _largeChange)) / trackLength + _minimum;
                // clamp
                newVal = Math.Max(_minimum, Math.Min(newVal, _maximum - _largeChange));
                Value = newVal;
            }
            else
            {
                // Horizontal
                int range = _maximum - _minimum;
                int thumbWidth = Math.Max(10, (drawingRect.Width * _largeChange) / range);
                int trackLength = drawingRect.Width - thumbWidth;
                if (trackLength < 1) trackLength = 1;

                int newVal = (((e.X - _dragOffset) - drawingRect.X) * (range - _largeChange)) / trackLength + _minimum;
                newVal = Math.Max(_minimum, Math.Min(newVal, _maximum - _largeChange));
                Value = newVal;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _dragging = false;
        }
    }
}
