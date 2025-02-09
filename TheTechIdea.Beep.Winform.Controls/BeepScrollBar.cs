using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls;

public class BeepScrollBar : BeepControl
{
    public event EventHandler Scroll;

    private int _value;
    private int _maximum = 100;
    private int _largeChange = 10;
    private bool _dragging;
    private int _dragOffset;

    // Orientation property to allow switching between vertical and horizontal.
    private Orientation _scrollOrientation = Orientation.Vertical;
    public Orientation ScrollOrientation
    {
        get => _scrollOrientation;
        set
        {
            _scrollOrientation = value;
            // Adjust the default size when changing orientation.
            if (_scrollOrientation == Orientation.Vertical)
                Width = 10;
            else
                Height = 10;
            Invalidate();
        }
    }

    public int Value
    {
        get => _value;
        set
        {
            // Clamp value between 0 and (Maximum - LargeChange)
            _value = Math.Max(0, Math.Min(value, Maximum - LargeChange));
            Invalidate();
            Scroll?.Invoke(this, EventArgs.Empty);
        }
    }

    public int Maximum
    {
        get => _maximum;
        set
        {
            _maximum = Math.Max(1, value);
            Invalidate();
        }
    }

    public int LargeChange
    {
        get => _largeChange;
        set
        {
            _largeChange = Math.Max(1, value);
            Invalidate();
        }
    }

    public BeepScrollBar()
    {
        // Enable double buffering and related styles for smoother rendering.
        SetStyle(ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.ResizeRedraw |
                 ControlStyles.UserPaint, true);

        // Set a default size based on vertical orientation.
        Width = 10;
    }

    public override void ApplyTheme()
    {
        base.ApplyTheme();

        if (BeepThemesManager.ThemeScrollBarColors.TryGetValue(Theme, out var colors))
        {
            this.BackColor = colors.ScrollbarBackColor;
            _currentTheme.ScrollbarTrackColor = colors.ScrollbarTrackColor;
            _currentTheme.ScrollbarThumbColor = colors.ScrollbarThumbColor;
        }

        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        Graphics g = e.Graphics;

        // Use the DrawingRect provided by BeepControl to define the drawing area.
        Rectangle drawingRect = DrawingRect;

        // Clear the drawing area using the BackColor.
        using (Brush backBrush = new SolidBrush(BackColor))
        {
            g.FillRectangle(backBrush, drawingRect);
        }

        using (Brush trackBrush = new SolidBrush(_currentTheme.ScrollbarTrackColor))
        using (Brush thumbBrush = new SolidBrush(_currentTheme.ScrollbarThumbColor))
        {
            if (ScrollOrientation == Orientation.Vertical)
            {
                // Calculate the thumb height and Y position within drawingRect.
                int thumbHeight = Math.Max(10, (drawingRect.Height * LargeChange) / Maximum);
                int thumbY = drawingRect.Y + (drawingRect.Height - thumbHeight) * Value / (Maximum - LargeChange);

                // Draw the track and the thumb.
                g.FillRectangle(trackBrush, drawingRect);
                g.FillRectangle(thumbBrush, new Rectangle(drawingRect.X, thumbY, drawingRect.Width, thumbHeight));
            }
            else // Horizontal orientation
            {
                // Calculate the thumb width and X position within drawingRect.
                int thumbWidth = Math.Max(10, (drawingRect.Width * LargeChange) / Maximum);
                int thumbX = drawingRect.X + (drawingRect.Width - thumbWidth) * Value / (Maximum - LargeChange);

                // Draw the track and the thumb.
                g.FillRectangle(trackBrush, drawingRect);
                g.FillRectangle(thumbBrush, new Rectangle(thumbX, drawingRect.Y, thumbWidth, drawingRect.Height));
            }
        }
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        // Use DrawingRect for hit testing.
        Rectangle drawingRect = DrawingRect;

        if (ScrollOrientation == Orientation.Vertical)
        {
            int thumbHeight = Math.Max(10, (drawingRect.Height * LargeChange) / Maximum);
            int thumbY = drawingRect.Y + (drawingRect.Height - thumbHeight) * Value / (Maximum - LargeChange);

            // Check if the mouse click is within the thumb.
            if (e.Y >= thumbY && e.Y <= thumbY + thumbHeight)
            {
                _dragging = true;
                _dragOffset = e.Y - thumbY;
            }
            else
            {
                // Calculate new Value based on click position relative to drawingRect.
                int newValue = ((e.Y - drawingRect.Y) * (Maximum - LargeChange)) / (drawingRect.Height - thumbHeight);
                Value = newValue;
            }
        }
        else // Horizontal orientation
        {
            int thumbWidth = Math.Max(10, (drawingRect.Width * LargeChange) / Maximum);
            int thumbX = drawingRect.X + (drawingRect.Width - thumbWidth) * Value / (Maximum - LargeChange);

            if (e.X >= thumbX && e.X <= thumbX + thumbWidth)
            {
                _dragging = true;
                _dragOffset = e.X - thumbX;
            }
            else
            {
                int newValue = ((e.X - drawingRect.X) * (Maximum - LargeChange)) / (drawingRect.Width - thumbWidth);
                Value = newValue;
            }
        }
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        Rectangle drawingRect = DrawingRect;

        if (_dragging)
        {
            if (ScrollOrientation == Orientation.Vertical)
            {
                int thumbHeight = Math.Max(10, (drawingRect.Height * LargeChange) / Maximum);
                int newValue = ((e.Y - _dragOffset - drawingRect.Y) * (Maximum - LargeChange)) / (drawingRect.Height - thumbHeight);
                Value = Math.Max(0, Math.Min(newValue, Maximum - LargeChange));
            }
            else
            {
                int thumbWidth = Math.Max(10, (drawingRect.Width * LargeChange) / Maximum);
                int newValue = ((e.X - _dragOffset - drawingRect.X) * (Maximum - LargeChange)) / (drawingRect.Width - thumbWidth);
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
