using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Calendar.Rendering;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (!IsDesignModeSafe)
            {
                RequestLayoutUpdate();
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (!IsDesignModeSafe)
            {
                RequestLayoutUpdate();
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (IsDesignModeSafe) return;
            Focus();
            _keyboardFocusVisible = false;

            Rectangle contentRect = GetContentRectForDrawing();

            var headerTextBounds = GetHeaderTextBounds();
            int headerLeft = Math.Max(0, headerTextBounds.Left - _rects.HeaderRect.X);
            int headerRight = Math.Max(0, _rects.HeaderRect.Right - headerTextBounds.Right);

            var ctx = new CalendarRenderContext(this, _currentTheme,
                HeaderFont, DayFont, EventFont, TimeFont, DaysHeaderFont,
                _state, _rects, _eventService, _categories, Resources,
                headerLeft, headerRight, GetDensityScale());
            _renderer.HandleClick(e.Location, ctx);
            UpdateViewButtonStates();
        }
    }
}