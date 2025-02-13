using System.ComponentModel;


namespace TheTechIdea.Beep.Winform.Controls;
[ToolboxItem(true)]
[Category("Controls")]
[DisplayName("Beep Scrollbar")]
[Description("a Scrollbar control")]
public class BeepScrollBar : BeepControl
{
    public event EventHandler Scroll;

    private int _value = 0;
    private int _maximum = 100;
    private int _largeChange = 10;

    private bool _dragging;
    private int _dragOffset;
    private Orientation _scrollOrientation = Orientation.Vertical;

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
                Width = 10;
            else
                Height = 10;
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
            _value = Math.Max(0, Math.Min(value, Math.Max(1, Maximum - LargeChange)));
            Invalidate();
            Scroll?.Invoke(this, EventArgs.Empty);
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
            _maximum = Math.Max(LargeChange + 1, value);
            Invalidate();
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
            _largeChange = Math.Max(1, Math.Min(value, Maximum - 1));
            Invalidate();
        }
    }

    /// <summary>
    /// Sets default size for the control.
    /// </summary>
    protected override Size DefaultSize => new Size(10, 100);

    public BeepScrollBar()
    {
        if (!DesignMode)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.UserPaint, true);
        }

         // Set a default size based on the current orientation.
        if (ScrollOrientation == Orientation.Vertical)
        {
            Width = 10;
            Height = 100; // a reasonable default height for vertical scrollbars
        }
        else
        {
            Width = 100; // a reasonable default width for horizontal scrollbars
            Height = 10;
        }
    }

   

    public override void ApplyTheme()
    {
        //base.ApplyTheme();

        //if ( BeepThemesManager.ThemeScrollBarColors.TryGetValue(Theme, out var colors))
        //{
        //    this.BackColor = colors.ScrollbarBackColor;
        //    if (_currentTheme != null)
        //    {
        //        _currentTheme.ScrollbarTrackColor = colors.ScrollbarTrackColor;
        //        _currentTheme.ScrollbarThumbColor = colors.ScrollbarThumbColor;
        //    }
        //}

        //Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        Graphics g = e.Graphics;
        UpdateDrawingRect();
        Rectangle drawingRect = DrawingRect;

        // If the control hasn't been sized yet, exit early.
        if (drawingRect.Width <= 0 || drawingRect.Height <= 0)
            return;

        // Draw the background.
        using (Brush backBrush = new SolidBrush(BackColor))
        {
            g.FillRectangle(backBrush, drawingRect);
        }

        // Use default colors if _currentTheme is null.
        Color trackColor = _currentTheme?.ScrollbarTrackColor ?? SystemColors.ControlDark;
        Color thumbColor = _currentTheme?.ScrollbarThumbColor ?? SystemColors.ControlDarkDark;

        using (Brush trackBrush = new SolidBrush(trackColor))
        using (Brush thumbBrush = new SolidBrush(thumbColor))
        {
            // If there's no scrollable range, fill the entire area.
            if (Maximum <= LargeChange)
            {
                g.FillRectangle(trackBrush, drawingRect);
                g.FillRectangle(thumbBrush, drawingRect);
                return;
            }

            if (ScrollOrientation == Orientation.Vertical)
            {
                int thumbHeight = Math.Max(10, (drawingRect.Height * LargeChange) / Maximum);
                int trackLength = drawingRect.Height - thumbHeight;
                if (trackLength <= 0)
                    trackLength = 1; // Prevent division by zero.
                int thumbY = drawingRect.Y + (trackLength * Value) / (Maximum - LargeChange);

                g.FillRectangle(trackBrush, drawingRect);
                g.FillRectangle(thumbBrush, new Rectangle(drawingRect.X, thumbY, drawingRect.Width, thumbHeight));
            }
            else // Horizontal orientation
            {
                int thumbWidth = Math.Max(10, (drawingRect.Width * LargeChange) / Maximum);
                int trackLength = drawingRect.Width - thumbWidth;
                if (trackLength <= 0)
                    trackLength = 1;
                int thumbX = drawingRect.X + (trackLength * Value) / (Maximum - LargeChange);

                g.FillRectangle(trackBrush, drawingRect);
                g.FillRectangle(thumbBrush, new Rectangle(thumbX, drawingRect.Y, thumbWidth, drawingRect.Height));
            }
        }
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        Rectangle drawingRect = DrawingRect;

        // If the control is not properly sized, do nothing.
        if (drawingRect.Width <= 0 || drawingRect.Height <= 0)
            return;

        // If there's no scrollable range, exit.
        if (Maximum <= LargeChange)
            return;

        if (ScrollOrientation == Orientation.Vertical)
        {
            int thumbHeight = Math.Max(10, (drawingRect.Height * LargeChange) / Maximum);
            int trackLength = drawingRect.Height - thumbHeight;
            if (trackLength <= 0)
                trackLength = 1;
            int thumbY = drawingRect.Y + (trackLength * Value) / (Maximum - LargeChange);

            // Check if the click is on the thumb.
            if (e.Y >= thumbY && e.Y <= thumbY + thumbHeight)
            {
                _dragging = true;
                _dragOffset = e.Y - thumbY;
            }
            else
            {
                int newValue = ((e.Y - drawingRect.Y) * (Maximum - LargeChange)) / trackLength;
                Value = newValue;
            }
        }
        else // Horizontal orientation
        {
            int thumbWidth = Math.Max(10, (drawingRect.Width * LargeChange) / Maximum);
            int trackLength = drawingRect.Width - thumbWidth;
            if (trackLength <= 0)
                trackLength = 1;
            int thumbX = drawingRect.X + (trackLength * Value) / (Maximum - LargeChange);

            if (e.X >= thumbX && e.X <= thumbX + thumbWidth)
            {
                _dragging = true;
                _dragOffset = e.X - thumbX;
            }
            else
            {
                int newValue = ((e.X - drawingRect.X) * (Maximum - LargeChange)) / trackLength;
                Value = newValue;
            }
        }
    }
    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        Rectangle drawingRect = DrawingRect;
        if (drawingRect.Width <= 0 || drawingRect.Height <= 0)
            return;

        if (_dragging && Maximum > LargeChange)
        {
            if (ScrollOrientation == Orientation.Vertical)
            {
                int thumbHeight = Math.Max(10, (drawingRect.Height * LargeChange) / Maximum);
                int trackLength = drawingRect.Height - thumbHeight;
                if (trackLength <= 0)
                    trackLength = 1;
                int newValue = ((e.Y - _dragOffset - drawingRect.Y) * (Maximum - LargeChange)) / trackLength;
                Value = Math.Max(0, Math.Min(newValue, Maximum - LargeChange));
            }
            else
            {
                int thumbWidth = Math.Max(10, (drawingRect.Width * LargeChange) / Maximum);
                int trackLength = drawingRect.Width - thumbWidth;
                if (trackLength <= 0)
                    trackLength = 1;
                int newValue = ((e.X - _dragOffset - drawingRect.X) * (Maximum - LargeChange)) / trackLength;
                Value = Math.Max(0, Math.Min(newValue, Maximum - LargeChange));
            }
        }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        _dragging = false;
    }
}
