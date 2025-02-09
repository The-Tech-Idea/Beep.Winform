using System;
using System.Drawing;
using System.Windows.Forms;
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

    public int Value
    {
        get => _value;
        set
        {
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
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
        Width = 10;
   //     BackColor = Color.Transparent;
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
        g.Clear(BackColor);

        using (Brush trackBrush = new SolidBrush(_currentTheme.ScrollbarTrackColor))
        using (Brush thumbBrush = new SolidBrush(_currentTheme.ScrollbarThumbColor))
        using (Brush backBrush = new SolidBrush(_currentTheme.ScrollbarBackColor))
        {
            g.FillRectangle(backBrush, new Rectangle(0, 0, Width, Height));
            int thumbHeight = Math.Max(10, (Height * LargeChange) / Maximum);
            int thumbY = (Height - thumbHeight) * Value / (Maximum - LargeChange);
            g.FillRectangle(trackBrush, new Rectangle(0, 0, Width, Height));
            g.FillRectangle(thumbBrush, new Rectangle(0, thumbY, Width, thumbHeight));
        }
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        int thumbHeight = (Height * LargeChange) / Maximum;
        int thumbY = (Height - thumbHeight) * Value / (Maximum - LargeChange);
        if (e.Y >= thumbY && e.Y <= thumbY + thumbHeight)
        {
            _dragging = true;
            _dragOffset = e.Y - thumbY;
        }
        else
        {
            Value = (e.Y * Maximum) / Height;
        }
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        if (_dragging)
        {
            int newValue = ((e.Y - _dragOffset) * (Maximum - LargeChange)) / (Height - (Height * LargeChange) / Maximum);
            Value = Math.Max(0, Math.Min(newValue, Maximum - LargeChange));
        }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        _dragging = false;
    }
}