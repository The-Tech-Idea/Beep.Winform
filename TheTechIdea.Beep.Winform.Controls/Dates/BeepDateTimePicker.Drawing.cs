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
        /// <summary>
        /// DrawContent override - called by BaseControl
        /// </summary>
        protected override void DrawContent(Graphics g)
        {
            Paint(g, DrawingRect);
        }

        /// <summary>
        /// Draw override - called by BeepGridPro and containers
        /// </summary>
        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            Paint(graphics, rectangle);
        }

        /// <summary>
        /// Main paint function - centralized painting logic
        /// Called from both DrawContent and Draw
        /// </summary>
        private void Paint(Graphics g, Rectangle bounds)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            
            UpdateDrawingRect();
            
            if (_currentPainter == null)
            {
                InitializePainter();
                if (_currentPainter == null) return;
            }

            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            // Update layout if needed
            if (_layout == null || _layout.CalendarGridRect.IsEmpty)
            {
                UpdateLayout();
            }

            // Register hit areas
            var props = GetCurrentProperties();
            if (_layout != null && _hitHelper != null)
            {
                try
                {
                    _hitHelper.RegisterHitAreas(_layout, props, _displayMonth);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Hit area registration error: {ex.Message}");
                }
            }

            // Let painter draw the complete calendar/picker interface
            _currentPainter.PaintCalendar(g, bounds, props, _displayMonth, _hoverState);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateLayout();
        }
    }
}
