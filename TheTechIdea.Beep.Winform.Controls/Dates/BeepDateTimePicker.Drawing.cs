using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Dates.Models;
using TheTechIdea.Beep.Winform.Controls.Dates.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Dates
{
    /// <summary>
    /// BeepDateTimePicker - Drawing partial: Direct painting using IDateTimePickerPainter (no dropdown)
    /// Similar to BeepDatePickerView - all rendering happens directly in the control
    /// </summary>
    public partial class BeepDateTimePicker
    {
        protected override void DrawContent(Graphics g)
        {     // Setup graphics quality
        
            base.DrawContent(g);
            UpdateDrawingRect();
            if (_currentPainter == null)
            {
                InitializePainter();
                if (_currentPainter == null) return;
            }
           

            // Get the drawing rectangle (from BaseControl)
            var drawingRect = DrawingRect;
            if (drawingRect.Width <= 0 || drawingRect.Height <= 0) return;

            // Update layout if needed
            if (_layout == null || _layout.CalendarGridRect.IsEmpty)
            {
                UpdateLayout();
            }

            // Let painter draw the complete calendar/picker interface
            var props = GetCurrentProperties();
           
            _currentPainter.PaintCalendar(g, drawingRect, props, _displayMonth, _hoverState);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateLayout();
        }
    }
}
